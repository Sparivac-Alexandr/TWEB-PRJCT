using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskoreBusinessLogic.DBModel
{
    [Table("Reviews")]
    public class ReviewDBModel
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Required]
        [Column("sender_id")]
        public int SenderId { get; set; }

        [Required]
        [Column("receiver_id")]
        public int ReceiverId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("rating")]
        public int? Rating { get; set; }
    }
} 