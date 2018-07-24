using LaborServices.Managers.Identity;
using LaborServices.Web.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Westwind.Globalization;
using LaborServices.Utility;
using LaborServices.Web.Controllers;
using LaborServices.Web.Managers;
using System.Collections.Generic;
using System.Net;
using System;

namespace LaborServices.Web.Controllers
{
    public class PayController : BaseController
    {
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


        #region individual

        [HttpGet]
        [Route("Pay/Individual/{id}")]
        [Route("{lang?}/Pay/Individual/{id}")]
        public async Task<ActionResult> IndividualPaymentMethod(string id)
        {
            DomesticInvoicePaymentManager Mgr; bool offline;
            if (Request.IsAuthenticated && User.Identity.IsAuthenticated)
            {
                offline = false;
                Mgr = new DomesticInvoicePaymentManager(Lang, UserManager.FindById(User.Identity.GetUserId()));
            }
            else
            {
                offline = true;
                Mgr = new OfflineDomesticInvoicePaymentManager(Lang);
            }

            DomesticInvoice invoice = await Mgr.GetDomesticInvoice(id);

            if (invoice == null) return HttpNotFound();

            if (invoice.IsPaid)
            {
                var resultMsg = IndividualPaidDomesticInvoice(id, offline);
                return View("Warning", resultMsg);
            }

            ViewBag.Offline = offline;
            return View(invoice);
        }


        [HttpGet]
        [Route("Pay/Individual/Online/{id}")]
        [Route("{lang?}/Pay/Individual/Online/{id}")]
        public async Task<ActionResult> IndividualOnlinePayment(string id)
        {

            DomesticInvoicePaymentManager Mgr; bool offline;
            if (Request.IsAuthenticated && User.Identity.IsAuthenticated)
            {
                offline = false;
                Mgr = new DomesticInvoicePaymentManager(Lang, UserManager.FindById(User.Identity.GetUserId()));
            }
            else
            {
                offline = true;
                Mgr = new OfflineDomesticInvoicePaymentManager(Lang);
            }

            DomesticInvoice invoice = await Mgr.GetDomesticInvoice(id);
            //System.Globalization.CultureInfo enUS = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            //DateTime tempdate;
            //string s = invoice.DueDate.Value.ToString("dd-MM-yyyy", enUS);
            //tempdate = DateTime.Parse(s);
            //invoice.DueDate = tempdate;
            if (invoice == null) return HttpNotFound();

            if (invoice.IsPaid)
            {
                var resultMsg = IndividualPaidDomesticInvoice(id, offline);
                return View("Warning", resultMsg);
            }

            ViewBag.Offline = offline;


            var _responseData = Mgr.CheckOutRequestToPayOnline(invoice.CustomerMobilePhone, invoice.Number, invoice.InvoiceAmount.Value);
            ViewBag.CheckoutId = _responseData["id"];

            return View(invoice);
        }

        [HttpGet]
        [Route("Pay/Individual/BankTransfer/{id}")]
        [Route("{lang?}/Pay/Individual/BankTransfer/{id}")]
        public async Task<ActionResult> IndividualBankTransfer(string id)
        {
            DomesticInvoicePaymentManager Mgr; bool offline;
            if (Request.IsAuthenticated && User.Identity.IsAuthenticated)
            {
                offline = false;
                Mgr = new DomesticInvoicePaymentManager(Lang, UserManager.FindById(User.Identity.GetUserId()));
            }
            else
            {
                offline = true;
                Mgr = new OfflineDomesticInvoicePaymentManager(Lang);
            }

            DomesticInvoice invoice = await Mgr.GetDomesticInvoice(id);

            if (invoice == null) return HttpNotFound();

            if (invoice.IsPaid)
            {
                var resultMsg = IndividualPaidDomesticInvoice(id, offline);
                return View("Warning", resultMsg);
            }

            ViewBag.Offline = offline;
            return View(invoice);
        }

        [HttpGet]
        [Route("Pay/Individual/OnlineResponse")]
        [Route("{lang?}/Pay/Individual/OnlineResponse")]
        public async Task<ActionResult> IndividualOnlineResponse(string InvoiceId)
        {
            DomesticInvoicePaymentManager Mgr; bool offline;
            if (Request.IsAuthenticated && User.Identity.IsAuthenticated)
            {
                offline = false;
                Mgr = new DomesticInvoicePaymentManager(Lang, UserManager.FindById(User.Identity.GetUserId()));
            }
            else
            {
                offline = true;
                Mgr = new OfflineDomesticInvoicePaymentManager(Lang);
            }
            ViewBag.Offline = offline;

            DomesticInvoice invoice = await Mgr.GetDomesticInvoice(InvoiceId);


            var paymentResponse = Mgr.GetPaymentResponse(Request.QueryString["id"].ToString());

            if (paymentResponse.Status == HyperPayStatus.Success)
            {
                // Add Succeed Transaction to LaborServices Db
                var transaction = Mgr.AddSuccessTransaction(invoice.CustomerId, invoice.Number, InvoiceId, invoice.InvoiceAmount.Value, paymentResponse);

                // Add reciept voucher & Update Invoice IsPaid Field
                var response = await Mgr.CreateReceiptVoucher(invoice);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // success CRM voucher creation
                    transaction.IsVoucherSaved = true;
                    Mgr.UpdatePaymentTransaction(transaction);
                }
                else
                {
                    // fail CRM voucher creation
                    Mgr.SaveFailedReceiptVoucher(response.Result, transaction.Id);
                }
                // return success msg
                return View("Success", IndividualSuccessOnlinePayment(InvoiceId, offline));
            }
            else
            {
                // Add Failed Transaction to LaborServices Db
                Mgr.AddFailTransaction(invoice.CustomerId, invoice.Number, InvoiceId, invoice.InvoiceAmount.Value, paymentResponse);

                // return fail msg
                return View("Failure", PaymentFailureMsg(InvoiceId, paymentResponse.Reason,
                    Url.Action("IndividualOnlinePayment", "Pay", new { id = InvoiceId, lang = LangCode })));
            }
        }

