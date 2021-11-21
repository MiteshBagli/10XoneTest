using _10XOneTest.API.Interface;
using _10XOneTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10XOneTest.API.Repository
{
    public class DataEntryRepository : IDataEntryRepository
    {
        public List<Partner> GetAllPartners()
        {
            try
            {
                //Adding Partners
                List<Partner> PartnerList = new List<Partner>();
                PartnerList.Add(new Partner(1, "Mitesh", 1, 20));
                PartnerList.Add(new Partner(2, "Ankit", 2, 15.5));
                PartnerList.Add(new Partner(3, "Gaurav", 3, 10));
                PartnerList.Add(new Partner(4, "Darshan", 1, 14));
                PartnerList.Add(new Partner(5, "Akshay", 1, 12.2));
                PartnerList.Add(new Partner(6, "Milind", 1, 13.7));
                PartnerList.Add(new Partner(7, "Jay", 2, 6));
                PartnerList.Add(new Partner(8, "Shankar", 2, 4.4));
                PartnerList.Add(new Partner(9, "Prabin", 2, 7.9));
                PartnerList.Add(new Partner(10, "Ajinkya", 3, 8));
                PartnerList.Add(new Partner(11, "Srajesh", 3,3.9));
                PartnerList.Add(new Partner(12, "Kedar", 3, 13.1));
                return PartnerList;
            }
            catch (Exception ex)
            {
                throw ex;
                //Log Ex
            }
            
        }
        public List<FinancialItem> GetAllPurchaseEntries(List<Partner> PartnerList, List<FinancialItem> PurchaseList)
        {
            try {
                // Joined to get  partner Name 
                var query = from Purchase in PurchaseList
                            join Partner in PartnerList
                                        on Purchase.Partner_Id equals Partner.Partner_Id
                            select new
                            {
                                Purchase.Purchase_Id,
                                Purchase.StrPurchase_Date,
                                Partner.Partner_Name,
                                Partner.Partner_Id,
                                Purchase.Amount
                            };

                //Convert the result To Financial item  List
                PurchaseList = query.AsEnumerable().Select(row => new FinancialItem
                {
                    Purchase_Id = row.Purchase_Id,
                    Partner_Id = row.Partner_Id,
                    Partner_Name = row.Partner_Name,
                    StrPurchase_Date = row.StrPurchase_Date,
                    Amount = row.Amount
                }).ToList();

                return PurchaseList;
            }
            catch (Exception ex)
            {
                throw ex;
                //Log Ex
            }

        }
        public List<PartnerCommission> GetAllPartnersCommission(List<FinancialItem> FinancialData, List<Partner> Partners)
        {
            List<PartnerCommission> PartnerCommList = new List<PartnerCommission>();

            //Partner puchase amount 
            List<FinancialItem> result = FinancialData.GroupBy(l => l.Partner_Id).Select(cl => new FinancialItem
            {
                Partner_Id = cl.First().Partner_Id,
                Amount = cl.Sum(c => c.Amount),
            }).ToList();

            //Total Shopping Amount according to Parent including Parent
            var Totalshopping = result.
            Join(Partners, r => r.Partner_Id, p => p.Partner_Id,
                (r, p) => new { r, p })
                .GroupBy(g => g.p.Parent_Partner_Id)
                .Select(c => new {
                    Partner_Id = c.Key,
                    TotalShoppingAmount = c.Sum(g => g.r.Amount)
                }
            ).ToList();

            //Team Shopping Amount according to Parent excluding Parent
            var Teamshopping = result.
             Join(Partners, r => r.Partner_Id, p => p.Partner_Id,
                 (r, p) => new { r, p })
                 .Where(m => m.p.Partner_Id != m.p.Parent_Partner_Id)
                 .GroupBy(g => g.p.Parent_Partner_Id)
                 .Select(c => new {
                     Partner_Id = c.Key,
                     TeamShoppingAmount = c.Sum(g => g.r.Amount)
                 }
            ).ToList();

            //Partner Commision Amount
            var Comissions = result.Join(Partners, r => r.Partner_Id, p => p.Partner_Id,
            (r, p) => new { r, p })
            .Select(t => new {
                t.p.Partner_Name,
                t.p.Partner_Id,
                t.p.Parent_Partner_Id,
                CommAmount = (t.r.Amount * t.p.Fee_Percent / 100)
            }
            ).ToList();

            //Comission list with all  partners comission calculated on own purchases, and subsequent parent comission.   
            var query = from partner in Partners
                        join Com in Comissions on partner.Partner_Id equals Com.Partner_Id into G from Com in G.DefaultIfEmpty()
                        join TS in Teamshopping on partner.Parent_Partner_Id equals TS.Partner_Id into T from TS in T.DefaultIfEmpty() 
                        join TSP in Totalshopping on partner.Parent_Partner_Id equals TSP.Partner_Id into TP from TSP in TP.DefaultIfEmpty()
                        join PP in Partners on partner.Parent_Partner_Id equals PP.Partner_Id 
                        select new
                        {
                            partner.Partner_Id,
                            partner.Partner_Name,
                            partner.Parent_Partner_Id,
                            partner.Fee_Percent,
                            ParentFeePercent=PP.Fee_Percent,
                            TSA = TS?.TeamShoppingAmount == null ? 0 : TS.TeamShoppingAmount,
                            TSHA = TSP?.TotalShoppingAmount == null ? 0 : TSP.TotalShoppingAmount,
                            CommAmount = Com?.CommAmount == null ?0  : Com.CommAmount,
                            ParentCommAmt = PP.Fee_Percent > partner.Fee_Percent ? (((PP.Fee_Percent - partner.Fee_Percent)/100) * (TSP?.TotalShoppingAmount == null ? 0 : TSP.TotalShoppingAmount)) : 0
                        };

            //Total Parent Comission from Partners 
            var ParentBonus = query.GroupBy(l => l.Parent_Partner_Id).Select(cl => new PartnerCommission
            {
                Parent_Partner_Id = cl.First().Parent_Partner_Id,
               ParentComAmount = cl.Sum(c => c.ParentCommAmt),
            }).ToList();

            //Final List  with full comission  calculations
            var FinalComList = from partnercom in query
                   join PB in ParentBonus on partnercom.Partner_Id equals PB.Parent_Partner_Id into PC
                   from PB in PC.DefaultIfEmpty()
                   select new
                   {
                        partnercom.Partner_Id,
                        partnercom.Partner_Name,
                        partnercom.Parent_Partner_Id,
                       TeamShoppingAmount=partnercom.TSA,
                       TotalShoppingAmount=partnercom.TSHA,
                       TotalCommissionAmount = partnercom.CommAmount+ (PB?.ParentComAmount == null ? 0 : PB.ParentComAmount)
                   };

            PartnerCommList = FinalComList.AsEnumerable().Select(row => new PartnerCommission
            {
                Partner_Id = row.Partner_Id,
                Partner_Name = row.Partner_Name,
                Parent_Partner_Id = row.Parent_Partner_Id,
                TeamShoppingAmount = Math.Round(row.TeamShoppingAmount,2),
                TotalShoppingAmount = Math.Round(row.TotalShoppingAmount,2),
                TotalCommissionAmount =Math.Round( row.TotalCommissionAmount,2)
            }).ToList();

            return PartnerCommList;
        }
        public List<FinancialItem> CreateUpdateFinancialEntry(FinancialItem FinancialEntry, List<FinancialItem> _listItem)
        {
            try
            {
                FinancialEntry.Amount = Math.Round(FinancialEntry.Amount,2);
                if (FinancialEntry.Purchase_Id == 0)
                {
                    //Add Entry 
                    int MaxPurchaseId = 0;
                    if (_listItem.Count > 0)
                    {
                        MaxPurchaseId = Int16.Parse(_listItem.Max(y => y.Purchase_Id).ToString());
                    }
                    FinancialEntry.Purchase_Id = MaxPurchaseId + 1;
                    _listItem.Add(new FinancialItem(FinancialEntry.Purchase_Id, FinancialEntry.Partner_Id, FinancialEntry.Purchase_Date, FinancialEntry.Amount, FinancialEntry.StrPurchase_Date));
                }
                else
                {
                    //update Entry
                    (from u in _listItem where u.Purchase_Id == FinancialEntry.Purchase_Id select u).ToList()
                        .ForEach(u =>
                        {
                            u.Purchase_Date = FinancialEntry.Purchase_Date;
                            u.Partner_Id = FinancialEntry.Partner_Id;
                            u.StrPurchase_Date = FinancialEntry.Purchase_Date.ToString("MM/dd/yyyy");
                            u.Amount = FinancialEntry.Amount;
                        });
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //Log Ex
            }
            return _listItem;
        }
        public List<FinancialItem> DeleteFinancialEntry(int PurchaseId, List<FinancialItem> _listItem)
        {
            try
            {
                _listItem.Remove(_listItem.Single(s => s.Purchase_Id == PurchaseId));
                return _listItem;
            }
            catch (Exception ex)
            {
                throw ex;
                //Log Ex
            }
        }
    }
}