using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class HyperPayResponse
    {
        public HyperPayStatus Status { get; set; }
        public String Reason { get; set; }
        public String RequiredValue { get; set; }
        public String RequiredCode { get; set; }
        public String CheckoutId { get; set; }


    }

    public enum HyperPayStatus
    {
        Success, Fail
    }
}