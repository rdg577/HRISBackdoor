//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace HRISv2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tappPreparation
    {
        public int recNo { get; set; }
        public string staffingCode { get; set; }
        public string AIC { get; set; }
        public string userEIC { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> dateEncoded { get; set; }
    }
}
