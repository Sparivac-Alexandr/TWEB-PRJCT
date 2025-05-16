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
        
        public bool DeleteProject(int id)
        {
            try
            {
                Debug.WriteLine($"Deleting project with ID: {id}");
                
                using (var context = new ProjectContext())
                {
                    var project = context.Projects.Find(id);
                    if (project != null)
                    {
                        context.Projects.Remove(project);
                        int rowsAffected = context.SaveChanges();
                        Debug.WriteLine($"Project deletion completed. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                    Debug.WriteLine($"Project with ID {id} not found for deletion");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in DeleteProject for ID {id}: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return false;
            }
        }
        
        public ProjectDBModel GetProjectById(int id)
        {
            try
            {
                Debug.WriteLine($"Getting project with ID: {id}");
                
                using (var context = new ProjectContext())
                {
                    return context.Projects.FirstOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in GetProjectById for ID {id}: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return null;
            }
        }
        
        public bool UpdateProject(ProjectDBModel project)
        {
            try
            {
                Debug.WriteLine($"Updating project with ID: {project.Id}");
                
                using (var context = new ProjectContext())
                {
                    var existingProject = context.Projects.Find(project.Id);
                    
                    if (existingProject == null)
                    {
                        Debug.WriteLine($"Project with ID {project.Id} not found for update");
                        return false;
                    }
                    
                    // Update the fields
                    existingProject.Title = project.Title;
                    existingProject.Description = project.Description;
                    existingProject.Category = project.Category;
                    existingProject.Budget = project.Budget;
                    existingProject.Deadline = project.Deadline;
                    existingProject.RequiredSkills = project.RequiredSkills;
                    
                    // Save changes
                    int rowsAffected = context.SaveChanges();
                    Debug.WriteLine($"Project update completed. Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in UpdateProject for ID {project.Id}: " + ex.Message);
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