﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreDomain.Enteties.User
{
   public class UserRegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
       
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }
    }
}
