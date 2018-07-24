using LaborServices.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        // GET: Employee
        public async Task<ActionResult> Index(string nationalityId, string professionId, string packageId)
        {
            if (string.IsNullOrEmpty(nationalityId) || string.IsNullOrEmpty(professionId) || string.IsNullOrEmpty(packageId))
            {
                return RedirectToAction("Index", "IndividualContract");
            }
            else
            {
                var data = await GetResourceAsync<List<Employee>>("api/IndivContract/GetAvailableEmployees?nationalityId=" + nationalityId + "&professionId=" + professionId);
                return View(data);
            }

           
        }
    }
}