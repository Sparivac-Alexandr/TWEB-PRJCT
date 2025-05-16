using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using taskore.Models;
using taskoreBusinessLogic;
using taskoreBusinessLogic.Interfaces;
using taskoreBusinessLogic.DBModel;
using taskoreDomain.Enteties.User;
using taskoreBusinessLogic.DBModel.Seed;

namespace taskore.Controllers
{
    public class MainController : Controller
    {
        private readonly IProject _projectService;
        private readonly IAuth _authService;
        private readonly ISesion _sessionService;
        private readonly INews _newsService;
        
        public MainController()
        {
            var bl = new BusinessLogic();
            _projectService = bl.GetProjectBL();
            _authService = bl.GetAuthBL();
            _sessionService = bl.GetSesionBL();
            _newsService = bl.GetNewsBL();
        }
        
        // GET: Main
        public ActionResult ExplorePage()
        {
            try
            {
                // Preluăm toate proiectele din baza de date
                var allProjects = _projectService.GetAllProjects();
                
                // Grupăm proiectele după categorie pentru a le afișa organizat
                var projectsByCategory = allProjects
                    .GroupBy(p => p.Category)
                    .ToDictionary(g => g.Key, g => g.ToList());
                
                Debug.WriteLine($"Loaded {allProjects.Count} projects from database");
                
                // Create a dictionary to store user names keyed by user ID
                Dictionary<int, string> userNames = new Dictionary<int, string>();
                
                // For each project, if we don't have the user's name yet, try to get it
                foreach (var project in allProjects)
                {
                    if (!userNames.ContainsKey(project.UserId))
                    {
                        // Get the full name of the user
                        string userName = FetchUserName(project.UserId);
                        userNames[project.UserId] = userName;
                    }
                }
                
                // Add the user names to ViewBag for use in the view
                ViewBag.UserNames = userNames;
                
                // Transmitem proiectele către view
                return View(projectsByCategory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading projects: {ex.Message}");
                // În caz de eroare, afișăm un view gol
                return View(new Dictionary<string, List<ProjectDBModel>>());
            }
        }
        
        // Helper method to fetch a user's name from their ID
        private string FetchUserName(int userId)
        {
            try
            {
                // First try to get the user name from the current session if it matches
                if (Session["UserId"] != null && (int)Session["UserId"] == userId && Session["UserFullName"] != null)
                {
                    return Session["UserFullName"].ToString();
                }
                
                // If not the current user, query the database directly
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        return $"{user.FirstName} {user.LastName}";
                    }
                }
                
                // If we couldn't get the user data, return a placeholder
                return "Unknown User";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching user name: {ex.Message}");
                return "Unknown User";
            }
        }

        public ActionResult ChatPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateProjectPage() {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to create a project.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProjectPage(ProjectDBModel model) 
        {
            Debug.WriteLine("Project creation request received: " + model.Title);
            
            // Verify user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to create a project.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            if (ModelState.IsValid)
            {
                // Get user information from session
                int userId = Session["UserId"] != null ? (int)Session["UserId"] : 1;
                string creatorName = Session["UserFullName"] as string ?? "Unknown User";
                
                try
                {
                    // Create project using the DBModel
                    var newProject = new ProjectDBModel
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Category = model.Category,
                        Budget = model.Budget,
                        Deadline = model.Deadline,
                        RequiredSkills = model.RequiredSkills,
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };
                    
                    bool isCreated = _projectService.CreateProject(newProject);
                    
                    if (isCreated)
                    {
                        TempData["SuccessMessage"] = "Project created successfully!";
                        // Reinițializăm modelul pentru a permite crearea unui nou proiect
                        ModelState.Clear();
                        return View(new ProjectDBModel());
                    }
                    else
                    {
                        ModelState.AddModelError("", "Project creation failed. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error creating project: " + ex.Message);
                    ModelState.AddModelError("", "Project creation failed. Please try again.");
                }
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        public ActionResult MyProfile()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            int userId = (int)Session["UserId"];
            
            // Get user data from database
            using (var context = new taskoreBusinessLogic.DBModel.Seed.UserContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                
                // Pass user data to the view using ViewBag
                ViewBag.UserData = user;
                
                // If user data doesn't exist in database yet, we'll use session data
                // The profile page already handles null user data gracefully
            }

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserProfile(taskoreBusinessLogic.DBModel.UDBModel model)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                // For AJAX requests
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "You must be logged in to update your profile." });
                }
                // For standard form submissions
                TempData["ErrorMessage"] = "You must be logged in to update your profile.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            int userId = (int)Session["UserId"];
            
