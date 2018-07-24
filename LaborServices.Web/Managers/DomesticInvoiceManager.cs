using LaborServices.Entity.Identity;
using LaborServices.Utility;
using LaborServices.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LaborServices.Web.Managers
{
    public class DomesticInvoiceManager
    {
        public Language Lang { get; set; }
        public ApplicationUser User { get; set; }
        public DomesticInvoiceManager(Language lang, ApplicationUser user)
        {
            Lang = lang;
            this.User = user;
        }
        public virtual Task<List<DomesticInvoice>> GetDomesticInvoices(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var caller = new ApiCaller(Lang);
            var apiUrl = string.Format("api/DomesticInvoice/GetUserInvoices/{0}", userId);

            return caller.GetResourceAsync<List<DomesticInvoice>>(apiUrl);
        }

        public virtual Task<DomesticInvoice> GetDomesticInvoiceDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var caller = new ApiCaller(Lang);
            var apiUrl = string.Format("api/DomesticInvoice/{0}", id);

            return caller.GetResourceAsync<DomesticInvoice>(apiUrl);
        }

    }
}