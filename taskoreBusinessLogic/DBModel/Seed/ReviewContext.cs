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
    public class ReviewContext : DbContext
    {
        public virtual DbSet<ReviewDBModel> Reviews { get; set; }
        
        public ReviewContext() : base("name=taskore")
        {
            // Enable database creation if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<ReviewContext>());
            
            // Enable logging database operations
            this.Database.Log = s => Debug.WriteLine(s);
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the ReviewDBModel
            modelBuilder.Entity<ReviewDBModel>()
                .ToTable("Reviews")
                .HasKey(e => e.Id);
                
            modelBuilder.Entity<ReviewDBModel>()
                .Property(e => e.Content)
                .IsRequired();
                
            modelBuilder.Entity<ReviewDBModel>()
                .Property(e => e.SenderId)
                .IsRequired();
                
            modelBuilder.Entity<ReviewDBModel>()
                .Property(e => e.ReceiverId)
                .IsRequired();
                
            base.OnModelCreating(modelBuilder);
        }
    }
} 