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
            return View();
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
                // For demonstration, we'll use a fixed user ID
                // In a real application, you would get this from the authenticated user
                int userId = 1; // Replace with your user authentication logic
                
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
                        return RedirectToAction("MyProfile");
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
