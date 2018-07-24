using LaborServices.Utility;
using LaborServices.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LaborServices.Web.Managers
{
    public class OfflineDomesticInvoicePaymentManager : DomesticInvoicePaymentManager
    {
        public OfflineDomesticInvoicePaymentManager(Language lang) : base(lang, null)
        {

        }


        public override  Task<DomesticInvoice> GetDomesticInvoice(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var caller = new ApiCaller(Lang);
            var apiUrl = string.Format("api/DomesticInvoice/{0}", id);

            return caller.GetResourceAsync<DomesticInvoice>(apiUrl);
        }

    }
}