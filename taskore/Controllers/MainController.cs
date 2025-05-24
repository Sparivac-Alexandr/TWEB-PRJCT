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
        private readonly IReview _reviewService;
        private readonly IUser _userService;
        
        public MainController()
        {
            var bl = new BusinessLogic();
            _projectService = bl.GetProjectBL();
            _authService = bl.GetAuthBL();
            _sessionService = bl.GetSesionBL();
            _newsService = bl.GetNewsBL();
            _reviewService = bl.GetReviewBL();
            _userService = bl.GetUserBL();
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
                        // Get the full name of the user using the user service
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
        
        // GET: Main/Index - Default landing page
        public ActionResult Index()
        {
            // Redirect to the ExplorePage action which serves as main content page
            return View();
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
                
                // If not the current user, use the user service
                return _userService.GetUserName(userId);
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
            
            // Get user data from database using user service
            var user = _userService.GetUserById(userId);
            
            // Pass user data to the view using ViewBag
            ViewBag.UserData = user;
            
            // Count completed projects for this user
            try 
            {
                // Get all user applications
                var allUserApplications = _projectService.GetUserApplications(userId);
                
                // Get completed projects count
                var completedProjectsCount = allUserApplications.Count(p => p.Status == "Completed");
                
                // If user record exists, update CompletedProjects count
                if (user != null)
                {
                    _userService.UpdateUserCompletedProjects(userId, completedProjectsCount);
                }
                
                // Pass the count to the view
                ViewBag.CompletedProjectsCount = completedProjectsCount;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting completed projects count: {ex.Message}");
                ViewBag.CompletedProjectsCount = 0;
            }
            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserProfile(UDBModel model)
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
                // Find the existing user in the database
                var existingUser = _userService.GetUserById(userId);
                
                if (existingUser == null)
                {
                    // Create a new user if one doesn't exist
                    existingUser = new UDBModel
                    {
                        Id = userId,
                        Email = Session["UserEmail"]?.ToString(),
                        FirstName = Session["UserFullName"]?.ToString().Split(' ')[0],
                        LastName = Session["UserFullName"]?.ToString().Split(' ')[1] ?? "",
                        // Set default password - this should be changed in a real application
                        Password = "default-password-hash"
                    };
                }
                
                // Handle name update if provided
                if (!string.IsNullOrEmpty(model.FullName))
                {
                    var nameParts = model.FullName.Split(new[] { ' ' }, 2);
                    existingUser.FirstName = nameParts[0];
                    existingUser.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                    
                    // Update session data
                    Session["UserFullName"] = model.FullName;
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
                
                // Save changes to database using user service
                bool updateSuccess = _userService.UpdateUserProfile(existingUser);
                
                if (!updateSuccess)
                {
                    throw new Exception("Failed to update user profile");
                }
                
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

            int targetUserId;
            if (string.IsNullOrEmpty(userId))
            {
                targetUserId = (int)Session["UserId"];
            }
            else if (!int.TryParse(userId, out targetUserId))
            {
                return RedirectToAction("AllFreelancers");
            }

            // Get user details from database using user service
            UDBModel targetUser = _userService.GetUserById(targetUserId);
            if (targetUser == null)
            {
                return RedirectToAction("AllFreelancers");
            }

            // Get reviews for this user
            var reviews = _reviewService.GetReviewsForUser(targetUserId);

            // Get sender names for each review
            Dictionary<int, string> userNames = new Dictionary<int, string>();
            foreach (var review in reviews)
            {
                if (!userNames.ContainsKey(review.SenderId))
                {
                    string senderName = FetchUserName(review.SenderId);
                    userNames[review.SenderId] = senderName;
                }
            }

            // Pass data to view
            ViewBag.UserName = $"{targetUser.FirstName} {targetUser.LastName}";
            ViewBag.UserRating = targetUser.Rating ?? 0;
            ViewBag.ReviewCount = targetUser.RatingCount ?? 0;
            ViewBag.UserType = "freelancer"; // You may want to add this to your user model
            ViewBag.UserId = targetUser.Id;
            ViewBag.CurrentUserId = Session["UserId"];
            ViewBag.UserNames = userNames;

            return View(reviews);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReview(ReviewDBModel model)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "You must be logged in to leave a review." });
                }
                
                TempData["ErrorMessage"] = "You must be logged in to leave a review.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            // Set the sender ID to the current user
            model.SenderId = (int)Session["UserId"];
            
            // Ensure created date is set
            model.CreatedAt = DateTime.Now;
            
            // Validate that user isn't reviewing themselves
            if (model.SenderId == model.ReceiverId)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "You cannot review yourself." });
                }
                
                TempData["ErrorMessage"] = "You cannot review yourself.";
                return RedirectToAction("UserReviews", new { userId = model.ReceiverId });
            }
            
            try
            {
                bool isCreated = _reviewService.CreateReview(model);
                
                if (isCreated)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Review created successfully." });
                    }
                    
                    TempData["SuccessMessage"] = "Review submitted successfully!";
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "Failed to create review. Please try again." });
                    }
                    
                    TempData["ErrorMessage"] = "Failed to create review. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error creating review: " + ex.Message);
                
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "An error occurred while creating your review." });
                }
                
                TempData["ErrorMessage"] = "An error occurred while creating your review.";
            }
            
            return RedirectToAction("UserReviews", new { userId = model.ReceiverId });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteReview(int id, int userId)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "You must be logged in to delete a review." });
                }
                
                TempData["ErrorMessage"] = "You must be logged in to delete a review.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            int currentUserId = (int)Session["UserId"];
            
            try
            {
                // Get the review to check if this user is allowed to delete it
                var review = _reviewService.GetReviewById(id);
                
                if (review == null)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "Review not found." });
                    }
                    
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToAction("UserReviews", new { userId = userId });
                }
                
                // Only the review author or the profile owner can delete a review
                if (review.SenderId != currentUserId && review.ReceiverId != currentUserId)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "You are not authorized to delete this review." });
                    }
                    
                    TempData["ErrorMessage"] = "You are not authorized to delete this review.";
                    return RedirectToAction("UserReviews", new { userId = userId });
                }
                
                bool isDeleted = _reviewService.DeleteReview(id);
                
                if (isDeleted)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Review deleted successfully." });
                    }
                    
                    TempData["SuccessMessage"] = "Review deleted successfully!";
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "Failed to delete review. Please try again." });
                    }
                    
                    TempData["ErrorMessage"] = "Failed to delete review. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error deleting review: " + ex.Message);
                
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = "An error occurred while deleting the review." });
                }
                
                TempData["ErrorMessage"] = "An error occurred while deleting the review.";
            }
            
            return RedirectToAction("UserReviews", new { userId = userId });
        }
        
        public ActionResult CompletedProjects(string userId)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            try
            {
                // If userId is not provided, use the current user's ID
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Session["UserId"].ToString();
                }

                int userIdInt = Convert.ToInt32(userId);
                
                // Get user info from database for the specific requested user
                string userName = "User";
                string userType = "freelancer";
                
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userIdInt);
                    if (user != null)
                    {
                        userName = $"{user.FirstName} {user.LastName}";
                        // You could determine userType here if that's stored in the user record
                    }
                }
                
                // Get user applications
                var allUserApplications = _projectService.GetUserApplications(userIdInt);
                
                // Filter to only show completed projects for this specific user
                var completedProjects = allUserApplications.Where(p => p.Status == "Completed").ToList();
                
                // Set ViewBag values for the view
                ViewBag.UserName = userName;
                ViewBag.UserType = userType;
                ViewBag.UserId = userIdInt; // Add the userId to ViewBag for reference in the view
                
                return View(completedProjects);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading completed projects: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading completed projects.";
                return View(new List<ProjectApplicationDBModel>());
            }
        }
        
        public ActionResult UserProfile(string userId)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Auth");
            }

            // If no userId provided, show the user's own profile
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
                
                // Pass all user data to the view via ViewBag
                ViewBag.UserId = user.Id;
                ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
                ViewBag.UserEmail = user.Email;
                ViewBag.Phone = user.Phone ?? "";
                ViewBag.Location = user.Location ?? "";
                ViewBag.Website = user.Website ?? "";
                ViewBag.Headline = user.Headline ?? "";
                ViewBag.About = user.About ?? "";
                ViewBag.Skills = user.Skills ?? "";
                ViewBag.PreferredProjectTypes = user.PreferredProjectTypes ?? "";
                ViewBag.HourlyRate = user.HourlyRate ?? "";
                ViewBag.ProjectDuration = user.ProjectDuration ?? "";
                ViewBag.CommunicationStyle = user.CommunicationStyle ?? "";
                ViewBag.AvailabilityStatus = user.AvailabilityStatus ?? "Available";
                ViewBag.AvailabilityHours = user.AvailabilityHours ?? "";
                ViewBag.Rating = user.Rating ?? 0;
                ViewBag.RatingCount = user.RatingCount ?? 0;
                ViewBag.CompletedProjects = user.CompletedProjects ?? 0;
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
        
        public ActionResult FAQ()
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
                // Get all users from the database using user service
                List<UDBModel> allUsers = _userService.GetAllUsers();
                Debug.WriteLine($"Loaded {allUsers.Count} users from database");
                
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
                // Don't allow deletion of the current user
                if (Session["UserId"] != null)
                {
                    int currentUserId;
                    if (int.TryParse(Session["UserId"].ToString(), out currentUserId) && currentUserId == id)
                    {
                        return Json(new { success = false, message = "You cannot delete your own account." });
                    }
                }
                
                // Check if the user exists
                var user = _userService.GetUserById(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }
                
                // Delete the user using user service
                bool isDeleted = _userService.DeleteUser(id);
                
                if (isDeleted)
                {
                    return Json(new { success = true, message = "User deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete user. Please try again." });
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

        public ActionResult Chat(int? userId, int? newUserId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("SignIn", "Auth");

            int currentUserId = (int)Session["UserId"];
            DateTime lastVisit = Session["LastChatVisit"] != null ? (DateTime)Session["LastChatVisit"] : DateTime.MinValue;

            // Get current user and update last active time
            var currentUser = _userService.GetUserById(currentUserId);
            
            // The rest of the method needs to be updated to use session service or another service for chat messages
            using (var context = new taskoreBusinessLogic.DBModel.Seed.UserContext())
            {
                // Userii cu care ai mesaje
                var userIdsWithMessages = context.ChatMessages
                    .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                    .Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                    .Distinct()
                    .ToList();
                var users = context.Users.Where(u => userIdsWithMessages.Contains(u.Id)).ToList();
                ViewBag.ChatUsers = users;

                // Pentru badge per user în lista de chats
                var newMessagesPerUser = users.ToDictionary(
                    u => u.Id,
                    u => context.ChatMessages.Count(m => m.ReceiverId == currentUserId && m.SenderId == u.Id && m.SentAt > lastVisit)
                );
                ViewBag.NewMessagesPerUser = newMessagesPerUser;

                // Badge global = suma badge-urilor individuale (mesaje noi de la fiecare user)
                var newMessagesCount = newMessagesPerUser.Values.Sum();
                ViewBag.NewMessagesCount = newMessagesCount;

                // Selectare user pentru chat
                var selectedUser = userId.HasValue
                    ? context.Users.FirstOrDefault(u => u.Id == userId.Value)
                    : (newUserId.HasValue ? context.Users.FirstOrDefault(u => u.Id == newUserId.Value) : null);
                ViewBag.SelectedUser = selectedUser;

                List<taskoreBusinessLogic.DBModel.ChatMessageDBModel> messages = new List<taskoreBusinessLogic.DBModel.ChatMessageDBModel>();
                if (selectedUser != null)
                {
                    messages = context.ChatMessages
                        .Where(m => (m.SenderId == currentUserId && m.ReceiverId == selectedUser.Id) ||
                                    (m.SenderId == selectedUser.Id && m.ReceiverId == currentUserId))
                        .OrderBy(m => m.SentAt)
                        .ToList();
                }
                ViewBag.Messages = messages;
            }
            // Actualizez ultima vizită la chat
            Session["LastChatVisit"] = DateTime.Now;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(int ToUserId, string Message)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("SignIn", "Auth");

            int fromUserId = (int)Session["UserId"];
            if (ToUserId <= 0 || string.IsNullOrWhiteSpace(Message))
            {
                // Nu trimite mesaj dacă nu e selectat userul sau mesajul e gol
                return RedirectToAction("Chat");
            }
            using (var context = new taskoreBusinessLogic.DBModel.Seed.UserContext())
            {
                // Actualizez prezența online
                var me = context.Users.FirstOrDefault(u => u.Id == fromUserId);
                if (me != null) { me.LastActiveAt = DateTime.Now; context.SaveChanges(); }

                var toUser = context.Users.FirstOrDefault(u => u.Id == ToUserId);
                if (toUser == null)
                {
                    // Nu există destinatarul
                    return RedirectToAction("Chat");
                }
                var chatMsg = new taskoreBusinessLogic.DBModel.ChatMessageDBModel
                {
                    SenderId = fromUserId,
                    ReceiverId = ToUserId,
                    Content = Message,
                    SentAt = DateTime.Now
                };
                context.ChatMessages.Add(chatMsg);
                context.SaveChanges();
            }
            return RedirectToAction("Chat", new { userId = ToUserId });
        }

        // Method to handle applying for a project
        public ActionResult ApplyForProject(int projectId, bool deleteAfterApply = false)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to apply for a project.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            try
            {
                int freelancerId = (int)Session["UserId"];
                string freelancerName = Session["UserFullName"] as string ?? "Unknown Freelancer";
                
                // Get the project details
                var project = _projectService.GetProjectById(projectId);
                if (project == null)
                {
                    TempData["ErrorMessage"] = "Project not found.";
                    return RedirectToAction("ExplorePage");
                }
                
                // Check if user has already applied for this project
                using (var context = new ProjectApplicationContext())
                {
                    bool alreadyApplied = context.ProjectApplications
                        .Any(a => a.ProjectId == projectId && a.FreelancerId == freelancerId);
                    
                    if (alreadyApplied)
                    {
                        TempData["ErrorMessage"] = "You have already applied for this project.";
                        return RedirectToAction("ExplorePage");
                    }
                }
                
                // Get the client name
                string clientName = FetchUserName(project.UserId);
                
                // Create a project application model
                var application = new ProjectApplicationDBModel
                {
                    Title = project.Title,
                    Status = "In Progress",
                    Client = clientName,
                    Freelancer = freelancerName,
                    Budget = project.Budget,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.TryParse(project.Deadline, out DateTime deadline) ? deadline : DateTime.Now.AddMonths(1),
                    Progress = 0, // Initial progress
                    ProjectId = projectId,
                    ClientId = project.UserId,
                    FreelancerId = freelancerId
                };
                
                // Save the application
                bool isApplied = _projectService.ApplyForProject(application);
                
                if (isApplied)
                {
                    // If the application was successful and deleteAfterApply is true, delete the original project
                    if (deleteAfterApply)
                    {
                        bool isDeleted = _projectService.DeleteProject(projectId);
                        if (isDeleted)
                        {
                            TempData["SuccessMessage"] = "You have successfully applied for this project and it has been removed from the public listings.";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "You have successfully applied for this project, but there was an issue removing it from the public listings.";
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "You have successfully applied for this project.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to apply for this project. Please try again.";
                }
                
                return RedirectToAction("ExplorePage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying for project: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while applying for the project.";
                return RedirectToAction("ExplorePage");
            }
        }

        // View to show user's applied projects
        public ActionResult MyProjects()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to view your projects.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            try
            {
                int userId = (int)Session["UserId"];
                
                // Get all project applications for the current user
                var applications = _projectService.GetUserApplications(userId);
                
                return View(applications);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading projects: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading your projects.";
                return View(new List<ProjectApplicationDBModel>());
            }
        }
        
        // GET: Show edit form for a project application
        public ActionResult EditProjectApplication(int id)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to edit projects.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            try
            {
                int userId = (int)Session["UserId"];
                
                // Get the project application
                var projectApplication = _projectService.GetProjectApplicationById(id);
                
                // Check if project exists and belongs to the current user
                if (projectApplication == null)
                {
                    TempData["ErrorMessage"] = "Project not found.";
                    return RedirectToAction("MyProjects");
                }
                
                if (projectApplication.FreelancerId != userId && projectApplication.ClientId != userId)
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit this project.";
                    return RedirectToAction("MyProjects");
                }
                
                return View(projectApplication);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading project for editing: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading the project.";
                return RedirectToAction("MyProjects");
            }
        }
        
        // GET: Return project application data in JSON format
        [HttpGet]
        public ActionResult GetProjectApplicationById(int id)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to be logged in to get project details." }, JsonRequestBehavior.AllowGet);
            }
            
            try
            {
                int userId = (int)Session["UserId"];
                
                var projectApplication = _projectService.GetProjectApplicationById(id);
                
                if (projectApplication == null)
                {
                    return Json(new { success = false, message = "Project not found." }, JsonRequestBehavior.AllowGet);
                }
                
                if (projectApplication.FreelancerId != userId && projectApplication.ClientId != userId)
                {
                    return Json(new { success = false, message = "You are not authorized to view this project." }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { 
                    success = true, 
                    Id = projectApplication.Id,
                    Title = projectApplication.Title,
                    Status = projectApplication.Status,
                    Progress = projectApplication.Progress,
                    Client = projectApplication.Client,
                    Freelancer = projectApplication.Freelancer,
                    Budget = projectApplication.Budget,
                    StartDate = projectApplication.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = projectApplication.EndDate.ToString("yyyy-MM-dd")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting project {id}: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while getting the project details." }, JsonRequestBehavior.AllowGet);
            }
        }
        
        // POST: Handle the project application edit submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.ValidateInput(false)]
        public ActionResult UpdateProjectApplication([System.Web.Mvc.ModelBinder(typeof(System.Web.Mvc.DefaultModelBinder))]ProjectApplicationDBModel model)
        {
            Debug.WriteLine("UpdateProjectApplication: Request started");
            
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                Debug.WriteLine("UpdateProjectApplication: User not logged in");
                TempData["ErrorMessage"] = "You need to be logged in to edit projects.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            // Check if we're dealing with AJAX request or direct form submission
            bool isAjaxRequest = Request.IsAjaxRequest();
            
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("UpdateProjectApplication: ModelState is invalid");
                
                // Log each validation error
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        Debug.WriteLine($"Error in field {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
                
                if (isAjaxRequest)
                {
                    return Json(new { 
                        success = false, 
                        message = "Invalid data submitted. Please check all required fields.", 
                        errors = ModelState.Keys
                            .Where(k => ModelState[k].Errors.Count > 0)
                            .Select(k => new { field = k, errors = ModelState[k].Errors.Select(e => e.ErrorMessage).ToList() })
                            .ToList()
                    });
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid data submitted. Please check all required fields.";
                    return RedirectToAction("MyProjects");
                }
            }
            
            try
            {
                int userId = (int)Session["UserId"];
                int projectId = model.Id;
                
                Debug.WriteLine($"UpdateProjectApplication: Updating project {projectId} with Status={model.Status}, Progress={model.Progress}");
                Debug.WriteLine($"Model details: Title={model.Title}, Client={model.Client}, Freelancer={model.Freelancer}");
                Debug.WriteLine($"Budget={model.Budget}, StartDate={model.StartDate}, EndDate={model.EndDate}");
                Debug.WriteLine($"ProjectId={model.ProjectId}, ClientId={model.ClientId}, FreelancerId={model.FreelancerId}");
                
                // Get the existing project application
                var existingApplication = _projectService.GetProjectApplicationById(projectId);
                
                if (existingApplication == null)
                {
                    Debug.WriteLine($"UpdateProjectApplication: Project {projectId} not found");
                    
                    if (isAjaxRequest)
                    {
                        return Json(new { success = false, message = "Project not found." });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Project not found.";
                        return RedirectToAction("MyProjects");
                    }
                }
                
                Debug.WriteLine($"Found existing project: Title={existingApplication.Title}, Status={existingApplication.Status}");
                
                // Check if the user has permission to edit this project
                if (existingApplication.FreelancerId != userId && existingApplication.ClientId != userId)
                {
                    Debug.WriteLine($"UpdateProjectApplication: User {userId} not authorized to edit project {projectId}");
                    
                    if (isAjaxRequest)
                    {
                        return Json(new { success = false, message = "You are not authorized to edit this project." });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "You are not authorized to edit this project.";
                        return RedirectToAction("MyProjects");
                    }
                }
                
                // Update only the fields that can be edited
                existingApplication.Status = model.Status;
                existingApplication.Progress = model.Progress;
                existingApplication.UpdatedAt = DateTime.Now;
                
                Debug.WriteLine($"Calling service to update project {projectId}");
                
                // Save the changes
                bool isUpdated = _projectService.UpdateProjectApplication(existingApplication);
                
                Debug.WriteLine($"Update result: {isUpdated}");
                
                if (isUpdated)
                {
                    if (isAjaxRequest)
                    {
                        return Json(new { 
                            success = true, 
                            message = "Project updated successfully.",
                            projectId = model.Id,
                            status = model.Status,
                            progress = model.Progress
                        });
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Project updated successfully.";
                        return RedirectToAction("MyProjects");
                    }
                }
                else
                {
                    if (isAjaxRequest)
                    {
                        return Json(new { success = false, message = "Failed to update project." });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update project.";
                        return RedirectToAction("MyProjects");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating project: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (isAjaxRequest)
                {
                    return Json(new { success = false, message = "An error occurred while updating the project: " + ex.Message });
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the project: " + ex.Message;
                    return RedirectToAction("MyProjects");
                }
            }
        }
    }
}
