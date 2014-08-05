using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TimeSheetApp.Models
{
    public class TableViewModel
    {
       public Models.Sheet Sheet {get; set;}
       public string Holiday { get; set; }
       public string Approved { get; set; }
    }
}