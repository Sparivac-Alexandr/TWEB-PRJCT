using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreBusinessLogic.Interfaces;

namespace taskoreBusinessLogic.BL_Struct
{
    public class ProjectBL : IProject
    {
        public bool CreateProject(ProjectDBModel data)
        {
            try
            {
                Debug.WriteLine("Starting project creation process for title: " + data.Title);

                var context = new ProjectContext();
                Debug.WriteLine("Database connection established");

                context.Projects.Add(data);
                Debug.WriteLine("Added project to context");

                int rowsAffected = context.SaveChanges();
                Debug.WriteLine("SaveChanges completed. Rows affected: " + rowsAffected);
                Debug.WriteLine("New project ID assigned by database: " + data.Id);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR in CreateProject: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return false;
            }
        }

        public List<ProjectDBModel> GetAllProjects()
        {
            try
            {
                Debug.WriteLine("Getting all projects");
                
                using (var context = new ProjectContext())
                {
                    return context.Projects
                        .OrderByDescending(p => p.CreatedAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR in GetAllProjects: " + ex.Message);
                return new List<ProjectDBModel>();
            }
        }

        public List<ProjectDBModel> GetUserProjects(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting projects for user ID: {userId}");
                
                using (var context = new ProjectContext())
                {
                    return context.Projects
                        .Where(p => p.UserId == userId)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetUserProjects for user {userId}: " + ex.Message);
                return new List<ProjectDBModel>();
            }
        }
    }
} 