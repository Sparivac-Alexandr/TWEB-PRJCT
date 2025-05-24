using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using taskoreBusinessLogic.DBModel.Seed;

namespace taskoreBusinessLogic.BL_Struct
{
    public class DbUpdateBL : IDbUpdate
    {
        public bool InitializeDatabase()
        {
            try
            {
                // Force database initialization
                using (var context = new UserContext())
                {
                    context.Database.Initialize(force: true);
                    
                    // Test that model is correct by performing a simple operation
                    var users = context.Users.ToList();
                    Debug.WriteLine($"Found {users.Count} users in database");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return false;
            }
        }

        public bool UpdateUserProfiles()
        {
            try
            {
                using (var context = new UserContext())
                {
                    // Check if we have users without a headline set
                    var usersWithoutHeadline = context.Users.Where(u => u.Headline == null).ToList();
                    foreach (var user in usersWithoutHeadline)
                    {
                        // Update with default values
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
                        user.CurrentProjects = 2;
                        user.OnTimePercentage = 92.5;
                        user.CompletionRate = 95.0;
                        user.ResponseRate = 98.0;
                        user.ClientSatisfaction = 96.0;
                        user.ProjectEfficiency = 87.5;
                    }
                    
                    // Save the changes
                    context.SaveChanges();
                    
                    Debug.WriteLine($"Updated {usersWithoutHeadline.Count} users with default values");
                    return usersWithoutHeadline.Count > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user profiles: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return false;
            }
        }
    }
} 