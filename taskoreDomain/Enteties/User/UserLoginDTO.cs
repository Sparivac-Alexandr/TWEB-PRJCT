using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreDomain.Enteties.User
{
   public class UserLoginDTO
    {
        //Auth
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserIp { get; set; }

    }
}
