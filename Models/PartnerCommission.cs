using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10XOneTest.Models
{
    public class PartnerCommission
    {
  
        public int Partner_Id { get; set; }
        public int Parent_Partner_Id { get; set; }
        public string Partner_Name { get; set; }
        public double TeamShoppingAmount { get; set; }
        public double TotalShoppingAmount { get; set; }
        public double TotalCommissionAmount { get; set; }
        public double ParentComAmount { get; set; }
        public double PartnerFeePercent{ get; set; }
    }
}