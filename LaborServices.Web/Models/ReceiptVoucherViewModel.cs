using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class ReceiptVoucherViewModel
    {
        public string Customerid { get; set; }
        public int paymenttype { get; set; }
        public string amount { get; set; }
        public string datatime { get; set; }
        public string vatrate { get; set; }
        public string contractid { get; set; }
        public string Contractnumber { get; set; }
        public string paymentcode { get; set; }
        public int who { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceId { get; set; }

    }
}