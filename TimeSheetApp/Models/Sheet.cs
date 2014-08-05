using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class Sheet
    {
        [Key]
        public int SheetId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string ApprovedDays { get; set; }
        //
        public virtual UserModel UserModel { get; set; }
        public virtual IEnumerable<TimeSheetApp.Models.Task> Task { get; set; }
    }
}