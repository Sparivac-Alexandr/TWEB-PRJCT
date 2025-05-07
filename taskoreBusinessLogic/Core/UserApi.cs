using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreDomain.Enteties.User;

namespace taskoreBusinessLogic.Core
{
   public class UserApi
    {
        //Auth
        public string UserAuthLogicAction(UserLoginDTO data)
        {
            return "token-key";
        }

        public LoginResult UserLoginAction(UserLoginSession data)
        {
            try
            {
                Debug.WriteLine("Attempting login for: " + data.Email);
                
                using (var context = new UserContext())
                {
                    // Verificăm email-ul și parola
                    var user = context.Users.FirstOrDefault(u => 
                        u.Email.ToLower() == data.Email.ToLower() && 
                        u.Password == data.Password);
                    
                    if (user != null)
                    {
                        Debug.WriteLine("Login successful for: " + data.Email);
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
                        Debug.WriteLine("Login failed for: " + data.Email);
                        return new LoginResult 
                        { 
                            Status = false, 
                            StatusMsg = new Exception("Invalid email or password") 
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR during login: " + ex.Message);
                return new LoginResult 
                { 
                    Status = false, 
                    StatusMsg = ex 
                };
            }
        }
    }
}
