//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRISv2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tappFAQ
    {
        public tappFAQ()
        {
            this.tappActions = new HashSet<tappAction>();
        }
    
        public int Id { get; set; }
        public string officeCode { get; set; }
        public int questionCode { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
    
        public virtual ICollection<tappAction> tappActions { get; set; }
        public virtual tappOffice tappOffice { get; set; }
        public virtual trefQuestion trefQuestion { get; set; }
    }
}