        [HttpPost]
        [Route("Pay/Individual/BankTransfer/{id}")]
        [Route("{lang?}/Pay/Individual/BankTransfer/{id}")]
        public async Task<ActionResult> IndividualBankTransfer(HttpPostedFileBase BankFile, string id)
        {
            try
            {
                DomesticInvoicePaymentManager Mgr; bool offline;
                if (Request.IsAuthenticated && User.Identity.IsAuthenticated)
                {
                    offline = false;
                    Mgr = new DomesticInvoicePaymentManager(Lang, UserManager.FindById(User.Identity.GetUserId()));
                }
                else
                {
                    offline = true;
                    Mgr = new OfflineDomesticInvoicePaymentManager(Lang);
                }
                ViewBag.Offline = offline;

                var result = await Mgr.UploadBankTransferFile(BankFile, id);
                if (result.IsSuccessStatusCode)
                {
                    var successMsg = IndividualSuccessBankTransferFileUploadMsg(id, offline);
                    return View("Success", successMsg);

                }
            }
            catch (Exception ex)
            {

            }

            var failMsg = IndividualFailBankTransferFileUploadMsg(id);
            return View("Failure", failMsg);
        }


        public ResultMessageVM IndividualPaidDomesticInvoice(string id, bool offline = false)
        {
            string urlToRedirectIfPaid;

            if (!offline)
                urlToRedirectIfPaid = Url.Action("DomesticInvoice", "DomesticInvoice", new { lang = LangCode, id });
            else
                urlToRedirectIfPaid = Url.Action("Index", "Home", new { lang = LangCode });

            var resultMsg = new ResultMessageVM()
            {
                Title = DbRes.T("InvoiceIsPaidBefore", "Payment"),
                Message = DbRes.T("InvoiceIsPaidBeforeMsg", "Payment"),
                IsWithAutoRedirect = true,
                UrlToRedirect = urlToRedirectIfPaid,
                RedirectTimeout = 10
            };
            return resultMsg;
        }

        public ResultMessageVM IndividualSuccessOnlinePayment(string id, bool offline = false)
        {
            string redirectUrl;

            if (!offline)
                redirectUrl = Url.Action("DomesticInvoice", "DomesticInvoice", new { lang = LangCode, id });
            else
                redirectUrl = Url.Action("Index", "Home", new { lang = LangCode });

            var resultMsg = new ResultMessageVM()
            {
                Title = DbRes.T("SuccessPaymentOperation", "Payment"),
                Message = DbRes.T("PaymentOperationDoneSuccessfully", "Payment"),
                IsWithAutoRedirect = true,
                UrlToRedirect = redirectUrl,
                RedirectTimeout = 10
            };

            return resultMsg;
        }

        public ResultMessageVM IndividualSuccessBankTransferFileUploadMsg(string id, bool offline = false)
        {
            string redirectUrl;

            if (!offline)
                redirectUrl = Url.Action("DomesticInvoice", "DomesticInvoice", new { lang = LangCode, id });
            else
                redirectUrl = Url.Action("Index", "Home", new { lang = LangCode });

            var resultMsg = new ResultMessageVM()
            {
                Title = DbRes.T("BankTransferIsUploaded", "Payment"),
                Message = DbRes.T("UploadedBankTransferMsg", "Payment"),
                IsWithAutoRedirect = true,
                UrlToRedirect = redirectUrl,
                RedirectTimeout = 10
            };

            return resultMsg;
        }

        public ResultMessageVM IndividualFailBankTransferFileUploadMsg(string id)
        {

            var resultMsg = new ResultMessageVM()
            {
                Title = DbRes.T("ProblemInUploadingBankTransfer", "Payment"),
                Message = DbRes.T("ProblemInUploadingBankTransferMsg", "Payment"),
            };

            return resultMsg;
        }



        #endregion

        public ResultMessageVM PaymentFailureMsg(string id, string reason, string urlToPayAgain)
        {


            var msgHtml = string.Format(@"
                                        <div class='mt-3'>
                                            <a href='{0}' class='btn btn-lg btn-info'>
                                              {1}  <i class='fa fa-lock fa-lg'></i>&nbsp;
                                            </a>
                                        </div>
                                        <br />
                                        <p>
                                            <b>{2}</b>  : {3}
                                        </p>
                                        ", urlToPayAgain,
                                        DbRes.T("PayBtn", "Shared"), DbRes.T("Reason", "Shared"), reason);

            var resultMsg = new ResultMessageVM()
            {
                Title = DbRes.T("FailedPaymentOperation", "Payment"),
                Message = DbRes.T("PaymentOperationFailed", "Payment"),
                HtmlContent = msgHtml
            };

            return resultMsg;
        }
    }
}