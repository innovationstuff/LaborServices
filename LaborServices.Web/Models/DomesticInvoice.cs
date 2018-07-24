using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class DomesticInvoice
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public int? InvoiceTypeKey { get; set; }
        public string InvoiceType { get; set; }
        public string ContractId { get; set; }
        public string Contract { get; set; }
        public string CustomerId { get; set; }
        public string Customer { get; set; }
        public string CustomerMobilePhone { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? CustomerAmount { get; set; }
        public decimal? TotalWithVat { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public bool IsPaid { get; set; }
        public int? InvoiceDaysCount { get; set; }

    }
}