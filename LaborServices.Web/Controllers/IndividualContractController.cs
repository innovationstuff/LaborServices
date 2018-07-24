using LaborServices.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LaborServices.Web.Controllers
{
    public class IndividualContractController : BaseController
    {
        // GET: IndividualContract
        public async Task<ActionResult>  Index()
        {
            var data = await GetResourceAsync<List<AvailableNumber>>("api/IndivContract/GetAvailableNumbers");
            return View(data);
        
        }
        [HttpGet]
        public async Task<JsonResult> GetIndivPrices(string nationalityId, string professionId)
        {
            var data = await GetResourceAsync<List<IndivPricing>>("api/IndivContract/GetIndivPrices?nationalityId="+nationalityId+ "&professionId=" + professionId);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

    }
}