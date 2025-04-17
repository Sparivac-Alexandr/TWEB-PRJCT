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
    }
}
