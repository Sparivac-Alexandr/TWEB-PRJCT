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
        
        // Project Application methods
        bool ApplyForProject(ProjectApplicationDBModel application);
        List<ProjectApplicationDBModel> GetUserApplications(int userId);
        ProjectApplicationDBModel GetProjectApplicationById(int id);
        bool UpdateProjectApplication(ProjectApplicationDBModel application);
        bool DeleteProjectApplication(int id);
        bool HasUserAppliedForProject(int projectId, int userId);
    }
} 