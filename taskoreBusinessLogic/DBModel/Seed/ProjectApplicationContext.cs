using System;
using System.Data.Entity;
using System.Diagnostics;
using taskoreBusinessLogic.DBModel;

namespace taskoreBusinessLogic.DBModel.Seed
{
    public class ProjectApplicationContext : DbContext
    {
        public virtual DbSet<ProjectApplicationDBModel> ProjectApplications { get; set; }
        
        public ProjectApplicationContext() : base("name=taskore")
        {
            // Enable database creation if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<ProjectApplicationContext>());
            
            // Enable logging database operations
            this.Database.Log = s => Debug.WriteLine(s);
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the ProjectApplicationDBModel
            modelBuilder.Entity<ProjectApplicationDBModel>()
                .ToTable("ProjectApplications")
                .HasKey(e => e.Id);
                
            modelBuilder.Entity<ProjectApplicationDBModel>()
                .Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);
                
            modelBuilder.Entity<ProjectApplicationDBModel>()
                .Property(e => e.Client)
                .IsRequired();
                
            modelBuilder.Entity<ProjectApplicationDBModel>()
                .Property(e => e.Freelancer)
                .IsRequired();
                
            modelBuilder.Entity<ProjectApplicationDBModel>()
                .Property(e => e.Budget)
                .IsRequired();
                
            modelBuilder.Entity<ProjectApplicationDBModel>()
                .Property(e => e.Status)
                .IsRequired();
                
            base.OnModelCreating(modelBuilder);
        }
    }
} 