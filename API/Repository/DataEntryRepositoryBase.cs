namespace _10XOneTest.API.Repository
{
    public class DataEntryRepositoryBase
    {

        public List<FinancialItem> GetAllPurchaseEntries(List<Partner> PartnerList, List<FinancialItem> PurchaseList)
        {
            try
            {
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
    }
}