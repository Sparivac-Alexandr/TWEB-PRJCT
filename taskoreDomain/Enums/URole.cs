using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreDomain.Enums
{
    public enum URole
    {
        None = 0,

        Banned = 1,
        FraudlesUser = 2,
        Deleted = 3,

        Moderator = 200,
        Vip = 300,
        Administrator = 400,
            
        Client = 50,
        Freelancer = 51,
        User = 401,
    }
}
