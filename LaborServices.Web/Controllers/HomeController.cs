using System.Linq;
using System.Web.Mvc;
using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Web.Models;

namespace LaborServices.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            using (var context = new LaborServicesDbContext())
            {
                var store = new SliderStoreBase(context);
                var mainSlider = store.DbEntitySet.ToList().Select(s => new SliderViewModel() { Slider = s }).ToList();
                var homeViewModel = new HomeViewModel
                {
                    MainSlider = mainSlider
                };

                return View(homeViewModel);
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult Works()
        {
            return View();
        }

        public ActionResult Missions()
        {
            return View();
        }

        public ActionResult Credentials()
        {
            return View();
        }

        public ActionResult Branches()
        {
            return View();
        }

        public ActionResult Soon()
        {
            return View();
        }

        public ActionResult IndividualStart()
        {
            using (var context = new LaborServicesDbContext())
            {
                var store = new SliderStoreBase(context);
                var mainSlider = store.DbEntitySet.ToList().Select(s => new SliderViewModel() { Slider = s }).ToList();
                var homeViewModel = new HomeViewModel
                {
                    MainSlider = mainSlider
                };

                return View(homeViewModel);
            }
        }
    }
}