using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace taskoreDomain.Enteties.User
{
    public class UserLoginData
    {
        //Session
        public string Password { get; set; }
        public string Email { get; set; }
        public string LoginIp { get; set; }
        public DateTime LoginDataTime { get; set;}

    }
}

public class LoginResult
{
    public bool Status { get; internal set; }
    public Exception StatusMsg { get; internal set; }
}
