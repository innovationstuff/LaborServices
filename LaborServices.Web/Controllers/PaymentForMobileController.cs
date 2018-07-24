using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Managers.Identity;
using LaborServices.Model;
using LaborServices.Models;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using LaborServices.Web.SettingsData;
using Westwind.Globalization;
using LaborServices.Utility;

namespace LaborServices.Web.Controllers
{
    public class PaymentForMobileController : BaseController
    {

        string StaticIP = ConfigurationManager.AppSettings["APIServerUrl"];
        private readonly PaymentTransactionStoreBase _PaymentTransactionStoreBase;
        private readonly ReceiptVoucherStoreBase _ReceiptVoucherStoreBase;
        private readonly SettingStoreBase _storeBase;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public PaymentForMobileController()
        {
            _PaymentTransactionStoreBase = new PaymentTransactionStoreBase(new LaborServicesDbContext());
            _ReceiptVoucherStoreBase = new ReceiptVoucherStoreBase(new LaborServicesDbContext());
            _storeBase = new SettingStoreBase(new LaborServicesDbContext());
        }

        public async Task<ActionResult> PaymentMethod(string id,string UserId)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var currentUser = UserManager.FindById(UserId);

            string url = string.Format("api/Profile/HourlyContract/Details/{0}?userId={1}", id, currentUser.CrmUserId);
            var contract = await GetResourceAsync<ServiceContractPerHour>(url);

            if (contract.StatusCode != "100000006")
            {
                return RedirectToAction("Details", "HourlyWorkers", new { id });
            }
            contract.Customer = currentUser.Name;

            ViewBag.ContractId = id;
            return View(contract);
        }


        [HttpGet]
        public async Task<ActionResult> BankTransfer(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            string url = string.Format("api/HourlyContract/{0}", id);
            var contract = await GetResourceAsync<ServiceContractPerHour>(url);

            if (contract == null) return HttpNotFound();

            if (contract.StatusCode == ((int)ServiceContractPerHour_Status.Cancelled).ToString())
                return RedirectToAction("CancelledContract", new { id });

            if (contract.StatusCode == ((int)ServiceContractPerHour_Status.PaymentIsPendingConfirmation).ToString())
                return RedirectToAction("UploadBankStatementDoneBefore", new { id });

            ViewBag.id = id;

            return View(contract);
        }

        Dictionary<string, dynamic> _responseData;
        public Dictionary<string, dynamic> CheckOutRequest(string mobile, ContractViewModel contractToPay)
        {
            //string customerMobile = Request.Cookies["userInfo"].Value.Split('=')[1];
            string customerMobile = mobile;
            Session["contractToPay"] = contractToPay;
            //------------------Test Environment---------------------------+ Math.Round(decimal.Parse(contractToPay.FinalPrice), 2).ToString()+decimal.Parse("0.00") 
            //string data =
            //    "authentication.userId=" + ConfigurationManager.AppSettings["authenticationuserId"] +
            //    "&authentication.password=" + ConfigurationManager.AppSettings["authenticationpassword"] +
            //    "&authentication.entityId=" + ConfigurationManager.AppSettings["authenticationentityId"] +
            //    "&amount=" + String.Format("{0:0.00}", decimal.Parse( contractToPay.FinalPrice)) +
            //"&currency=" + ConfigurationManager.AppSettings["checkOutcurrancy"] +
            //    "&paymentType=" + ConfigurationManager.AppSettings["checkoutpaymentType"] +
            //    "&merchantTransactionId=" + contractToPay.ContractId +"#"+ DateTime.Now.Ticks.ToString() +
            //    "&customer.email=" + customerMobile + DateTime.Now.Ticks.ToString() + "@gmail.com" +
            //    "&testMode=EXTERNAL";

            //------------------Live Environment---------------------------
            string data =
                "authentication.userId=" + ConfigurationManager.AppSettings["authenticationuserId"] +
                "&authentication.password=" + ConfigurationManager.AppSettings["authenticationpassword"] +
                "&authentication.entityId=" + ConfigurationManager.AppSettings["authenticationentityId"] +
              "&amount=" + String.Format("{0:0.00}", decimal.Parse(contractToPay.FinalPrice)) +
                //"&currency=" + ConfigurationManager.AppSettings["checkOutcurrancy"] +
                "&currency=" + _storeBase.GetSettingValueByName("checkOutcurrancy") +
                 //"&paymentType=" + ConfigurationManager.AppSettings["checkoutpaymentType"] +
                 "&paymentType=" + _storeBase.GetSettingValueByName("checkoutpaymentType") +
                "&merchantTransactionId=" + contractToPay.ContractId + "#" + DateTime.Now.Ticks.ToString() +
                "&customer.email=" + customerMobile + DateTime.Now.Ticks.ToString() + "@gmail.com";

            //string url = ConfigurationManager.AppSettings["checkoutUrl"];
            string url = _storeBase.GetSettingValueByName("checkoutUrl");
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream postData = request.GetRequestStream();
            postData.Write(buffer, 0, buffer.Length);
            postData.Close();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var s = new JavaScriptSerializer();
            _responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
            reader.Close();
            dataStream.Close();

            return _responseData;
        }

