//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimeSheetApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Task
    {
        public int TaskId { get; set; }
        public int SheetId { get; set; }
        public string TaskDescription { get; set; }
        public string TaskNotes { get; set; }
        public string HoursWorked { get; set; }
    
        public virtual Sheet Sheet { get; set; }
    }
}