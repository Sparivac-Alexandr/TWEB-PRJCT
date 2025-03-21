using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.Core;
using taskoreBusinessLogic.Interfaces;
using taskoreDomain.Enteties.User;

namespace taskoreBusinessLogic
{
    public class SessionBL : UserApi, ISesion
    {
        public object UserLogin(UserLoginData data)
        {
            throw new NotImplementedException();
        }
    }
}
