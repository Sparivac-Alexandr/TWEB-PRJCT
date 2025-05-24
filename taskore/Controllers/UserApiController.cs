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
using taskoreBusinessLogic;
using taskoreBusinessLogic.Interfaces;

namespace taskore.Controllers
{
    [RoutePrefix("api/user")]
    public class UserApiController : ApiController
    {
        private readonly IUser _userService;
        
        public UserApiController()
        {
            var bl = new BusinessLogic();
            _userService = bl.GetUserBL();
        }

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
                
                // Use the business logic service to upload the profile image
                string relativeFilePath = _userService.UploadProfileImage(userId, uploadedFile);
                
                if (string.IsNullOrEmpty(relativeFilePath))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed to upload profile image"));
                }
                
                // Store the image path in session
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