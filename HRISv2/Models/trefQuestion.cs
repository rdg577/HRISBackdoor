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
    
    public partial class trefQuestion
    {
        public trefQuestion()
        {
            this.tappFAQs = new HashSet<tappFAQ>();
        }
    
        public int Id { get; set; }
        public string detail { get; set; }
    
        public virtual ICollection<tappFAQ> tappFAQs { get; set; }
    }
}