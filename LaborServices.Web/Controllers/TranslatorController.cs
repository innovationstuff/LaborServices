using System.Web.Mvc;
using LaborServices.Web.Helpers;

namespace LaborServices.Web.Controllers
{
    [Authorize]
    public class TranslatorController : Controller
    {
        // GET: Translator
        [SetPermissions(nameAr: "الترجمة", nameEn: "Translations", controller: "Translator", action: "Index", area: null, isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }
    }
}