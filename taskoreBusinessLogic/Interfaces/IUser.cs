using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;

namespace taskoreBusinessLogic.Interfaces
{
    public interface IUser
    {
        string GetUserName(int userId);
        UDBModel GetUserById(int userId);
        bool UpdateUserProfile(UDBModel model);
        List<UDBModel> GetAllUsers();
        bool DeleteUser(int userId);
        int UpdateUserCompletedProjects(int userId, int completedProjectsCount);
    }
} 