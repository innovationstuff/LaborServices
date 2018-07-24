using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Controllers
{
    public class ServiceController : BaseController
    {
        [AllowAnonymous]
        public async Task<JsonResult> GetRegions()
        {
            var data = await GetResourceAsync<dynamic>("api/General/Regions");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetCities()
        {
            var data = await GetResourceAsync<dynamic>("api/City/QuickAll");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetProfessions()
        {
            var data = await GetResourceAsync<dynamic>("api/Professions/QuickAll");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetNationalities()
        {
            var data = await GetResourceAsync<dynamic>("api/Nationality/QuickAll");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetIndividualProfessions()
        {
            var data = await GetResourceAsync<dynamic>("api/Professions/Individual");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetIndividualNationalities()
        {
            var data = await GetResourceAsync<dynamic>("api/Nationality/Individual");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }



        [AllowAnonymous]
        public async Task<JsonResult> GetGenders()
        {
            var data = await GetResourceAsync<dynamic>("api/contact/Genders");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}