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
        [Display(Name = "username")]
        [StringLength(30 ,MinimumLength = 5)]
        public string Name { get; set; }

        public string Phone {  get; set; }

        public string Address { get; set; }

        public string UserIP  { get; set; }

        [Display (Name  = "reg_dt")]
        public DateTime RegistrationDateTime  { get; set; }

        [Display (Name = "Login_dt")]

        public URole Level { get; set; }



    }
}
