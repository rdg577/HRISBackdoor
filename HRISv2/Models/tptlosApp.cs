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
    
    public partial class tptlosApp
    {
        public int recNo { get; set; }
        public string controlNo { get; set; }
        public Nullable<System.DateTime> dateApplied { get; set; }
        public string proceedTo { get; set; }
        public string purpose { get; set; }
        public string nameText { get; set; }
        public Nullable<System.DateTime> departure { get; set; }
        public Nullable<System.DateTime> arrival { get; set; }
        public Nullable<System.DateTime> returnOfficial { get; set; }
        public string remarks { get; set; }
        public string recommendEIC { get; set; }
        public Nullable<int> recommendStatus { get; set; }
        public string approveEIC { get; set; }
        public string approvePos { get; set; }
        public Nullable<System.DateTime> approvedDate { get; set; }
        public Nullable<int> approveStatus { get; set; }
        public Nullable<int> isPrintable { get; set; }
        public string facilitatorEIC { get; set; }
        public string userEIC { get; set; }
        public string SchemeCode { get; set; }
        public string GroupCode { get; set; }
        public Nullable<int> Tag { get; set; }
    }
}