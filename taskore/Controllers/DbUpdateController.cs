using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;

namespace taskore.Controllers
{
    // Clasa de configurare pentru migrări
    public class Configuration : DbMigrationsConfiguration<UserContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
    
    public class DbUpdateController : Controller
    {
        // GET: /DbUpdate/UpdateUserTable
        public ActionResult UpdateUserTable()
        {
            try
            {
                // Pasul 1: Forțăm inițializarea bazei de date
                using (var context = new UserContext())
                {
                    // Forțăm inițializarea pentru a aplica strategia de DropCreateDatabaseIfModelChanges
                    context.Database.Initialize(force: true);
                    
                    // Testăm că modelul este corect prin efectuarea unei operații simple
                    var users = context.Users.ToList();
                    Debug.WriteLine($"Found {users.Count} users in database");
                }
                
                // Pasul 2: Încărcăm date implicite în baza de date
                using (var context = new UserContext())
                {
                    // Verificăm dacă avem utilizatori care nu au headline setat
                    var usersWithoutHeadline = context.Users.Where(u => u.Headline == null).ToList();
                    foreach (var user in usersWithoutHeadline)
                    {
                        // Actualizăm cu valori implicite
                        user.Headline = "Web Developer & UI/UX Designer";
                        user.About = "A passionate web developer and UI/UX designer with experience in creating responsive websites and user-friendly interfaces that deliver exceptional user experiences.";
                        user.Skills = "HTML5,CSS3,JavaScript,React.js,Node.js,Figma,UI/UX Design,Responsive Design,API Integration,WordPress";
                        user.Phone = "+1 234 567 8900";
                        user.Location = "New York, USA";
                        user.Website = "www.portfolio.com";
                        user.PreferredProjectTypes = "Web Development, UI/UX Design, E-commerce";
                        user.HourlyRate = "$45 - $65 per hour";
                        user.ProjectDuration = "Short to medium-term (1-6 months)";
                        user.CommunicationStyle = "Weekly video calls, daily messaging";
                        user.AvailabilityStatus = "Available for work";
                        user.AvailabilityHours = "30 hrs/week";
                        user.Rating = 4.8;
                        user.RatingCount = 40;
                        user.CompletedProjects = 23;
                    }
                    
                    // Salvăm modificările
                    context.SaveChanges();
                    
                    Debug.WriteLine($"Updated {usersWithoutHeadline.Count} users with default values");
                }
                
                return Content($"Database updated successfully! The user profiles now have all the required fields.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating database: {ex.Message}");
                return Content($"Error updating database: {ex.Message}");
            }
        }
    }
} 