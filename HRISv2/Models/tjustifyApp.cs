//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tjustifyApp
    {
        public int recNo { get; set; }
        public string CtrlNo { get; set; }
        public string controlNo { get; set; }
        public string approveEIC { get; set; }
        public string EIC { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<int> logType { get; set; }
        public string time { get; set; }
        public string reason { get; set; }
        public Nullable<int> statusID { get; set; }
        public Nullable<int> period { get; set; }
        public Nullable<System.DateTime> dtStamp { get; set; }
        public Nullable<System.DateTime> logTime { get; set; }
        public string userEIC { get; set; }
    }
}