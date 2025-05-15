using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.Core;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreBusinessLogic.Interfaces;
using taskoreDomain.Enteties.User;
using System.Diagnostics;
using taskoreHelpers;

namespace taskoreBusinessLogic
{
    public class SessionBL : UserApi, ISesion
    {
        public object UserLogin(UserLoginSession data)
        {
            return UserLoginAction(data);
        }
        
        public bool CheckEmailExists(string email)
        {
            try
            {
                Debug.WriteLine("Checking if email exists: " + email);
                
                using (var context = new UserContext())
                {
                    bool exists = context.Users.Any(u => u.Email.ToLower() == email.ToLower());
                    Debug.WriteLine("Email check result: " + (exists ? "exists" : "does not exist"));
                    return exists;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR checking email existence: " + ex.Message);
                return false; // Assume email doesn't exist in case of error
            }
        }
        
        public LoginResult GetUserByCookie(string cookieValue)
        {
            try
            {
                Debug.WriteLine("Validating cookie: " + cookieValue);
                
                // Decrypt the cookie value to get the user identifier (email or id)
                string decryptedValue = CoockieGenerator.Validate(cookieValue);
                
                if (string.IsNullOrEmpty(decryptedValue))
                {
                    Debug.WriteLine("Cookie validation failed - invalid or empty value");
                    return null;
                }
                
                // The decrypted value is expected to be the user's email
                using (var context = new UserContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Email.ToLower() == decryptedValue.ToLower());
                    
                    if (user != null)
                    {
                        Debug.WriteLine("Cookie validation successful for user: " + decryptedValue);
                        return new LoginResult
                        {
                            Status = true,
                            UserId = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email
                        };
                    }
                    else
                    {
                        Debug.WriteLine("Cookie validation failed - user not found: " + decryptedValue);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR during cookie validation: " + ex.Message);
                return null;
            }
        }
    }
}
