using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LaborServices.Managers.Identity;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;

namespace LaborServices.Web.Controllers
{
    [Authorize]
    public class HourlyWorkersController : BaseController
    {



        [AllowAnonymous]
        // GET: HourlyWorkers
        public ActionResult Index()
        {
            if (Session["HourlyWorkers"] != null)
                Session.Remove("HourlyWorkers");
            return View();
        }

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

        private HourlyWorkersViewModel GetContactDetails(string promotionCode = null)
        {
            if (Session["HourlyWorkers"] == null)
            {
                var model = new HourlyWorkersViewModel();
                model.PromotionCode = promotionCode;
                Session["HourlyWorkers"] = model;
            }
            return (HourlyWorkersViewModel)Session["HourlyWorkers"];
        }

        private void RemoveContactDetails()
        {
            Session.Remove("HourlyWorkers");
        }

        public bool IsNotValidDetails
        {
            get
            {
                var details = GetContactDetails();
                return string.IsNullOrEmpty(details.CityId);
            }
        }

        public  async Task<ActionResult> Create(string promotionCode = null)
        {
           
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/GetLatestNotPaidContract/" + currentUser.CrmUserId);
            if (string.IsNullOrEmpty(data))
            {
                HourlyWorkersViewModel model = GetContactDetails(promotionCode);
                ViewBag.MorningOffset = ConfigurationSettings.AppSettings["MorningOffset"];
                ViewBag.EveningOffset = ConfigurationSettings.AppSettings["EveningOffset"];
                return View(model);
            }
            else
            {
                return RedirectToAction("Details", "HourlyWorkers", new { id = data });
            }



        }

        [HttpPost]
        public async Task<ActionResult> Create(HourlyWorkersViewModel model, string BtnNext)
        {
            if (BtnNext == null) return View(model);
            if (!ModelState.IsValid) return View(model);

            HourlyWorkersViewModel details = GetContactDetails();
            details.CityId = model.CityId;
            details.DistrictId = model.DistrictId;
            details.NumOfWorkers = model.NumOfWorkers;
            details.NumOfVisits = model.NumOfVisits;
            details.NumOfHours = model.NumOfHours;
            details.AvailableDays = model.AvailableDays;
            details.StartDay = model.StartDay;
            details.ContractDuration = model.ContractDuration;
            details.Nationality = model.Nationality;
            details.HourlypricingId = model.HourlypricingId;
            details.IsMorningShift = model.IsMorningShift;
            details.AgreedToTerms = model.AgreedToTerms;
            details.CityName = model.CityName;
            details.DistrictName = model.DistrictName;
            details.NationalityName = model.NationalityName;
            details.NumOfVisitsWritten = model.NumOfVisitsWritten;
            details.ContractDurationWritten = model.ContractDurationWritten;
            details.TotalPrice = model.TotalPrice;
            details.TotalBeforeDiscount = model.TotalBeforeDiscount;

            details.Discount = model.Discount;
            details.VatRate = model.VatRate;
            details.VatAmount = model.VatAmount;
            details.TotalPriceWithVat = model.TotalPriceWithVat;
            details.PromotionCode = model.PromotionCode;
            details.TotalPriceAfterPromotion = model.TotalPriceAfterPromotion;
            details.TotalPromotionDiscountAmount = model.TotalPromotionDiscountAmount;
            details.PromotionExtraVisits = model.PromotionExtraVisits;
            details.PromotionName = model.PromotionName;

            details = await SetTimeTable(details);
            Session["HourlyWorkers"] = details;
            return View("ConfirmTimeTable", details);
        }

        public ActionResult Next(HourlyWorkersViewModel model, string BtnPrevious, string BtnNext)
        {
            HourlyWorkersViewModel details = GetContactDetails();
            if (IsNotValidDetails) return RedirectToAction("Create");

            if (BtnPrevious != null)
            {
                ModelState.Clear();
                return View("Create", details);
            }
            if (BtnNext != null)
            {
                return View("PickLocation", details);
            }
            return View("ConfirmTimeTable", details);
        }

        public ActionResult NextStep(HourlyWorkersViewModel model, string BtnPrevious, string BtnNext)
        {
            HourlyWorkersViewModel details = GetContactDetails();
            if (IsNotValidDetails) return RedirectToAction("Create");

            if (BtnPrevious != null)
            {
                return View("ConfirmTimeTable", details);
            }
            if (BtnNext != null)
            {
                details.Longitude = model.Longitude;
                details.Latitude = model.Latitude;
                details.AddressNotes = model.AddressNotes;
                details.HouseType = model.HouseType;
                details.HouseNo = model.HouseNo;
                details.FloorNo = model.FloorNo;
                details.PartmentNo = model.PartmentNo;

                Session["HourlyWorkers"] = details;
                return View("Finish", details);
            }
            return View("PickLocation", details);
        }

