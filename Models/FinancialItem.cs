using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10XOneTest.Models
{
    public class FinancialItem
    {
        public FinancialItem()
        {

        }

        public FinancialItem(int _purchaseId , int _partnerId , DateTime _purchaseDate, double amount,string _strPurchaseDate)
        {
            Purchase_Id = _purchaseId;
            Partner_Id = _partnerId;
            Purchase_Date = _purchaseDate;
            Amount = amount;
            StrPurchase_Date = _strPurchaseDate;
        }

        public int Purchase_Id { get; set; }
        public int Partner_Id { get; set; }
        public string Partner_Name { get; set; }
        public DateTime Purchase_Date { get; set; }
        public string StrPurchase_Date { get; set; } // To display  in Grid
        public double Amount { get; set; }

    }
}