﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;

namespace HRISv2.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    
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
    
        public DbSet<tappEmpGroup> tappEmpGroups { get; set; }
        public DbSet<tappEmployee> tappEmployees { get; set; }
        public DbSet<tappEmpStatu> tappEmpStatus { get; set; }
        public DbSet<tappOffice> tappOffices { get; set; }
        public DbSet<tappPosition> tappPositions { get; set; }
        public DbSet<tappPreparation> tappPreparations { get; set; }
        public DbSet<tappSalarySched> tappSalaryScheds { get; set; }
        public DbSet<tappSalarySchem> tappSalarySchems { get; set; }
        public DbSet<tappServiceRecord> tappServiceRecords { get; set; }
        public DbSet<tAttEmpArea> tAttEmpAreas { get; set; }
        public DbSet<tAttEmpScheme> tAttEmpSchemes { get; set; }
        public DbSet<tAttScheme> tAttSchemes { get; set; }
        public DbSet<tAttStationArea> tAttStationAreas { get; set; }
        public DbSet<trefEmpGroup> trefEmpGroups { get; set; }
        public DbSet<vDuplicateEICPlantillaPreparation> vDuplicateEICPlantillaPreparations { get; set; }
        public DbSet<vUserProfile> vUserProfiles { get; set; }
        public DbSet<vUserProfileWithService> vUserProfileWithServices { get; set; }
        public DbSet<vUserProfileWithUsername> vUserProfileWithUsernames { get; set; }
        public DbSet<tpassSlipApp> tpassSlipApps { get; set; }
        public DbSet<vpassSlipApp> vpassSlipApps { get; set; }
        public DbSet<vPtlosApp> vPtlosApps { get; set; }
        public DbSet<tptlosApp> tptlosApps { get; set; }
        public DbSet<tAttDailyLog> tAttDailyLogs { get; set; }
        public DbSet<tappDFlexible> tappDFlexibles { get; set; }
        public DbSet<tappDFlexiblesLog> tappDFlexiblesLogs { get; set; }
        public DbSet<tappPositionSub> tappPositionSubs { get; set; }
        public DbSet<tAttDTR> tAttDTRs { get; set; }
        public DbSet<tjustifyApp> tjustifyApps { get; set; }
        public DbSet<vJustifyApp> vJustifyApps { get; set; }
        public DbSet<vAttDTR> vAttDTRs { get; set; }
        public DbSet<tapp212Image> tapp212Image { get; set; }
    
        public virtual ObjectResult<string> JustifyAction(string eIC, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> action, string actionEIC, string ctrlNo, string remarks, Nullable<int> period)
        {
            var eICParameter = eIC != null ?
                new ObjectParameter("EIC", eIC) :
                new ObjectParameter("EIC", typeof(string));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var actionParameter = action.HasValue ?
                new ObjectParameter("action", action) :
                new ObjectParameter("action", typeof(int));
    
            var actionEICParameter = actionEIC != null ?
                new ObjectParameter("actionEIC", actionEIC) :
                new ObjectParameter("actionEIC", typeof(string));
    
            var ctrlNoParameter = ctrlNo != null ?
                new ObjectParameter("CtrlNo", ctrlNo) :
                new ObjectParameter("CtrlNo", typeof(string));
    
            var remarksParameter = remarks != null ?
                new ObjectParameter("remarks", remarks) :
                new ObjectParameter("remarks", typeof(string));
    
            var periodParameter = period.HasValue ?
                new ObjectParameter("period", period) :
                new ObjectParameter("period", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("JustifyAction", eICParameter, startDateParameter, endDateParameter, actionParameter, actionEICParameter, ctrlNoParameter, remarksParameter, periodParameter);
        }
    
        [EdmFunction("HRISEntities", "fnGetEmployeeServiceRecords")]
        public virtual IQueryable<fnGetEmployeeServiceRecords_Result> fnGetEmployeeServiceRecords(string eic)
        {
            var eicParameter = eic != null ?
                new ObjectParameter("eic", eic) :
                new ObjectParameter("eic", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fnGetEmployeeServiceRecords_Result>("[HRISEntities].[fnGetEmployeeServiceRecords](@eic)", eicParameter);
        }
    
        public virtual ObjectResult<string> DtrAction(string dtrID, string txtperiod, Nullable<int> period, Nullable<int> action, string actionEIC, string remarks)
        {
            var dtrIDParameter = dtrID != null ?
                new ObjectParameter("DtrID", dtrID) :
                new ObjectParameter("DtrID", typeof(string));
    
            var txtperiodParameter = txtperiod != null ?
                new ObjectParameter("txtperiod", txtperiod) :
                new ObjectParameter("txtperiod", typeof(string));
    
            var periodParameter = period.HasValue ?
                new ObjectParameter("Period", period) :
                new ObjectParameter("Period", typeof(int));
    
            var actionParameter = action.HasValue ?
                new ObjectParameter("action", action) :
                new ObjectParameter("action", typeof(int));
    
            var actionEICParameter = actionEIC != null ?
                new ObjectParameter("actionEIC", actionEIC) :
                new ObjectParameter("actionEIC", typeof(string));
    
            var remarksParameter = remarks != null ?
                new ObjectParameter("remarks", remarks) :
                new ObjectParameter("remarks", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("DtrAction", dtrIDParameter, txtperiodParameter, periodParameter, actionParameter, actionEICParameter, remarksParameter);
        }
    }
}
