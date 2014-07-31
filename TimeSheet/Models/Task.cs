using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Models
{
    public class Task
    {
        [Key]
        public int ID { get; set; }
        public int Employee { get; set; }
        public string Name { get; set; }
        public string Week{ get; set; }
    }
}