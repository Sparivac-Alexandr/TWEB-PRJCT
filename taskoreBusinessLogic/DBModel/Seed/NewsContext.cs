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
    public class NewsContext : DbContext
    {
        public virtual DbSet<NewsDBModel> News { get; set; }
        
        public NewsContext() : base("name=taskore")
        {
            // Enable database creation if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<NewsContext>());
            
            // Enable logging database operations
            this.Database.Log = s => Debug.WriteLine(s);
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the NewsDBModel
            modelBuilder.Entity<NewsDBModel>()
                .ToTable("News")
                .HasKey(e => e.Id);
                
            modelBuilder.Entity<NewsDBModel>()
                .Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);
                
            modelBuilder.Entity<NewsDBModel>()
                .Property(e => e.Content)
                .IsRequired();
                
            modelBuilder.Entity<NewsDBModel>()
                .Property(e => e.Priority)
                .IsRequired()
                .HasMaxLength(50);
                
            modelBuilder.Entity<NewsDBModel>()
                .Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);
                
            modelBuilder.Entity<NewsDBModel>()
                .Property(e => e.Author)
                .IsRequired()
                .HasMaxLength(100);
                
            modelBuilder.Entity<NewsDBModel>()
                .Property(e => e.PublishDate)
                .IsRequired();
                
            base.OnModelCreating(modelBuilder);
        }
    }
} 