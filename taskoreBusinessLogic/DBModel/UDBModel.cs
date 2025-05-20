using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreBusinessLogic.DBModel
{
    [Table("Users")]
    public class UDBModel
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        [Required]
        [Column("firstname")]
        [StringLength(100)]
        public string FirstName {get; set;}
        [Required]
        [Column("lastname")]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [Column("password")]
        [StringLength(255)]
        public string Password { get; set; }
        [Required]
        [Column("email")]
        [StringLength(255)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        // Câmpuri adiționale pentru profilul utilizatorului
        [Column("headline")]
        [StringLength(255)]
        public string Headline { get; set; }

        [Column("about")]
        public string About { get; set; }

        [Column("skills")]
        public string Skills { get; set; }

        [Column("phone")]
        [StringLength(50)]
        public string Phone { get; set; }

        [Column("location")]
        [StringLength(255)]
        public string Location { get; set; }

        [Column("website")]
        [StringLength(255)]
        public string Website { get; set; }

        [Column("preferred_project_types")]
        [StringLength(255)]
        public string PreferredProjectTypes { get; set; }

        [Column("hourly_rate")]
        [StringLength(50)]
        public string HourlyRate { get; set; }

        [Column("project_duration")]
        [StringLength(100)]
        public string ProjectDuration { get; set; }

        [Column("communication_style")]
        [StringLength(255)]
        public string CommunicationStyle { get; set; }

        [Column("availability_status")]
        [StringLength(50)]
        public string AvailabilityStatus { get; set; }

        [Column("availability_hours")]
        [StringLength(50)]
        public string AvailabilityHours { get; set; }

        [Column("rating")]
        public double? Rating { get; set; }

        [Column("rating_count")]
        public int? RatingCount { get; set; }

        [Column("completed_projects")]
        public int? CompletedProjects { get; set; }

        [Column("current_projects")]
        public int? CurrentProjects { get; set; }

        [Column("on_time_percentage")]
        public double? OnTimePercentage { get; set; }

        [Column("completion_rate")]
        public double? CompletionRate { get; set; }

        [Column("response_rate")]
        public double? ResponseRate { get; set; }

        [Column("client_satisfaction")]
        public double? ClientSatisfaction { get; set; }

        [Column("project_efficiency")]
        public double? ProjectEfficiency { get; set; }

        [Column("last_active_at")]
        public DateTime? LastActiveAt { get; set; }

        // Not mapped to database, used for form submission
        [NotMapped]
        public string FullName { get; set; }
    }
}
