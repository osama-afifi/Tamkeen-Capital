using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class UserModel
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email Address: ")]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "First Name: ")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "Last Name: ")]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [StringLength(20, MinimumLength = 10)]
        [Display(Name = "Mobile: ")]
        public string Mobile { get; set; } // Regex to check if valid

        [Required]
        public bool Admin { get; set; }


        public DateTime? LastLogin { get; set; }


        public virtual ICollection<Sheet> Sheet { get; set; }
        public virtual ICollection<CasHolidays> CasualHoliday { get; set; } 
    }
}