using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreDomain.Enteties.User;

namespace taskoreBusinessLogic.Interfaces
{
    public interface ISesion
    {
      object UserLogin(UserLoginSession data);
    }

}
