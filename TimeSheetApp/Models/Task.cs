using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class Task
    {
        [Key]
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int SheetId { get; set; }
        public string HoursDone { get; set; }
        [Required]
        public string Description { get; set; }
        public string Notes { get; set; }
        //
        public virtual Sheet Sheet { get; set; }
    }
}