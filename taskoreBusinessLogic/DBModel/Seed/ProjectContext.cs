using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;

namespace taskoreBusinessLogic.DBModel.Seed
{
    public class ProjectContext : DbContext
    {
        public virtual DbSet<ProjectDBModel> Projects { get; set; }
        
        public ProjectContext() : base("name=taskore")
        {
            // Enable database creation if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<ProjectContext>());
            
            // Enable logging database operations
            this.Database.Log = s => Debug.WriteLine(s);
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the ProjectDBModel
            modelBuilder.Entity<ProjectDBModel>()
                .ToTable("Projects")
                .HasKey(e => e.Id);
                
            modelBuilder.Entity<ProjectDBModel>()
                .Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);
                
            modelBuilder.Entity<ProjectDBModel>()
                .Property(e => e.Description)
                .IsRequired();
                
            modelBuilder.Entity<ProjectDBModel>()
                .Property(e => e.UserId)
                .IsRequired();
                
            base.OnModelCreating(modelBuilder);
        }
    }
} 