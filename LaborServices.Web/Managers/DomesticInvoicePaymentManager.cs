using LaborServices.Entity;
using LaborServices.Entity.Identity;
using LaborServices.Managers;
using LaborServices.Model;
using LaborServices.Models;
using LaborServices.Utility;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Westwind.Globalization;

namespace LaborServices.Web.Managers
{
    public class DomesticInvoicePaymentManager
    {
        public Language Lang { get; set; }
        public ApplicationUser User { get; set; }
        public DomesticInvoicePaymentManager(Language lang, ApplicationUser user)
        {
            Lang = lang;
            this.User = user;
        }
        public virtual Task<DomesticInvoice> GetDomesticInvoice(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var caller = new ApiCaller(Lang);
            var apiUrl = string.Format("api/Profile/DomesticInvoice/Details/{0}?userId={1}", id, User.CrmUserId);

            return caller.GetResourceAsync<DomesticInvoice>(apiUrl);
        }

        public virtual Dictionary<string, dynamic> CheckOutRequestToPayOnline(string MobilePhone, string InvoiceNum, decimal FinalPrice)
        {
            using (var paymentMgr = new PaymentManager())
            {
                return paymentMgr.CheckOutRequest(MobilePhone, InvoiceNum, FinalPrice);
            }
        }

        public virtual HyperPayResponse GetPaymentResponse(string checkoutId)
        {
            using (var paymentMgr = new PaymentManager())
            {
                return paymentMgr.GetPaymentResponse(checkoutId);
            }
        }

        public virtual Task<HttpResponseMessage> UploadBankTransferFile(HttpPostedFileBase File, string id)
        {
            var caller = new ApiCaller(Lang);
            var apiUrl = String.Format("api/Payment/Individual/UploadInvoiceBankFile/{0}", id);
            return caller.PostFileAsync(apiUrl, File);
        }

        public PaymentTransaction AddSuccessTransaction(string customerId, string invoiceNum, string invoiceId, decimal amount, HyperPayResponse hyperPay)
        {
            try
            {
                PaymentTransaction transaction = new PaymentTransaction()
                {
                    CustomerId = customerId,
                    ContractId = invoiceId,
                    PaymentStatus = hyperPay.RequiredCode,
                    PaymentStatusName = hyperPay.RequiredValue,
                    Amount = Convert.ToDecimal(amount),
                    Who = 2,
                    EntityName = "Successfull Payment Transaction for Domestic Invoice No [ " + invoiceNum + " ] with Total price of : [ " + amount + " SR ] and customer : [ " + customerId + " ]",
                    IsVoucherSaved = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TransactionType = (int)TransactionType.DomesticInvoice,
                    TransactionTypeName = TransactionType.DomesticInvoice.ToString()

                };

                using (var paymentMgr = new PaymentManager())
                {
                    transaction = paymentMgr.CreatePaymentTransaction(transaction);
                    return transaction;

                }
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);
                return null;
            }
        }

        public PaymentTransaction AddFailTransaction(string customerId, string invoiceNum, string invoiceId, decimal amount, HyperPayResponse hyperPay)
        {
            try
            {
                PaymentTransaction transaction = new PaymentTransaction()
                {
                    CustomerId = customerId,
                    ContractId = invoiceId,
                    Amount = amount,
                    Who = 2,
                    EntityName = "Failed Payment Transaction for Contract Number [ " + invoiceNum + " ] with Total price of : [ " + amount + " SR ] and customer : [ " + customerId + " ]",
                    IsVoucherSaved = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TransactionType = (int)TransactionType.DomesticInvoice,
                    TransactionTypeName = TransactionType.DomesticInvoice.ToString()
                };

                if (String.IsNullOrEmpty(hyperPay.CheckoutId))
                {
                    transaction.PaymentStatus = "000000";
                    transaction.PaymentStatusName = "Checkout payment failed";
                }
                else
                {
                    transaction.PaymentStatus = hyperPay.RequiredCode;
                    transaction.PaymentStatusName = hyperPay.RequiredValue;
                }

                using (var paymentMgr = new PaymentManager())
                {
                    transaction = paymentMgr.CreatePaymentTransaction(transaction);
                    return transaction;

                }
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);
                return null;
            }
        }


        public virtual Task<APIResponseModel<ReceiptVoucherViewModel>> CreateReceiptVoucher(DomesticInvoice invoice)
        {
            CultureInfo info = new CultureInfo("en-us");
            ReceiptVoucherViewModel data = new ReceiptVoucherViewModel()
            {

                contractid = invoice.ContractId,
                Customerid = invoice.CustomerId,
                Contractnumber = invoice.Contract,
                InvoiceNumber = invoice.Number,
                amount = invoice.InvoiceAmount.ToString(),
                datatime = DateTime.Now.ToString("dd/MM/yyyy", info.DateTimeFormat),
                paymentcode = "2",
                vatrate = "0.0",
                who = AppConstants.Who_WebSource,
                InvoiceId = invoice.Id
            };

            try
            {
                var caller = new ApiCaller(Lang);
                var apiUrl = String.Format("api/Payment/Individual/AddRecieptVoucher");
                return caller.PostResourceAsync<ReceiptVoucherViewModel>(apiUrl, data);
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);

                return new Task<APIResponseModel<ReceiptVoucherViewModel>>(() => new APIResponseModel<ReceiptVoucherViewModel>() { StatusCode = System.Net.HttpStatusCode.InternalServerError, Result = data });
            }
        }

        public virtual PaymentTransaction UpdatePaymentTransaction(PaymentTransaction transaction)
        {
            using (var paymentMgr = new PaymentManager())
            {
                transaction = paymentMgr.UpdatePaymentTransaction(transaction);
                return transaction;
            }
        }

        public ReceiptVoucher SaveFailedReceiptVoucher(ReceiptVoucherViewModel receiptVoucher, long transactionId)
        {
            //Insert into failed reciept vouchers only
            try
            {
               
                using (var paymentMgr = new PaymentManager())
                {
                    return paymentMgr.SaveFailedReceiptVoucher(receiptVoucher, transactionId);
                }

             
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);
                return null;
            }
            finally
            {
                //string ToEmails = ConfigurationManager.AppSettings["PaymentFailureEmails"].ToString();
                 SettingStoreBase _storeBase;
                _storeBase = new SettingStoreBase(new LaborServicesDbContext());
                string ToEmails = _storeBase.GetSettingValueByName("PaymentFailureEmails");
                string CCEmail = "";
                string subject = "خطأ في إنشاء سند قبض للعميل من علي البورتال";
                string body = " خطأ في إنشاء سند قبض للعميل رقم ";
                body += receiptVoucher.Customerid;
                body += " من علي البورتال لعقد رقم ";
                body += receiptVoucher.contractid;

                MailSender.SendEmail02(ToEmails, CCEmail, subject, body, false, "");
            }
        }


    }
}