using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Model;
using LaborServices.Models;
using LaborServices.Utility;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace LaborServices.Web.Managers
{
    public class PaymentManager : IDisposable
    {
        private readonly PaymentTransactionStoreBase _PaymentTransactionStoreBase;
        private readonly ReceiptVoucherStoreBase _ReceiptVoucherStoreBase;
        private readonly SettingStoreBase _storeBase;

        public PaymentManager()
        {
            _ReceiptVoucherStoreBase = new ReceiptVoucherStoreBase(new LaborServicesDbContext());
            _PaymentTransactionStoreBase = new PaymentTransactionStoreBase(new LaborServicesDbContext());
            _storeBase = new SettingStoreBase(new LaborServicesDbContext());
        }
        public Dictionary<string, dynamic> CheckOutRequest(ServiceContractPerHour contractToPay)
        {
            return CheckOutRequest(contractToPay.CustomerMobilePhone, contractToPay.ContractId, contractToPay.FinalPrice ?? 0m);
        }

        //public Dictionary<string, dynamic> CheckOutRequest(ContractViewModel contractToPay)
        //{

        //    return CheckOutRequest(contractToPay.CustomerMobilePhone, contractToPay.ContractId, contractToPay.FinalPrice ?? 0m);
        //}

        public Dictionary<string, dynamic> CheckOutRequest(string CustomerMobilePhone, string ContractId, decimal FinalPrice)
        {
            //string customerMobile = Request.Cookies["userInfo"].Value.Split('=')[1];
            string customerMobile = CustomerMobilePhone;
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
              "&amount=" + String.Format("{0:0.00}", FinalPrice) +
                //"&currency=" + ConfigurationManager.AppSettings["checkOutcurrancy"] +
                "&currency=" + _storeBase.GetSettingValueByName("checkOutcurrancy") +
                 //"&paymentType=" + ConfigurationManager.AppSettings["checkoutpaymentType"] +
                 "&paymentType=" + _storeBase.GetSettingValueByName("checkoutpaymentType") +
                "&merchantTransactionId=" + ContractId + "#" + DateTime.Now.Ticks.ToString() +
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
            var _responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
            reader.Close();
            dataStream.Close();

            return _responseData;
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

        public PaymentTransaction CreatePaymentTransaction(PaymentTransaction model)
        {
            var paymentModel = new PaymentTransaction();
            paymentModel = _PaymentTransactionStoreBase.Create(model);

            return paymentModel;
        }

        public PaymentTransaction UpdatePaymentTransaction(PaymentTransaction model)
        {
            try
            {
                var paymentModel = new PaymentTransaction();
                paymentModel = _PaymentTransactionStoreBase.Update(model);

                return paymentModel;
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);
                return null;
            }
        }

        public ReceiptVoucherViewModel SaveFailedReceiptVoucher(ServiceContractPerHour contractToPay, PaymentTransaction newTransaction)
        {
            //Insert into failed reciept vouchers only

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
            receipt.VatRate = Convert.ToDecimal(contractToPay.vatrate);
            receipt.Who = 2;
            receipt.IsSaved = false;
            receipt.CreatedDate = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
            receipt.ModifiedDate = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
            receipt.TransactionId = newTransaction.Id;

            _ReceiptVoucherStoreBase.Create(receipt);

            MailSender.SendEmail02(ToEmails, CCEmail, subject, body, false, "");
            return new ReceiptVoucherViewModel();
        }

        public ReceiptVoucher SaveFailedReceiptVoucher(ReceiptVoucherViewModel receiptVoucher, long transactionId)
        {
            var receipt = new ReceiptVoucher();
            receipt.ContractId = receiptVoucher.contractid;
            receipt.CustomerId = receiptVoucher.Customerid;
            receipt.ContractNumber = receiptVoucher.Contractnumber;
            receipt.Amount = Convert.ToDecimal(receiptVoucher.amount);

            CultureInfo info1 = new CultureInfo("en-us");
            receipt.Date = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
            receipt.PaymentCode = "2";//ToDo
            receipt.PaymentType = 2;
            receipt.VatRate = Convert.ToDecimal(receiptVoucher.vatrate);
            receipt.Who = 2;
            receipt.IsSaved = false;
            receipt.CreatedDate = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
            receipt.ModifiedDate = DateTime.Now.ToString("MM/dd/yyyy", info1.DateTimeFormat);
            receipt.TransactionId = transactionId;

            return _ReceiptVoucherStoreBase.Create(receipt);
        }


        public PaymentTransaction AddFailedPaymentTransaction(ServiceContractPerHour contractToPay, string requiredCode, string requiredValue)
        {
            try
            {
                PaymentTransaction transaction = new PaymentTransaction()
                {
                    CustomerId = contractToPay.CustomerId,
                    ContractId = contractToPay.ContractId,
                    PaymentStatus = requiredCode,
                    PaymentStatusName = requiredValue,
                    Amount = Convert.ToDecimal(contractToPay.FinalPrice),
                    Who = 2,
                    EntityName = "Failed Payment Transaction for Contract Number [ " + contractToPay.ContractNum + " ] with Total price of : [ " + contractToPay.FinalPrice + " SR ] and customer : [ " + contractToPay.CustomerId + " ]",
                    IsVoucherSaved = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TransactionType = (int)TransactionType.ServiceContractPerHour,
                    TransactionTypeName = TransactionType.ServiceContractPerHour.ToString()
                };
                transaction = CreatePaymentTransaction(transaction);
                return transaction;
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);
                return null;
            }
        }

        public PaymentTransaction AddSucceededPaymentTransaction(ServiceContractPerHour contractToPay, string requiredCode, string requiredValue)
        {
            try
            {
                PaymentTransaction transaction = new PaymentTransaction()
                {
                    CustomerId = contractToPay.CustomerId,
                    ContractId = contractToPay.ContractId,
                    PaymentStatus = requiredCode,
                    PaymentStatusName = requiredValue,
                    Amount = Convert.ToDecimal(contractToPay.FinalPrice),
                    Who = 2,
                    EntityName = "Successfull Payment Transaction for Contract Number [ " + contractToPay.ContractNum + " ] with Total price of : [ " + contractToPay.FinalPrice + " SR ] and customer : [ " + contractToPay.CustomerId + " ]",
                    IsVoucherSaved = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TransactionType = (int)TransactionType.ServiceContractPerHour,
                    TransactionTypeName = TransactionType.ServiceContractPerHour.ToString()

                };
                transaction = CreatePaymentTransaction(transaction);
                return transaction;
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);
                return null;

            }
        }

        public bool IsPaymentTransactionSucceededWithCheckoutId(string fullCheckoutId)
        {
            return !string.IsNullOrEmpty(fullCheckoutId);
        }

        public PaymentTransaction AddFailedPaymentTransactionWithNoCheckoutId(ServiceContractPerHour contractToPay)
        {
            try
            {
                PaymentTransaction transaction = new PaymentTransaction()
                {
                    CustomerId = contractToPay.CustomerId,
                    ContractId = contractToPay.ContractId,
                    PaymentStatus = "000000",
                    PaymentStatusName = "Checkout payment failed",
                    Amount = Convert.ToDecimal(contractToPay.FinalPrice),
                    Who = 2,
                    EntityName = "Failed Payment Transaction for Contract Number [ " + contractToPay.ContractNum + " ] with Total price of : [ " + contractToPay.FinalPrice + " SR ] and customer : [ " + contractToPay.CustomerId + " ]",
                    IsVoucherSaved = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    TransactionType = (int)TransactionType.ServiceContractPerHour,
                    TransactionTypeName = TransactionType.ServiceContractPerHour.ToString()
                };

                transaction = CreatePaymentTransaction(transaction);

                return transaction;
            }
            catch (Exception ex)
            {
                IExceptionLogger logger = new DefaultExceptionLogger();
                logger.Log("Error", ex);
                return null;

            }
        }


        public HyperPayResponse GetPaymentResponse(string checkoutId)
        {
            if (!IsPaymentTransactionSucceededWithCheckoutId(checkoutId))
                return new HyperPayResponse() { Status = HyperPayStatus.Fail, Reason = "There is not checkout id retrieved for this payment", CheckoutId = null };

            Dictionary<string, dynamic> paymentStatusResult = StatuesRequestRequest(checkoutId);
            string requiredValue = paymentStatusResult["result"]["description"];
            string requiredCode = paymentStatusResult["result"]["code"];//000.100.112 success

            //000.000.000   Live Transaction Success Code
            //Transaction succeeded
            if (requiredCode == "000.000.000" || requiredValue == "Transaction succeeded")
                return new HyperPayResponse() { Status = HyperPayStatus.Success, RequiredValue = requiredValue, RequiredCode = requiredCode, CheckoutId = checkoutId };

            return new HyperPayResponse() { Status = HyperPayStatus.Fail, Reason = requiredValue, RequiredValue = requiredValue, RequiredCode = requiredCode, CheckoutId = checkoutId };

        }
        public void Dispose()
        {
            //<form action = "@Url.Action("Status","Payment", new { lang= lang})" class="paymentWidgets" data-brands="VISA MASTER AMEX"></form>

        }
    }
}