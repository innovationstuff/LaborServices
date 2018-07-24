using LaborServices.Model;
using LaborServices.Models;
using LaborServices.Utility;
using LaborServices.Web.Helpers;
using LaborServices.Web.Managers;
using LaborServices.Web.Models;
using LaborServices.Web.SettingsData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Westwind.Globalization;

namespace LaborServices.Web.Controllers
{
    public class DalalCrmController : BaseController
    {

        // GET: DalalCrm
        [HttpGet]
        public ActionResult Terms(string id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Terms(string id, bool approved)
        {
            if (approved)
            {
                var result = await PostResourceAsync<bool>("api/HourlyContract/ConfirmTerms/" + id, null);
                if (result.StatusCode == HttpStatusCode.OK && result.Result)
                {
                    return RedirectToAction("PickLocation", new { id });
                }
            }
            ViewBag.id = id;
            return View("Terms");
        }

        [HttpGet]
        public ActionResult PickLocation(string id)
        {
            var model = new PickCustomerLocationViewModel() { ContractId = id };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> PickLocation(PickCustomerLocationViewModel CustomerLocation)
        {

            if (!ModelState.IsValid)
            {
                return View(CustomerLocation);
            }

            if (string.IsNullOrEmpty(CustomerLocation.ContractId))
                return HttpNotFound();

            string url = string.Format("api/HourlyContract/{0}", CustomerLocation.ContractId);
            var contract = await GetResourceAsync<ServiceContractPerHour>(url);

            if (contract == null) return HttpNotFound();

            var result = await PostResourceAsync<bool>("api/HourlyContract/PickCustomerLocation", CustomerLocation);

            if (result.StatusCode == HttpStatusCode.OK && result.Result)
            {
                return RedirectToAction("SystemicPaymentMethod", new { id = CustomerLocation.ContractId });
            }

            return View(CustomerLocation);
        }

        public async Task<ActionResult> SystemicPaymentMethod(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            string url = string.Format("api/HourlyContract/{0}", id);
            var contract = await GetResourceAsync<ServiceContractPerHour>(url);

            if (contract == null) return HttpNotFound();

            ViewBag.ContractId = id;
            return View(contract);
        }

        [HttpGet]
        public async Task<ActionResult> SystemicPayOnline(string id)
        {
            var paymentMgr = new PaymentManager();

            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            string url = string.Format("api/HourlyContract/{0}", id);
            var basicDetails = await GetResourceAsync<ServiceContractPerHour>(url);

            if (basicDetails == null) return HttpNotFound();


            if (basicDetails.StatusCode != "100000006")
            {
                return HttpNotFound();
            }

            var _responseData = paymentMgr.CheckOutRequest(basicDetails);
            ViewBag.CheckoutId = _responseData["id"];

            return View(basicDetails);
        }

        [HttpGet]
        public async Task<ActionResult> SystemicBankTransfer(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            string url = string.Format("api/HourlyContract/{0}", id);
            var contract = await GetResourceAsync<ServiceContractPerHour>(url);

            if (contract == null) return HttpNotFound();

            if (contract.StatusCode == ((int)ServiceContractPerHour_Status.Cancelled).ToString())
                return RedirectToAction("CancelledContract");

            if (contract.StatusCode == ((int)ServiceContractPerHour_Status.PaymentIsPendingConfirmation).ToString())
                return RedirectToAction("UploadBankStatementDoneBefore");

            ViewBag.id = id;

            return View(contract);
        }

        [HttpPost]
        public async Task<ActionResult> SystemicBankTransfer(HttpPostedFileBase BankFile, string id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string apiServiceUrl = SharedClass.ApiServerUrl + (Lang == Language.English ? "en" : "ar") + "/api/HourlyContract/BankTransferStatementFile/" + id;

                    using (var content =
                        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(BankFile.InputStream), "BankFile", BankFile.FileName);

                        using (
                           var message =
                               await client.PostAsync(apiServiceUrl, content))
                        {
                            var input = await message.Content.ReadAsStringAsync();
                            if (message.IsSuccessStatusCode)
                            {
                                var successMsg = new ResultMessageVM()
                                {
                                    Title = DbRes.T("BankTransferIsUploaded", "DalalResources"),
                                    Message = DbRes.T("UploadedBankTransferMsg", "DalalResources"),
                                    IsWithAutoRedirect = true,
                                    UrlToRedirect = Url.Action("Index", "Home", new { lang = Lang == Language.Arabic ? "ar" : "en" }),
                                    RedirectTimeout = 10
                                };
                                return View("Success", successMsg);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            var failMsg = new ResultMessageVM()
            {
                Title = DbRes.T("ProblemInUploadingBankTransfer", "DalalResources"),
                Message = DbRes.T("ProblemInUploadingBankTransferMsg", "DalalResources"),
                IsWithAutoRedirect = true,
                UrlToRedirect = Url.Action("Index", "Home", new { lang = Lang == Language.Arabic ? "ar" : "en" }),
                RedirectTimeout = 10
            };
            return View("Failure", failMsg);
        }

        public async Task<ActionResult> PaymentStatus(string contractId)
        {
            var paymentMgr = new PaymentManager();

            string realCheckoutId = Request.QueryString["id"].ToString();

            string url = string.Format("api/HourlyContract/{0}", contractId);
            var contractToPay = await GetResourceAsync<ServiceContractPerHour>(url);

            if (paymentMgr.IsPaymentTransactionSucceededWithCheckoutId(realCheckoutId))
            {
                Dictionary<string, dynamic> paymentStatusResult = paymentMgr.StatuesRequestRequest(realCheckoutId);

                string requiredValue = paymentStatusResult["result"]["description"];
                string requiredCode = paymentStatusResult["result"]["code"];//000.100.112 success

                //000.000.000   Live Transaction Success Code
                //Transaction succeeded
                if (requiredCode == "000.000.000" || requiredValue == "Transaction succeeded")
                {
                   var transaction = paymentMgr.AddSucceededPaymentTransaction(contractToPay, requiredCode, requiredValue);
                    if (transaction != null)
                    {
                        await CreateReceiptVoucher(contractToPay, transaction);
                        transaction.IsVoucherSaved = true;
                        paymentMgr.UpdatePaymentTransaction(transaction);
                    }

                    var resultMsg = new ResultMessageVM()
                    {
                        Title = DbRes.T("SuccessPaymentOperation", "DalalResources"),
                        Message = DbRes.T("PaymentOperationDoneSuccessfully", "DalalResources"),
                        IsWithAutoRedirect = true,
                        UrlToRedirect = Url.Action("Index", "Home", new { lang = Lang == Language.Arabic ? "ar" : "en" }),
                        RedirectTimeout = 10
                    };

                    return View("Success", resultMsg);
                }
                else
                {
                    paymentMgr.AddFailedPaymentTransaction(contractToPay, requiredCode, requiredValue);
                    return RedirectToAction("PaymentFailure", new { reason = requiredValue, id = contractId });
                }
            }
            else
            {
                paymentMgr.AddFailedPaymentTransactionWithNoCheckoutId(contractToPay);
                return RedirectToAction("PaymentFailure", new { id = contractId, reason = "There is not checkout id retrieved for this payment" });
            }
        }

        public async Task<ReceiptVoucherViewModel> CreateReceiptVoucher(ServiceContractPerHour contractToPay, PaymentTransaction newTransaction)
        {
            var paymentMgr = new PaymentManager();

            try
            {


                var data = new ReceiptVoucherViewModel();
                data.contractid = contractToPay.ContractId;
                data.Customerid = contractToPay.CustomerId;
                data.Contractnumber = contractToPay.ContractNum;
                data.amount = contractToPay.totalpricewithoutvat;

                CultureInfo info = new CultureInfo("en-us");
                data.datatime = DateTime.Now.ToString("dd/MM/yyyy", info.DateTimeFormat);
                data.paymentcode = "2";//ToDo
                data.paymenttype = 2;
                data.vatrate = contractToPay.vatrate;
                data.who = 2;
                var newVoucher = await PostResourceAsync<ReceiptVoucherViewModel>("api/Payment/AddRecieptVoucher", data);
                if (newVoucher.StatusCode == HttpStatusCode.OK)
                {
                    //SaveFailedReceiptVoucher(contractToPay, newTransaction);
                    return newVoucher.Result;
                }
                else
                {
                    return paymentMgr.SaveFailedReceiptVoucher(contractToPay, newTransaction);
                }
            }
            catch (Exception ex)
            {
                return paymentMgr.SaveFailedReceiptVoucher(contractToPay, newTransaction);
            }
        }

        public ActionResult CancelledContract()
        {
            var model = new ResultMessageVM()
            {
                Title = DbRes.T("ContractIsCancelled", "DalalResources"),
                Message = DbRes.T("CancelledContractMsg", "DalalResources"),
                IsWithAutoRedirect = true,
                UrlToRedirect = Url.Action("Index", "Home", new { lang = Lang == Language.Arabic ? "ar" : "en" }),
                RedirectTimeout = 10
            };
            return View("Failure", model);
            //return View("Success", model);
            //return View("Warning", model);
        }

        public ActionResult UploadBankStatementDoneBefore()
        {
            var model = new ResultMessageVM()
            {
                Title = DbRes.T("BankTransferIsUploadedBefore", "DalalResources"),
                Message = DbRes.T("UploadedBankTransferBeforeMsg", "DalalResources"),
                IsWithAutoRedirect = true,
                UrlToRedirect = Url.Action("Index", "Home", new { lang = Lang == Language.Arabic ? "ar" : "en" }),
                RedirectTimeout = 10
            };
            return View("Warning", model);
        }


        public ActionResult PaymentFailure(string id, string reason)
        {


            var msgHtml = String.Format(@"
                                        <div class='mt-3'>
                                            <a href='{0}' class='btn btn-lg btn-info'>
                                              {1}  <i class='fa fa-lock fa-lg'></i>&nbsp;
                                            </a>
                                        </div>
                                        <br />
                                        <p>
                                            <b>{2}</b>  : {3}
                                        </p>
                                        ", Url.Action("SystemicPayOnline", "DalalCrm", new { id, lang = (Lang == Language.Arabic ? "ar" : "en") }),
                                        DbRes.T("PayBtn", "Shared"), DbRes.T("Reason", "Shared"), reason);

            var resultMsg = new ResultMessageVM()
            {
                Title = DbRes.T("FailedPaymentOperation", "DalalResources"),
                Message = DbRes.T("PaymentOperationFailed", "DalalResources"),
                HtmlContent = msgHtml
            };

            return View("Warning", resultMsg);
        }

        public ActionResult ErrorTest()
        {
            return View();
        }  

    }
}