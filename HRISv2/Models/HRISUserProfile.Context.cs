﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRISv2.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HRISEntities : DbContext
    {
        public HRISEntities()
            : base("name=HRISEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tappEmpGroup> tappEmpGroup { get; set; }
        public DbSet<tAttEmpArea> tAttEmpArea { get; set; }
        public DbSet<tAttEmpScheme> tAttEmpScheme { get; set; }
        public DbSet<tAttScheme> tAttScheme { get; set; }
        public DbSet<tAttStationArea> tAttStationArea { get; set; }
        public DbSet<trefEmpGroup> trefEmpGroup { get; set; }
        public DbSet<vUserProfile> vUserProfile { get; set; }
        public DbSet<vUserProfileWithUsername> vUserProfileWithUsername { get; set; }
        public DbSet<vUserProfileWithServices> vUserProfileWithServices { get; set; }
    }
}
