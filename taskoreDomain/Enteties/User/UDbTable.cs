using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskoreDomain.Enums;

namespace taskoreDomain.Enteties.User
{
    public class UDbTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(30 ,MinimumLength = 5, ErrorMessage ="Username cannot be longer then 30 characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(30, MinimumLength = 8 ,ErrorMessage = "Password cannot be shorter then 8 characters.")]
        public string Password { get; set; }


        [Required]
        [Display(Name = "Email Address")]
        [StringLength(30)]

        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastLogin { get; set; }

        [StringLength(30)]
        public string LastIp { get; set; }

        public URole Level { get; set; }

        [StringLength(255)]
        public string ProfileImageUrl { get; set; }

    }
}
