using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class SchHolidays
    {
        [Key]
        public int ScheduledHolidayId { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime HolidayStart { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime HolidayEnd { get; set; }
        [Required]
        public string Reason { get; set; }

        public virtual UserModel UserModel { get; set; }
        
    }
}