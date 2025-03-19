using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.Interfaces;

namespace taskoreBusinessLogic
{
    public class BusinessLogic
    {
      public ISesion GetSesionBL()
        {
            return new SessionBL();
        }
    }
}
