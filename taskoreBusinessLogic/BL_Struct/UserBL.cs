using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreBusinessLogic.Interfaces;

namespace taskoreBusinessLogic.BL_Struct
{
    public class UserBL : IUser
    {
        public string GetUserName(int userId)
        {
            try
            {
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        return $"{user.FirstName} {user.LastName}";
                    }
                }
                
                return "Unknown User";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching user name: {ex.Message}");
                return "Unknown User";
            }
        }
        
        public UDBModel GetUserById(int userId)
        {
            try
            {
                using (var context = new UserContext())
                {
                    return context.Users.FirstOrDefault(u => u.Id == userId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user by ID: {ex.Message}");
                return null;
            }
        }
        
        public bool UpdateUserProfile(UDBModel model)
        {
            try
            {
                using (var context = new UserContext())
                {
                    var existingUser = context.Users.FirstOrDefault(u => u.Id == model.Id);
                    
                    if (existingUser == null)
                    {
                        Debug.WriteLine($"User with ID {model.Id} not found for update");
                        return false;
                    }
                    
                    // Update user fields
                    existingUser.FirstName = model.FirstName;
                    existingUser.LastName = model.LastName;
                    existingUser.Email = model.Email;
                    existingUser.Headline = model.Headline;
                    existingUser.About = model.About;
                    existingUser.Skills = model.Skills;
                    existingUser.Phone = model.Phone;
                    existingUser.Location = model.Location;
                    existingUser.Website = model.Website;
                    existingUser.PreferredProjectTypes = model.PreferredProjectTypes;
                    existingUser.HourlyRate = model.HourlyRate;
                    existingUser.ProjectDuration = model.ProjectDuration;
                    existingUser.CommunicationStyle = model.CommunicationStyle;
                    existingUser.AvailabilityStatus = model.AvailabilityStatus;
                    existingUser.AvailabilityHours = model.AvailabilityHours;
                    existingUser.ProfileImagePath = model.ProfileImagePath;
                    
                    // Save changes
                    int rowsAffected = context.SaveChanges();
                    Debug.WriteLine($"User profile update completed. Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user profile: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
        
        public List<UDBModel> GetAllUsers()
        {
            try
            {
                using (var context = new UserContext())
                {
                    return context.Users.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting all users: {ex.Message}");
                return new List<UDBModel>();
            }
        }
        
        public bool DeleteUser(int userId)
        {
            try
            {
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    
                    if (user == null)
                    {
                        Debug.WriteLine($"User with ID {userId} not found for deletion");
                        return false;
                    }
                    
                    context.Users.Remove(user);
                    int rowsAffected = context.SaveChanges();
                    
                    Debug.WriteLine($"User deletion completed. Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting user: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
        
        public int UpdateUserCompletedProjects(int userId, int completedProjectsCount)
        {
            try
            {
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    
                    if (user == null)
                    {
                        Debug.WriteLine($"User with ID {userId} not found for update");
                        return 0;
                    }
                    
                    user.CompletedProjects = completedProjectsCount;
                    context.SaveChanges();
                    
                    return completedProjectsCount;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating completed projects: {ex.Message}");
                return 0;
            }
        }
        
        public string UploadProfileImage(int userId, HttpPostedFile uploadedFile)
        {
            try
            {
                if (uploadedFile == null)
                {
                    Debug.WriteLine("No file uploaded");
                    return null;
                }
                
                // Validate file type (must be an image)
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    Debug.WriteLine("Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                    return null;
                }
                
                // Generate a unique filename to avoid collisions
                string uniqueFileName = $"user_{userId}_{DateTime.Now.Ticks}{fileExtension}";
                
                // Create the directory if it doesn't exist
                string uploadDirectory = HttpContext.Current.Server.MapPath("~/wwwroot/images/avatars");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }
                
                // Save the file
                string filePath = Path.Combine(uploadDirectory, uniqueFileName);
                uploadedFile.SaveAs(filePath);
                
                // Get the relative path to save in the database
                string relativeFilePath = $"~/wwwroot/images/avatars/{uniqueFileName}";
                
                // Update the user's profile image path in the database
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        user.ProfileImagePath = relativeFilePath;
                        context.SaveChanges();
                    }
                }
                
                return relativeFilePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error uploading profile image: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
} 