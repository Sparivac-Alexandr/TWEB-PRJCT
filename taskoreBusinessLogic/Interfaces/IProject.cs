using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;

namespace taskoreBusinessLogic.Interfaces
{
    public interface IProject
    {
        bool CreateProject(ProjectDBModel data);
        List<ProjectDBModel> GetUserProjects(int userId);
        List<ProjectDBModel> GetAllProjects();
        bool DeleteProject(int id);
        ProjectDBModel GetProjectById(int id);
        bool UpdateProject(ProjectDBModel project);
    }
} 