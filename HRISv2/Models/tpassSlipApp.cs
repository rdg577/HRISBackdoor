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
    
    public partial class tpassSlipApp
    {
        public int recNo { get; set; }
        public string controlNo { get; set; }
        public string EIC { get; set; }
        public Nullable<System.DateTime> timeOut { get; set; }
        public Nullable<System.DateTime> timeIn { get; set; }
        public string destination { get; set; }
        public string purpose { get; set; }
        public Nullable<int> isOfficial { get; set; }
        public Nullable<int> statusID { get; set; }
        public string apprvEIC { get; set; }
        public string SchemeCode { get; set; }
        public string isOfficialprev { get; set; }
    }
}
