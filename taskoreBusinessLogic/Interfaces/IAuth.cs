using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreDomain.Enteties.User;

namespace taskoreBusinessLogic.Interfaces
{
    public interface IAuth
    {
        string UserAuthLogic(UserLoginDTO data);
        bool UserRegisterLogic(UserRegisterDTO data);
        bool InitiatePasswordReset(string email);
        bool ValidateResetCode(string email, string resetCode);
        bool ResetPassword(string email, string newPassword);
    }
}