            try
            {
                using (var context = new taskoreBusinessLogic.DBModel.Seed.UserContext())
                {
                    // Find the existing user in the database
                    var existingUser = context.Users.FirstOrDefault(u => u.Id == userId);
                    
                    if (existingUser == null)
                    {
                        // Create a new user if one doesn't exist
                        existingUser = new taskoreBusinessLogic.DBModel.UDBModel
                        {
                            Id = userId,
                            Email = Session["UserEmail"]?.ToString(),
                            FirstName = Session["UserFullName"]?.ToString().Split(' ')[0],
                            LastName = Session["UserFullName"]?.ToString().Split(' ')[1] ?? "",
                            // Set default password - this should be changed in a real application
                            Password = "default-password-hash"
                        };
                        context.Users.Add(existingUser);
                    }
                    
                    // Update user fields only if they are provided in the form
                    // Basic information
                    if (!string.IsNullOrEmpty(model.Headline))
                        existingUser.Headline = model.Headline;
                    
                    if (!string.IsNullOrEmpty(model.About))
                        existingUser.About = model.About;
                    
                    if (!string.IsNullOrEmpty(model.Skills))
                        existingUser.Skills = model.Skills;
                    
                    // Contact information
                    if (!string.IsNullOrEmpty(model.Email))
                        existingUser.Email = model.Email;
                    
                    if (!string.IsNullOrEmpty(model.Phone))
                        existingUser.Phone = model.Phone;
                    
                    if (!string.IsNullOrEmpty(model.Location))
                        existingUser.Location = model.Location;
                    
                    if (!string.IsNullOrEmpty(model.Website))
                        existingUser.Website = model.Website;
                    
                    // Preferences
                    if (!string.IsNullOrEmpty(model.PreferredProjectTypes))
                        existingUser.PreferredProjectTypes = model.PreferredProjectTypes;
                    
                    if (!string.IsNullOrEmpty(model.HourlyRate))
                        existingUser.HourlyRate = model.HourlyRate;
                    
                    if (!string.IsNullOrEmpty(model.ProjectDuration))
                        existingUser.ProjectDuration = model.ProjectDuration;
                    
                    if (!string.IsNullOrEmpty(model.CommunicationStyle))
                        existingUser.CommunicationStyle = model.CommunicationStyle;
                    
                    // Availability
                    if (!string.IsNullOrEmpty(model.AvailabilityStatus))
                        existingUser.AvailabilityStatus = model.AvailabilityStatus;
                    
                    if (!string.IsNullOrEmpty(model.AvailabilityHours))
                    {
                        // Add hrs/week if it's not already included
                        if (!model.AvailabilityHours.Contains("hrs/week"))
                            existingUser.AvailabilityHours = model.AvailabilityHours + " hrs/week";
                        else
                            existingUser.AvailabilityHours = model.AvailabilityHours;
                    }
                    
                    // Save changes to database
                    context.SaveChanges();
                    
                    // Update session data if needed
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        Session["UserEmail"] = model.Email;
                    }
                    
                    // For AJAX requests
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Profile updated successfully" });
                    }
                    
                    // For standard form submissions
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("MyProfile");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error updating user profile: " + ex.Message);
                
