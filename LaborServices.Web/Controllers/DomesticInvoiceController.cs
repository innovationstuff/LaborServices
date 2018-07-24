using LaborServices.Entity.Identity;
using LaborServices.Managers.Identity;
using LaborServices.Web.Managers;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Controllers
{
    public class DomesticInvoiceController : BaseController
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

        // GET: DomesticInvoice
        public async Task<ActionResult> Index()
        {
            DomesticInvoiceManager Mgr;
            ApplicationUser u = UserManager.FindById(User.Identity.GetUserId());
            Mgr = new DomesticInvoiceManager(Lang, u);
            if (u.CrmUserId != null)
            {
                List<DomesticInvoice> invoices = await Mgr.GetDomesticInvoices(u.CrmUserId);
                if (invoices == null) return View(new List<DomesticInvoice>());

                return View(invoices);
            }
            else
            {
                return View(new List<DomesticInvoice>());
            }
        }

        public async Task<ActionResult> DomesticInvoice(string id)
        {
            DomesticInvoiceManager Mgr;
            ApplicationUser u = UserManager.FindById(User.Identity.GetUserId());
            Mgr = new DomesticInvoiceManager(Lang, u);
            DomesticInvoice invoice = await Mgr.GetDomesticInvoiceDetails(id);
            if (invoice == null) return HttpNotFound();

            return View(invoice);
        }

    }
}