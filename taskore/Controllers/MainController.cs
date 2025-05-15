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
        
        public MainController()
        {
            var bl = new BusinessLogic();
            _projectService = bl.GetProjectBL();
            _authService = bl.GetAuthBL();
            _sessionService = bl.GetSesionBL();
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

            return View();
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

                // Pass user data to the view
                ViewBag.UserId = user.Id;
                ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
                ViewBag.UserEmail = user.Email;
                
                // Pass additional profile fields to the view
                ViewBag.Headline = user.Headline ?? "Web Developer & UI/UX Designer";
                ViewBag.About = user.About ?? "A passionate web developer and UI/UX designer with experience in creating responsive websites and user-friendly interfaces that deliver exceptional user experiences.";
                ViewBag.Skills = user.Skills ?? "HTML5,CSS3,JavaScript,React.js,Node.js,Figma,UI/UX Design,Responsive Design,API Integration,WordPress";
                ViewBag.Phone = user.Phone ?? "+1 234 567 8900";
                ViewBag.Location = user.Location ?? "New York, USA";
                ViewBag.Website = user.Website ?? "www.portfolio.com";
                ViewBag.PreferredProjectTypes = user.PreferredProjectTypes ?? "Web Development, UI/UX Design, E-commerce";
                ViewBag.HourlyRate = user.HourlyRate ?? "$45 - $65 per hour";
                ViewBag.ProjectDuration = user.ProjectDuration ?? "Short to medium-term (1-6 months)";
                ViewBag.CommunicationStyle = user.CommunicationStyle ?? "Weekly video calls, daily messaging";
                ViewBag.AvailabilityStatus = user.AvailabilityStatus ?? "Available for work";
                ViewBag.AvailabilityHours = user.AvailabilityHours ?? "30 hrs/week";
                ViewBag.Rating = user.Rating ?? 4.8;
                ViewBag.RatingCount = user.RatingCount ?? 40;
                ViewBag.CompletedProjects = user.CompletedProjects ?? 23;
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
    }
}
