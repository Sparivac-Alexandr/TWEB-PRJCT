using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;

namespace taskore.Controllers
{
    [RoutePrefix("api/user")]
    public class UserApiController : ApiController
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["taskore"].ConnectionString;

        [HttpPost]
        [Route("upload-profile-image")]
        public IHttpActionResult UploadProfileImage()
        {
            try
            {
                // Check if the user is authenticated
                if (HttpContext.Current.Session["UserId"] == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User not authenticated"));
                }

                // Get the current user ID
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserId"]);

                // Check if a file was uploaded
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count == 0)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No file uploaded"));
                }

                // Get the uploaded file
                var uploadedFile = httpRequest.Files[0];
                
                // Validate file type (must be an image)
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid file type. Only JPG, PNG, and GIF files are allowed."));
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
                
                // Instead of updating the database directly, just store the image path in session
                // We'll need to modify the Users table to include profile_image_url column later
                HttpContext.Current.Session["ProfileImageUrl"] = relativeFilePath;
                
                // Return success response with the image URL
                return Ok(new { 
                    success = true, 
                    message = "Profile image uploaded successfully", 
                    imageUrl = relativeFilePath 
                });
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error uploading profile image: {ex.Message}"));
            }
        }
    }
} 