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

        User = 100,
        User1 = 101,
        User2 = 102,

        Moderator = 200,
        Vip = 300,
        Administrator = 400,

    }
}
