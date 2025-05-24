using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using taskoreBusinessLogic.BL_Struct;
using taskoreBusinessLogic.DBModel;
using taskoreBusinessLogic.DBModel.Seed;
using taskoreBusinessLogic.Interfaces;

namespace taskore.Controllers
{
    // Clasa de configurare pentru migrări
    public class Configuration : DbMigrationsConfiguration<UserContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
    
    public class DbUpdateController : Controller
    {
        private readonly IDbUpdate _dbUpdateBL;
        
        public DbUpdateController()
        {
            _dbUpdateBL = new DbUpdateBL();
        }
        
        // GET: /DbUpdate/UpdateUserTable
        public ActionResult UpdateUserTable()
        {
            try
            {
                // Pasul 1: Inițializăm baza de date folosind business logic
                bool initSuccess = _dbUpdateBL.InitializeDatabase();
                
                if (!initSuccess)
                {
                    return Content("Database initialization failed.");
                }
                
                // Pasul 2: Actualizăm profilurile utilizatorilor folosind business logic
                bool updateSuccess = _dbUpdateBL.UpdateUserProfiles();
                
                if (updateSuccess)
                {
                    return Content($"Database updated successfully! The user profiles now have all the required fields.");
                }
                else
                {
                    return Content("No users needed updating or update process failed.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating database: {ex.Message}");
                return Content($"Error updating database: {ex.Message}");
            }
        }
    }
} 