        public ActionResult ShopperResult(ContractViewModel newContract, string UserId)
        {
            var currentUser = UserManager.FindById(UserId);

            newContract = Session["ContractToPayData"] as ContractViewModel;
            _responseData = CheckOutRequest(currentUser.PhoneNumber, newContract);
            Session["CheckoutId"] = _responseData["id"];
            return View(newContract);
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
                                    UrlToRedirect = Url.Action("Details", "HourlyWorkers", new { id, lang = (Lang == Language.Arabic ? "ar" : "en") }),
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
                IsWithAutoRedirect = false,
                //UrlToRedirect = Url.Action("Index", "Home", new { lang = Lang == Language.Arabic ? "ar" : "en" }),
                //RedirectTimeout = 10
            };
            return View("Failure", failMsg);
        }

        public ActionResult CancelledContract(string id)
        {
            var model = new ResultMessageVM()
            {
                Title = DbRes.T("ContractIsCancelled", "DalalResources"),
                Message = DbRes.T("CancelledContractMsg", "DalalResources"),
                IsWithAutoRedirect = true,
                UrlToRedirect = Url.Action("Details", "HourlyWorkers", new { id, lang = (Lang == Language.Arabic ? "ar" : "en") }),
                RedirectTimeout = 10
            };
            return View("Failure", model);
            //return View("Success", model);
            //return View("Warning", model);
        }

        public ActionResult UploadBankStatementDoneBefore(string id)
        {
            var model = new ResultMessageVM()
            {
                Title = DbRes.T("BankTransferIsUploadedBefore", "DalalResources"),
                Message = DbRes.T("UploadedBankTransferBeforeMsg", "DalalResources"),
                IsWithAutoRedirect = true,
                UrlToRedirect = Url.Action("Details", "HourlyWorkers", new { id, lang = (Lang == Language.Arabic ? "ar" : "en") }),
                RedirectTimeout = 10
            };
            return View("Warning", model);
        }


        public async Task<ActionResult> DalalShopperResult(string id, string UserId)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var currentUser = UserManager.FindById(UserId);
            Session["UserId"] = UserId;
            string url = string.Format("api/Profile/HourlyContract/Details/{0}?userId={1}", id, currentUser.CrmUserId);
            var basicDetails = await GetResourceAsync<ServiceContractPerHour>(url);

            if (basicDetails.StatusCode != "100000006")
            {
                return RedirectToAction("Details", "HourlyWorkers", new { id = id });
            }

            ContractViewModel newContract = new ContractViewModel()
            {
                ContractId = basicDetails.ContractId,
                ContractNum = basicDetails.ContractNum,

                PriceBeforeDiscount = basicDetails.totalpricewithoutvat,
                FinalPrice = basicDetails.FinalPrice.Value.ToString("F"),
                VatRate = basicDetails.vatrate,
                Customer = currentUser.Name
            };


            _responseData = CheckOutRequest(currentUser.PhoneNumber, newContract);
            Session["CheckoutId"] = _responseData["id"];
            return View("ShopperResult", newContract);
        }


        public async Task<ActionResult> Status()
        {
            string fullCheckoutId = Request.QueryString["id"].ToString();
            string fullCheckoutResourcePath = Request.QueryString["resourcePath"].ToString();
            string realCheckoutId = fullCheckoutId;
            //"6D6477C462F95472B02F00779D233FA4.prod01-vm-tx01"
            ContractViewModel contractToPay = Session["contractToPay"] as ContractViewModel;
            PaymentTransaction transaction = new PaymentTransaction();

            if (!string.IsNullOrEmpty(realCheckoutId))
            {
                Dictionary<string, dynamic> paymentStatusResult = StatuesRequestRequest(realCheckoutId);

                string requiredValue = paymentStatusResult["result"]["description"];
                string requiredCode = paymentStatusResult["result"]["code"];//000.100.112 success

                string UserId = Session["UserId"].ToString();
                var currentUser = UserManager.FindById(UserId);
                transaction.CustomerId = currentUser.CrmUserId;
                contractToPay.CustomerId = currentUser.CrmUserId;


                transaction.ContractId = contractToPay.ContractId;
                transaction.PaymentStatus = requiredCode;
                transaction.PaymentStatusName = requiredValue;
                transaction.Amount = Convert.ToDecimal(contractToPay.FinalPrice);
                transaction.Who = 2;

                //000.000.000   Live Transaction Success Code
                //Transaction succeeded
                if (requiredCode == "000.000.000" || requiredValue == "Transaction succeeded")
                {
                    transaction.EntityName = "Successfull Payment Transaction for Contract Number [ " + contractToPay.ContractNum + " ] with Total price of : [ " + contractToPay.FinalPrice + " SR ] and customer : [ " + contractToPay.CustomerId + " ]";

                    transaction.CreatedDate = DateTime.Now;
                    transaction.ModifiedDate = DateTime.Now;
                    transaction.IsVoucherSaved = false;

                    PaymentTransaction newTransaction = Create(transaction);

                    ReceiptVoucherViewModel model = await CreateReceiptVoucher(contractToPay, newTransaction);

                    newTransaction.IsVoucherSaved = model != null ? true : false;
                    Update(newTransaction);


                    return RedirectToAction("Success", new { id = transaction.ContractId });
                }
                else
                {
                    transaction.EntityName = "Failed Payment Transaction for Contract Number [ " + contractToPay.ContractNum + " ] with Total price of : [ " + contractToPay.FinalPrice + " SR ] and customer : [ " + contractToPay.CustomerId + " ]";
                    transaction.IsVoucherSaved = false;
                    transaction.CreatedDate = DateTime.Now;
                    transaction.ModifiedDate = DateTime.Now;
                    Create(transaction);

                    return RedirectToAction("Failure", new { reason = requiredValue, id = transaction.ContractId });
                }
            }
            else
            {
                transaction.CustomerId = contractToPay.CustomerId;
                transaction.ContractId = contractToPay.ContractId;
                transaction.PaymentStatus = "000000";
                transaction.PaymentStatusName = "Checkout payment failed";
                transaction.Amount = Convert.ToDecimal(contractToPay.FinalPrice);
                transaction.Who = 2;
                transaction.EntityName = "Failed Payment Transaction for Contract Number [ " + contractToPay.ContractNum + " ] with Total price of : [ " + contractToPay.FinalPrice + " SR ] and customer : [ " + contractToPay.CustomerId + " ]";
                transaction.IsVoucherSaved = false;
                transaction.CreatedDate = DateTime.Now;
                transaction.ModifiedDate = DateTime.Now;
                Create(transaction);
                return RedirectToAction("Failure", new { reason = "There is not checkout id retrieved for this payment" });
            }
        }



        public Dictionary<string, dynamic> StatuesRequestRequest(string checkOutId)
        {
            Dictionary<string, dynamic> responseData;
            string data =
                "authentication.userId=" + ConfigurationManager.AppSettings["authenticationuserId"] +
                "&authentication.password=" + ConfigurationManager.AppSettings["authenticationpassword"] +
                "&authentication.entityId=" + ConfigurationManager.AppSettings["authenticationentityId"];

            //string url = ConfigurationManager.AppSettings["checkoutUrl"] + "/{id}/payment?" + data;
            string url = _storeBase.GetSettingValueByName("checkoutUrl") + "/{id}/payment?" + data;
            url = url.Replace("{id}", checkOutId);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var s = new JavaScriptSerializer();
                responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }
            return responseData;
        }
        public PaymentTransaction Create(PaymentTransaction model)
        {
            try
            {
                var paymentModel = new PaymentTransaction();
                paymentModel = _PaymentTransactionStoreBase.Create(model);

                return paymentModel;
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);

                return new PaymentTransaction();
            }
        }

        public PaymentTransaction Update(PaymentTransaction model)
        {
            try
            {
                var paymentModel = new PaymentTransaction();
                paymentModel = _PaymentTransactionStoreBase.Update(model);

                return paymentModel;
            }
            catch (Exception ex)
            {
                return new PaymentTransaction();
            }
        }

        public async Task<ReceiptVoucherViewModel> CreateReceiptVoucher(ContractViewModel contractToPay, PaymentTransaction newTransaction)
        {
            try
            {


                var data = new ReceiptVoucherViewModel();
                data.contractid = contractToPay.ContractId;
                data.Customerid = contractToPay.CustomerId;
                data.Contractnumber = contractToPay.ContractNum;
                data.amount = contractToPay.PriceBeforeDiscount;

                CultureInfo info = new CultureInfo("en-us");
                data.datatime = DateTime.Now.ToString("dd/MM/yyyy", info.DateTimeFormat);
                data.paymentcode = "2";//ToDo
                data.paymenttype = 2;
                data.vatrate = contractToPay.VatRate;
                data.who = 2;
                var newVoucher = await PostResourceAsync<ReceiptVoucherViewModel>("api/Payment/AddRecieptVoucher", data);
                if (newVoucher.StatusCode == HttpStatusCode.OK)
                {
                    //SaveFailedReceiptVoucher(contractToPay, newTransaction);
                    return newVoucher.Result;
                }
                else
                {
                    return SaveFailedReceiptVoucher(contractToPay, newTransaction);
                }
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);

                return SaveFailedReceiptVoucher(contractToPay, newTransaction);
            }
        }

        public ReceiptVoucherViewModel SaveFailedReceiptVoucher(ContractViewModel contractToPay, PaymentTransaction newTransaction)
        {
            //Insert into failed reciept vouchers only
            try
            {
                //string ToEmails = ConfigurationSettings.AppSettings["PaymentFailureEmails"].ToString();
                string ToEmails = _storeBase.GetSettingValueByName("PaymentFailureEmails");
                string CCEmail = "";
                string subject = "خطأ في إنشاء سند قبض للعميل من علي البورتال";
                string body = " خطأ في إنشاء سند قبض للعميل رقم ";
                body += contractToPay.CustomerId;
                body += " من علي البورتال لعقد رقم ";
                body += contractToPay.ContractId;

                var receipt = new ReceiptVoucher();
                receipt.ContractId = contractToPay.ContractId;
                receipt.CustomerId = contractToPay.CustomerId;
                receipt.ContractNumber = contractToPay.ContractNum;
                receipt.Amount = Convert.ToDecimal(contractToPay.PriceBeforeDiscount);

                CultureInfo info1 = new CultureInfo("en-us");
                receipt.Date = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
                receipt.PaymentCode = "2";//ToDo
                receipt.PaymentType = 2;
                receipt.VatRate = Convert.ToDecimal(contractToPay.VatRate);
                receipt.Who = 2;
                receipt.IsSaved = false;
                receipt.CreatedDate = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
                receipt.ModifiedDate = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
                receipt.TransactionId = newTransaction.Id;

                _ReceiptVoucherStoreBase.Create(receipt);

                MailSender.SendEmail02(ToEmails, CCEmail, subject, body, false, "");
                return new ReceiptVoucherViewModel();
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);

                return null;
            }
        }

    }
}