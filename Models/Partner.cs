using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10XOneTest.Models
{
    public class Partner
    {
        public Partner()
        {
            
        }
        public Partner(int _PartnerId ,string _PartnerName , int _ParentPartnerId , double _FeePercent) {
            Partner_Id = _PartnerId;
            Partner_Name = _PartnerName;
            Parent_Partner_Id = _ParentPartnerId;
            Fee_Percent = _FeePercent;
        }
        public int Partner_Id { get; set; }
        public string Partner_Name { get; set; }
        public int Parent_Partner_Id { get; set; }
        public double Fee_Percent { get; set; }
    }
}