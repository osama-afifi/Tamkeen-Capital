using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TimeSheetApp.Models
{
    public class DayViewModel
    {
        public TimeSheetApp.Models.Task dayTask { get; set; }
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "{0} must be a Number.")]
        public int hourValue { get; set; }
    }
    public class DayViewModelList
    {
       public List<DayViewModel> DayTasks { get; set; }
       public int day { get; set; }
    }
}