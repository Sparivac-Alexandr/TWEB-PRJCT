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
    }
}
