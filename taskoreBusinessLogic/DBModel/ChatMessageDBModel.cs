using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taskoreBusinessLogic.DBModel
{
    [Table("ChatMessages")]
    public class ChatMessageDBModel
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("sender_id")]
        public int SenderId { get; set; }

        [Required]
        [Column("receiver_id")]
        public int ReceiverId { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Column("sent_at")]
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
} 