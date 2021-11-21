using _10XOneTest.API.Interface;
using _10XOneTest.API.Repository;
using _10XOneTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _10XOneTest.Controllers
{

    public class DataEntryController : Controller
    {
        // GET: DataEntry
        IDataEntryRepository _dataEntry;
        List<FinancialItem> _listItem;


        public DataEntryController()
        {
            _dataEntry = new DataEntryRepository();
            _listItem = new List<FinancialItem>();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAllPartners()
        {
            JsonResult jResult = null;
            try
            {
                var list = _dataEntry.GetAllPartners();
                var jlist = list.ToList();
                Session["Partners"] = jlist;
                jResult = Json(jlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Log Ex
            }
            return jResult;

        }

        [HttpGet]
        public ActionResult GetAllPurchaseEntries()
        {
            JsonResult jResult = null;
            List<FinancialItem> PurchaseList = new List<FinancialItem>();
            List<Partner> PartnerList = new List<Partner>();
            try
            {
                if (Session["FinancialItem"] != null)
                {
                    PurchaseList = Session["FinancialItem"] as List<FinancialItem>;
                    PartnerList = Session["Partners"] as List<Partner>;
                    PurchaseList = _dataEntry.GetAllPurchaseEntries(PartnerList, PurchaseList);
                }
            }
            catch (Exception ex)
            {
                //Log Ex
            }

            jResult = Json(PurchaseList, JsonRequestBehavior.AllowGet);
            return jResult;
        }

        [HttpPost]

        public ActionResult CreateUpdateFinancialEntry(FinancialItem model)
        {
            bool isSuccess;
            try
            {
                model.StrPurchase_Date = model.Purchase_Date.ToString("MM/dd/yyyy");
                if (Session["FinancialItem"] != null)
                {
                    _listItem = Session["FinancialItem"] as List<FinancialItem>;
                }
                _listItem = _dataEntry.CreateUpdateFinancialEntry(model, _listItem);

                Session["FinancialItem"] = _listItem;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            if (!isSuccess)
                return Json(new { success = false, message = "Something went wrong! please Contact to Administrator" });
            else
                return Json(new { success = true, message = "Financial Item added successfully", Entries = _listItem });
        }

        [HttpPost]

        public ActionResult DeleteFinancialEntry(int PurchaseId)
        {
            bool isSuccess;
            try
            {
                _listItem = Session["FinancialItem"] as List<FinancialItem>;
                _listItem = _dataEntry.DeleteFinancialEntry(PurchaseId, _listItem);
                Session["FinancialItem"] = _listItem;
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            if (!isSuccess)
                return Json(new { success = false, message = "Something went wrong with deleting! please Contact to Administrator" });
            else
                return Json(new { success = true, message = "Financial Item Removed successfully", Entries = _listItem });
        }



        [HttpGet]
        public ActionResult GetAllPartnersCommission()
        {
            bool isSuccess = false;
            List<PartnerCommission> PartnerCom = new List<PartnerCommission>();
            try
            {

                List<FinancialItem> PurchaseList = Session["FinancialItem"] != null ? Session["FinancialItem"] as List<FinancialItem> : new List<FinancialItem>();
                List<Partner> PartnerList = Session["Partners"] as List<Partner>;
                PartnerCom = _dataEntry.GetAllPartnersCommission(PurchaseList, PartnerList);
                isSuccess = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            if (!isSuccess)
                return Json(new { success = false, message = "Something went wrong! please Contact to Administrator" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = true, message = "Fee calculation completed", Comissions = PartnerCom }, JsonRequestBehavior.AllowGet);
        }

    }
}
