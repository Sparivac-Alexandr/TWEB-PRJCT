using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreBusinessLogic.DBModel;
using System.Diagnostics;

namespace taskoreBusinessLogic.DBModel.Seed
{
    public class CustomDatabaseInitializer : DropCreateDatabaseIfModelChanges<UserContext>
    {
        protected override void Seed(UserContext context)
        {
            // Seed data if needed
            base.Seed(context);
        }
    }

    public class UserContext : DbContext
    {
        public virtual DbSet<UDBModel> Users { get; set; }
        
        public UserContext() : base("name=taskore")
        {
            // Folosim DropCreateDatabaseIfModelChanges pentru a recrea baza de date când modelul se schimbă
            Database.SetInitializer(new CustomDatabaseInitializer());
            
            // Enable change detection for entity updates
            Configuration.AutoDetectChangesEnabled = true;
            
            // Enable logging database operations
            this.Database.Log = s => Debug.WriteLine(s);
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the UDBModel
            modelBuilder.Entity<UDBModel>()
                .ToTable("Users")
                .HasKey(e => e.Id);
                
            modelBuilder.Entity<UDBModel>()
                .Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            modelBuilder.Entity<UDBModel>()
                .Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
