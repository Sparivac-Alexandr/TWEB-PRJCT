using System;
using System.ComponentModel.DataAnnotations;

namespace taskoreBusinessLogic.DBModel
{
    public class ProjectApplicationDBModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Status { get; set; } = "In Progress";
        
        [Required]
        public string Client { get; set; }
        
        [Required]
        public string Freelancer { get; set; }
        
        [Required]
        public string Budget { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public int Progress { get; set; } = 0;
        
        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public int FreelancerId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
} 