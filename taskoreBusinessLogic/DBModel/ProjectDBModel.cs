using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreBusinessLogic.DBModel
{
    [Table("Projects")]
    public class ProjectDBModel
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("category")]
        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        [Column("budget")]
        [StringLength(100)]
        public string Budget { get; set; }

        [Required]
        [Column("deadline")]
        [StringLength(100)]
        public string Deadline { get; set; }

        [Required]
        [Column("required_skills")]
        public string RequiredSkills { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
} 