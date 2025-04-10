using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.Core;
using taskoreBusinessLogic.Interfaces;
using taskoreDomain.Enteties.User;

namespace taskoreBusinessLogic.BL_Struct
{
   public class AuthBL : UserApi, IAuth
    {
        public string UserAuthLogic(UserLoginDTO data)
        {
            return UserAuthLogicAction(data);
        }

        public bool UserRegisterLogic(UserRegisterDTO data)
        {
            throw new NotImplementedException();
        }
    }
}
