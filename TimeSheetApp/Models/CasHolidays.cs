using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class CasHolidays
    {
        [Key]
        public int CasualHolidayId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
       [DataType(DataType.Date)]
       [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime HolidayDate { get; set; }
        [Required]
        public string Reason { get; set; }
        //
        public virtual UserModel UserModel { get; set; }
    }
}