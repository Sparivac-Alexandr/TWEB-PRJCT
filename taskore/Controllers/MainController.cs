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

namespace taskore.Controllers
{
    public class MainController : Controller
    {
        private readonly IProject _projectService;
        
        public MainController()
        {
            var bl = new BusinessLogic();
            _projectService = bl.GetProjectBL();
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

        public ActionResult ChatPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateProjectPage() {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProjectPage(ProjectDBModel model) 
        {
            Debug.WriteLine("Project creation request received: " + model.Title);
            
            if (ModelState.IsValid)
            {
                // Pentru demonstrație, putem folosi ID-ul utilizatorului din sesiune
                int userId = Session["UserId"] != null ? (int)Session["UserId"] : 1;
                
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
            // Verificăm dacă utilizatorul este autentificat
            if (Session["UserId"] == null)
            {
                // Dacă nu este autentificat, redirectăm către pagina de login
                TempData["ErrorMessage"] = "Please login to view your profile.";
                return RedirectToAction("SignIn", "Auth");
            }
            
            // Dacă este autentificat, afișăm profilul
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
    }
}
