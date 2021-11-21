using _10XOneTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10XOneTest.API.Interface
{
    public interface IDataEntryRepository
    {
        List<Partner> GetAllPartners();
        List<FinancialItem> GetAllPurchaseEntries(List<Partner> Partners, List<FinancialItem> FinancialData);

        List<FinancialItem> CreateUpdateFinancialEntry(FinancialItem FinancialEntry, List<FinancialItem> FinancialData);
        List<FinancialItem> DeleteFinancialEntry(int PurchaseId, List<FinancialItem> FinancialData);

        List<PartnerCommission> GetAllPartnersCommission(List<FinancialItem> FinancialData, List<Partner> Partners);
    }
}