                // For AJAX requests
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "An error occurred while updating your profile: " + ex.Message });
                }
                
                // For standard form submissions
                TempData["ErrorMessage"] = "An error occurred while updating your profile: " + ex.Message;
                return RedirectToAction("MyProfile");
            }
        }
        
        public ActionResult UserReviews(string userId)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            // In a real application, you would fetch the user data and reviews from a database
            // Here we're just passing the user ID from the query string
            if (string.IsNullOrEmpty(userId))
            {
                userId = Session["UserId"].ToString();
            }

            // Example: Get user details from database based on userId
            // var user = db.Users.Find(userId);
            // ViewBag.UserName = user.FullName;
            // ViewBag.UserRating = user.Rating;
            // ViewBag.ReviewCount = user.Reviews.Count;
            // ViewBag.UserType = user.UserType;

            // For demo purposes, we'll use default values
            ViewBag.UserName = Session["UserFullName"] ?? "User";
            ViewBag.UserRating = 4.8;
            ViewBag.ReviewCount = 40;
            ViewBag.UserType = Session["UserType"] ?? "freelancer";

            return View();
        }
        
        public ActionResult CompletedProjects(string userId)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            // In a real application, you would fetch the user data and completed projects from a database
            if (string.IsNullOrEmpty(userId))
            {
                userId = Session["UserId"].ToString();
            }

            return View();
        }
        
        public ActionResult UserProfile(string userId)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            // In a real application, you would fetch user data from the database
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("MyProfile");
            }

            // Using DBContext to get user data by ID
            using (var db = new taskoreBusinessLogic.DBModel.Seed.UserContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id.ToString() == userId);
                if (user == null)
                {
                    return RedirectToAction("AllFreelancers");
                }
            }
            
            return View();
        }
        
        public ActionResult AdminDashboard()
        {
            // Check if user is logged in and is an admin
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }
            
            // In a production environment, you should check if the user is an admin
            // Example: if(Session["UserRole"].ToString() != "Admin") { return RedirectToAction("Index", "Home"); }
            
            // Get some stats for the dashboard
            using (var db = new taskoreBusinessLogic.DBModel.Seed.UserContext())
            {
                // Count total users
                ViewBag.TotalUsers = db.Users.Count();
                
                // You could add more statistics here
                // ViewBag.TotalProjects = db.Projects.Count();
                // ViewBag.CompletedTasks = db.Tasks.Count(t => t.Status == "Completed");
                // ViewBag.PendingTasks = db.Tasks.Count(t => t.Status == "Pending");
            }

            // Get news stats
            try
            {
                using (var newsContext = new taskoreBusinessLogic.DBModel.Seed.NewsContext())
                {
                    ViewBag.TotalNews = newsContext.News.Count();
                    ViewBag.HighPriorityNews = newsContext.News.Count(n => n.Priority == "High");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getting news stats: " + ex.Message);
                ViewBag.TotalNews = 0;
                ViewBag.HighPriorityNews = 0;
            }
            
            // Create empty model for the news form
            ViewBag.NewsModel = new NewsDBModel
            {
                PublishDate = DateTime.Now,
                Author = Session["UserFullName"]?.ToString() ?? "Admin"
            };
            
            // Based on TempData, show the correct section
            if (TempData["SuccessMessage"] != null || TempData["ErrorMessage"] != null)
            {
                ViewBag.ActiveSection = "add-post-section";
            }
            
            return View();
        }
        
        public ActionResult Terms()
        {
            return View();
        }
        
        public ActionResult AboutUs()
        {
            return View();
        }
        
        public ActionResult Privacy()
        {
            return View();
        }
        
        public ActionResult ContactUs()
        {
            return View();
        }
        
        public ActionResult UserStats(string userId)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            // In a real application, you would fetch the user's statistics from the database
            if (string.IsNullOrEmpty(userId))
            {
                userId = Session["UserId"].ToString();
            }

            // For demo purposes, we'll use default values
            ViewBag.UserName = Session["UserFullName"] ?? "User";
            ViewBag.UserType = Session["UserType"] ?? "freelancer";
            
            // Sample statistics data
            if (ViewBag.UserType == "freelancer")
            {
                ViewBag.TotalEarned = 8750;
                ViewBag.TotalProjects = 23;
                ViewBag.AverageRating = 4.8;
                ViewBag.AverageProjectValue = 380;
                ViewBag.CompletionRate = 95;
                ViewBag.ResponseRate = 98;
                ViewBag.AverageResponseTime = 3.5;
            }
            else
            {
                ViewBag.TotalSpent = 12500;
                ViewBag.TotalProjects = 15;
                ViewBag.AverageProjectValue = 833;
                ViewBag.HiredFreelancers = 18;
                ViewBag.AverageRating = 4.9;
                ViewBag.ResponseRate = 92;
                ViewBag.AverageResponseTime = 5.2;
            }

            return View();
        }
        
        public ActionResult AllFreelancers()
        {
            try
            {
                // Get all users from the database
                List<UDBModel> allUsers = new List<UDBModel>();
                
                using (var context = new UserContext())
                {
                    // Retrieve all users from the database
                    allUsers = context.Users.ToList();
                    Debug.WriteLine($"Loaded {allUsers.Count} users from database");
                }
                
                // Pass the data directly to the view without adding any random data
                return View(allUsers);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading freelancers: {ex.Message}");
                // In case of error, display an empty view
                return View(new List<UDBModel>());
            }
        }
        
        public ActionResult Deals()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }
            
            // Model pentru a stoca date despre deals - folosim un dictionary în loc de obiect anonim
            var dealsModel = new List<Dictionary<string, object>>();
            
            // Adăugăm primul deal
            var deal1 = new Dictionary<string, object>
            {
                { "Id", 1 },
                { "Title", "Website Redesign Project" },
                { "ClientName", "Tech Solutions Inc." },
                { "FreelancerName", "Alex Johnson" },
                { "Status", "In Progress" },
                { "Budget", "$2,500" },
                { "StartDate", "2023-11-15" },
                { "Deadline", "2024-01-30" },
                { "Progress", 65 },
                { "Messages", 28 },
                { "LastActivity", "3 hours ago" }
            };
            dealsModel.Add(deal1);
            
            // Adăugăm al doilea deal
            var deal2 = new Dictionary<string, object>
            {
                { "Id", 2 },
                { "Title", "Mobile App Development" },
                { "ClientName", "Green Innovations" },
                { "FreelancerName", "Sarah Williams" },
                { "Status", "Pending Approval" },
                { "Budget", "$5,000" },
                { "StartDate", "2023-12-01" },
                { "Deadline", "2024-02-28" },
                { "Progress", 30 },
                { "Messages", 14 },
                { "LastActivity", "1 day ago" }
            };
            dealsModel.Add(deal2);
            
            // Adăugăm al treilea deal
            var deal3 = new Dictionary<string, object>
            {
                { "Id", 3 },
                { "Title", "E-commerce Integration" },
                { "ClientName", "Fashion Outlet" },
                { "FreelancerName", "David Chen" },
                { "Status", "Completed" },
                { "Budget", "$1,800" },
                { "StartDate", "2023-10-05" },
                { "Deadline", "2023-11-20" },
                { "Progress", 100 },
                { "Messages", 42 },
                { "LastActivity", "1 week ago" }
            };
            dealsModel.Add(deal3);
            
            // Adăugăm al patrulea deal
            var deal4 = new Dictionary<string, object>
            {
                { "Id", 4 },
                { "Title", "SEO Optimization" },
                { "ClientName", "Healthy Life Blog" },
                { "FreelancerName", "Emma Rodriguez" },
                { "Status", "In Review" },
                { "Budget", "$800" },
                { "StartDate", "2023-11-25" },
                { "Deadline", "2023-12-15" },
                { "Progress", 90 },
                { "Messages", 16 },
                { "LastActivity", "2 days ago" }
            };
            dealsModel.Add(deal4);
            
            // Adăugăm al cincilea deal
            var deal5 = new Dictionary<string, object>
            {
                { "Id", 5 },
                { "Title", "Logo & Branding Package" },
                { "ClientName", "Startup Ventures" },
                { "FreelancerName", "Michael Thompson" },
                { "Status", "In Progress" },
                { "Budget", "$1,200" },
                { "StartDate", "2023-12-05" },
                { "Deadline", "2024-01-10" },
                { "Progress", 45 },
                { "Messages", 21 },
                { "LastActivity", "5 hours ago" }
            };
            dealsModel.Add(deal5);
            
            return View(dealsModel);
        }

        public ActionResult News()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            try
            {
                // Get news from database using the News service
                var allNews = _newsService.GetAllNews();
                
                // Convert to the format expected by the view
                var newsModel = allNews.Select(n => new Dictionary<string, object>
                {
                    { "Id", n.Id },
                    { "Title", n.Title },
                    { "Content", n.Content },
                    { "Author", n.Author },
                    { "PublishDate", n.PublishDate.ToString("yyyy-MM-dd") },
                    { "Category", n.Category },
                    { "ImageUrl", n.ImageUrl ?? "/wwwroot/images/news/default.jpg" },
                    { "Priority", n.Priority }
                }).ToList();
                
                // If no news found in database, create some sample news
                if (!newsModel.Any())
                {
                    // Create sample news data
                    newsModel = CreateSampleNewsData();
                }
                
                return View(newsModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading news: {ex.Message}");
                // In case of error, create some sample news
                var newsModel = CreateSampleNewsData();
                return View(newsModel);
            }
        }
        
        private List<Dictionary<string, object>> CreateSampleNewsData()
        {
            var newsModel = new List<Dictionary<string, object>>();
           
            
            return newsModel;
        }
        
        [HttpGet]
        public ActionResult CreateNewsPage()
        {
            // Check if user is logged in and is an admin
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to create news.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            // In production, check if user is admin
            // if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin") 
            // {
            //     return RedirectToAction("News", "Main");
            // }
            
            return View(new NewsDBModel { 
                PublishDate = DateTime.Now,
                Author = Session["UserFullName"]?.ToString() ?? "Admin"
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewsPage(NewsDBModel model, string returnUrl = null)
        {
            Debug.WriteLine("News creation request received: " + model.Title);
            
            // Verify user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to create news.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Set the publish date to now if not already set
                    if (model.PublishDate == default(DateTime))
                    {
                        model.PublishDate = DateTime.Now;
                    }
                    
                    bool isCreated = _newsService.CreateNews(model);
                    
                    if (isCreated)
                    {
                        TempData["SuccessMessage"] = "News created successfully!";
                        // Reset the model for creating a new news item
                        ModelState.Clear();
                        
                        // If the request came from admin dashboard, return there
                        if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.Contains("AdminDashboard"))
                        {
                            return RedirectToAction("AdminDashboard");
                        }
                        
                        // Otherwise go to News page
                        return RedirectToAction("News");
                    }
                    else
                    {
                        ModelState.AddModelError("", "News creation failed. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error creating news: " + ex.Message);
                    ModelState.AddModelError("", "News creation failed. Please try again.");
                }
            }
            
            // If we got this far, something failed
            // If coming from admin dashboard, redirect there with error message
            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.Contains("AdminDashboard"))
            {
                TempData["ErrorMessage"] = "News creation failed. Please check the form and try again.";
                return RedirectToAction("AdminDashboard");
            }
            
            // Otherwise redisplay form
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNews(NewsDBModel model)
        {
            Debug.WriteLine("News creation request received from Admin Dashboard: " + model.Title);
            
            // Verify user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to create news.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Set the publish date to now
                    model.PublishDate = DateTime.Now;
                    
                    bool isCreated = _newsService.CreateNews(model);
                    
                    if (isCreated)
                    {
                        TempData["SuccessMessage"] = "News post created successfully!";
                        ViewBag.ActiveSection = "manage-posts-section";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "News creation failed. Please try again.";
                        ViewBag.ActiveSection = "add-post-section";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error creating news: " + ex.Message);
                    TempData["ErrorMessage"] = "News creation failed: " + ex.Message;
                    ViewBag.ActiveSection = "add-post-section";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
                ViewBag.ActiveSection = "add-post-section";
            }
            
            // Return to the admin dashboard
            return RedirectToAction("AdminDashboard");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNews(int id)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "You need to be logged in to delete news." });
                }
                
                TempData["ErrorMessage"] = "You need to be logged in to delete news.";
                return RedirectToAction("AdminDashboard");
            }
            
            bool isDeleted = false;
            
            try
            {
                isDeleted = _newsService.DeleteNews(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting news {id}: " + ex.Message);
            }
            
            if (Request.IsAjaxRequest())
            {
                if (isDeleted)
                {
                    return Json(new { success = true, message = "News deleted successfully." });
                }
                return Json(new { success = false, message = "Failed to delete news. News not found or already deleted." });
            }
            
            if (isDeleted)
            {
                TempData["SuccessMessage"] = "News deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete news. News not found or already deleted.";
            }
            
            ViewBag.ActiveSection = "manage-posts-section";
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        public ActionResult GetNewsById(int id)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to be logged in to get news details." }, JsonRequestBehavior.AllowGet);
            }
            
            try
            {
                var news = _newsService.GetNewsById(id);
                
                if (news != null)
                {
                    return Json(new { 
                        success = true, 
                        Id = news.Id,
                        Title = news.Title,
                        Content = news.Content,
                        Category = news.Category,
                        Priority = news.Priority,
                        Author = news.Author,
                        PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                        ImageUrl = news.ImageUrl
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "News not found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting news {id}: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while getting the news." }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(NewsDBModel model)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to be logged in to edit news." });
            }
            
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data submitted." });
            }
            
            try
            {
                // Get the existing news item
                var existingNews = _newsService.GetNewsById(model.Id);
                
                if (existingNews == null)
                {
                    return Json(new { success = false, message = "News not found." });
                }
                
                // Update the fields
                existingNews.Title = model.Title;
                existingNews.Content = model.Content;
                existingNews.Category = model.Category;
                existingNews.Priority = model.Priority;
                existingNews.Author = model.Author;
                existingNews.ImageUrl = model.ImageUrl;
                
                // Save the changes
                bool isUpdated = _newsService.UpdateNews(existingNews);
                
                if (isUpdated)
                {
                    return Json(new { success = true, message = "News updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update news." });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating news {model.Id}: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while updating the news." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int id)
        {
            // Check if user is logged in with admin privileges
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to be logged in to delete users." });
            }
            
            // In a real application, check if user is an admin
            // if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin") 
            // {
            //     return Json(new { success = false, message = "You do not have permission to delete users." });
            // }
            
            try
            {
                using (var context = new taskoreBusinessLogic.DBModel.Seed.UserContext())
                {
                    // Don't allow deletion of the current user
                    if (Session["UserId"] != null)
                    {
                        int currentUserId;
                        if (int.TryParse(Session["UserId"].ToString(), out currentUserId) && currentUserId == id)
                        {
                            return Json(new { success = false, message = "You cannot delete your own account." });
                        }
                    }
                    
                    var user = context.Users.Find(id);
                    if (user == null)
                    {
                        return Json(new { success = false, message = "User not found." });
                    }
                    
                    // Delete the user
                    context.Users.Remove(user);
                    int rowsAffected = context.SaveChanges();
                    
                    if (rowsAffected > 0)
                    {
                        return Json(new { success = true, message = "User deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to delete user. Please try again." });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error deleting user: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while deleting the user." });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProject(int id)
        {
            // Check if user is logged in with admin privileges
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to be logged in to delete projects." });
            }
            
            try
            {
                bool isDeleted = _projectService.DeleteProject(id);
                
                if (isDeleted)
                {
                    return Json(new { success = true, message = "Project deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete project. Project not found or already deleted." });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error deleting project: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while deleting the project." });
            }
        }
        
        [HttpGet]
        public ActionResult GetProjectById(int id)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to be logged in to get project details." }, JsonRequestBehavior.AllowGet);
            }
            
            try
            {
                var project = _projectService.GetProjectById(id);
                
                if (project != null)
                {
                    return Json(new { 
                        success = true, 
                        Id = project.Id,
                        Title = project.Title,
                        Description = project.Description,
                        Category = project.Category,
                        Budget = project.Budget,
                        Deadline = project.Deadline,
                        RequiredSkills = project.RequiredSkills,
                        UserId = project.UserId,
                        CreatedAt = project.CreatedAt.ToString("yyyy-MM-dd")
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Project not found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting project {id}: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while getting the project details." }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProject(ProjectDBModel model)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to be logged in to edit projects." });
            }
            
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data submitted." });
            }
            
            try
            {
                // Get the existing project
                var existingProject = _projectService.GetProjectById(model.Id);
                
                if (existingProject == null)
                {
                    return Json(new { success = false, message = "Project not found." });
                }
                
                // Update the fields
                existingProject.Title = model.Title;
                existingProject.Description = model.Description;
                existingProject.Category = model.Category;
                existingProject.Budget = model.Budget;
                existingProject.Deadline = model.Deadline;
                existingProject.RequiredSkills = model.RequiredSkills;
                
                // Save the changes
                bool isUpdated = _projectService.UpdateProject(existingProject);
                
                if (isUpdated)
                {
                    return Json(new { success = true, message = "Project updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update project." });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating project {model.Id}: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while updating the project." });
            }
        }
    }
}
