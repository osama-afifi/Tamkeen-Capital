﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db8bc58a7c5b7347cbb656a21900e60022Entities : DbContext
    {
        public db8bc58a7c5b7347cbb656a21900e60022Entities()
            : base("name=db8bc58a7c5b7347cbb656a21900e60022Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CasualHoliday> CasualHolidays { get; set; }
        public DbSet<ScheduledHoliday> ScheduledHolidays { get; set; }
        public DbSet<Sheet> Sheets { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}