        public async Task<ActionResult> Finish(string BtnPrevious, string BtnNext)
        {
            HourlyWorkersViewModel details = GetContactDetails();
            if (IsNotValidDetails) return RedirectToAction("Create");

            if (BtnPrevious != null)
            {
                return View("PickLocation", details);
            }

            if (!await IsProfileCompleted())
            {
                return RedirectToAction("CompleteProfile", "Account", new { returnUrl = Url.Action("Finish", "HourlyWorkers") });
            }
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var data = new ContractViewModel(details);

            data.CustomerId = currentUser.CrmUserId;
            var newContract = await PostResourceAsync<ContractViewModel>("api/HourlyContract", data);
            if (newContract.StatusCode == HttpStatusCode.OK)
            {
                data.ContractId = newContract.Result.ContractId;
                data.ContractNum = newContract.Result.ContractNum;

                data.PriceBeforeDiscount = (Session["HourlyWorkers"] as HourlyWorkersViewModel).TotalBeforeDiscount;
                data.FinalPrice = newContract.Result.FinalPrice;
                data.VatRate = Math.Round((decimal.Parse(data.FinalPrice) - decimal.Parse(data.PriceBeforeDiscount)) / decimal.Parse(data.PriceBeforeDiscount), 2).ToString();
                data.VatAmount = Math.Round((decimal.Parse(data.FinalPrice) - decimal.Parse(data.PriceBeforeDiscount))).ToString();

                Session["ContractToPayData"] = data;
                RemoveContactDetails();
                return View("Success", newContract.Result);
            }
            return View("Error", details);
        }

        public async Task<ActionResult> UserContracts(string hourlyContractStatus = "")
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            StringBuilder urlBuilder = new StringBuilder();

            urlBuilder.AppendFormat("api/Profile/HourlyContract/All?userId={0}", currentUser.CrmUserId);

            if (!string.IsNullOrEmpty(hourlyContractStatus))
                urlBuilder.AppendFormat("&statusCode={0}", hourlyContractStatus);

            var result = await GetResourceAsync<List<ServiceContractPerHour>>(urlBuilder.ToString());
            return PartialView("_UserContracts", result);
        }

        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            string url = string.Format("api/Profile/HourlyContract/Details/{0}?userId={1}", id, currentUser.CrmUserId);
            var basicDetails = await GetResourceAsync<ServiceContractPerHour>(url);

            if (basicDetails == null) return HttpNotFound();
            string appointmentsUrl = string.Format("api/Profile/HourlyContract/Appointments/{0}?userId={1}", id, currentUser.CrmUserId);
            var appointments = await GetResourceAsync<List<HourlyAppointment>>(appointmentsUrl);

            basicDetails.HourlyAppointments = appointments;

            return View(basicDetails);
        }

        public async Task<ActionResult> Contracts()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var result = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);
            var model = result == null ? new ContactViewModel() : result.ToObject<ContactViewModel>();
            return View(model);
        }



        #region calling API 

        [AllowAnonymous]
        public async Task<JsonResult> GetDistricts(string cityId)
        {
            var data = await GetResourceAsync<dynamic>("api/city/Districts/", cityId);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetNumOfWorkers()
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/options/Labours");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetNumOfVisits()
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/options/Visits");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetAvailableDays(string districtId)
        {
            var data = await GetResourceAsync<dynamic>("api/district/DistrictDays/", districtId);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetNationalities(string districtId)
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/Lookups/DistrictNationalities/", districtId);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetHours()
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/options/Hours");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetContractDuration()
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/options/ContractDuration");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetPackages(MiniHourlyWorkersViewModel entity)
        {
            var parameters = HttpUtility.UrlDecode(entity.GetQueryString());
            var data = await GetResourceAsync<dynamic>("api/HourlyPricing?" + parameters);
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        private async Task<HourlyWorkersViewModel> SetTimeTable(HourlyWorkersViewModel details)
        {
            if (details == null) return new HourlyWorkersViewModel();
            string link = string.Format("api/HourlyPricing/ContractDays?ContractStartDate={0}&NoOfMonths={1}&Days={2}&hours={3}&shift={4}",
                details.StartDay,
                details.ContractDuration + details.PromotionExtraVisits,
                string.Join(",", details.AvailableDays),
                details.NumOfHours, details.IsMorningShift);

            JArray data = await GetResourceAsync<dynamic>(link);

            var t = data.ToObject<string[]>();
            //if (result != null)
            //{
            //    data = JsonConvert.DeserializeObject<string[]>(result);
            //}
            details.ConfirmTimeTableList = t.ToList();
            return details;
        }

        private async Task<bool> IsProfileCompleted()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (string.IsNullOrEmpty(currentUser.CrmUserId))
                return false;

            var isProfileCompleted = await GetResourceAsync<dynamic>("api/contact/IsProfileCompleted/" + currentUser.CrmUserId);
            var result = isProfileCompleted as bool?;
            return result != null && result != false;
        }

        public ActionResult GetTerms()
        {
            return PartialView("GetTerms");
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetHourlyContractStatus()
        {
            var data = await GetResourceAsync<dynamic>("api/Profile/Options/HourlyContractStatus");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetHouseTypes()
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/options/HousingTypes");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetFloors()
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/options/HousingFloors");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public async Task<JsonResult> GetCities()
        {
            var data = await GetResourceAsync<dynamic>("api/HourlyContract/Lookups/AvailableCities");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }





        #endregion
    }
}