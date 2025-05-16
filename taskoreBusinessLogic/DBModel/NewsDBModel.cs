using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreBusinessLogic.DBModel
{
    [Table("News")]
    public class NewsDBModel
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
        [Column("content")]
        public string Content { get; set; }

        [Required]
        [Column("category")]
        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        [Column("author")]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [Column("priority")]
        [StringLength(50)]
        public string Priority { get; set; }

        [Required]
        [Column("publish_date")]
        public DateTime PublishDate { get; set; }

        [Column("image_url")]
        [StringLength(255)]
        public string ImageUrl { get; set; }
    }
} 