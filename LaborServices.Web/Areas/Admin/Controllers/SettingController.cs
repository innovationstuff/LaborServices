using System.Linq;
using System.Net;
using System.Web.Mvc;
using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Model;
using LaborServices.Utility;
using PagedList;
using LaborServices.Web.Helpers;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly SettingStoreBase _storeBase;
        private int _pageSize = 10;

        public SettingController()
        {
            _storeBase = new SettingStoreBase(new LaborServicesDbContext());
        }

        [SetPermissions(nameAr: "الاعدادات", nameEn: "Settings", controller: "Setting", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }

        [SetPermissions(nameAr: "عرض كل الاعدادات", nameEn: "List all settings", controller: "Setting", action: "List", area: "Admin", isBaseParent: false)]
        public ActionResult List(int? page)
        {
            int pageNumber = page ?? 1;

            int start = (pageNumber - 1) * _pageSize;
            if (start < 0) start = 0;

            var items = _storeBase.EntitySet.ToList();
            var data = items.Skip(start).Take(_pageSize);
            var pagedList = new StaticPagedList<Setting>(data, pageNumber, _pageSize, items.Count());

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = _pageSize;

            return PartialView("_List", pagedList);
        }

        [SetPermissions(nameAr: "تفاصيل الاعداد", nameEn: "Details of setting", controller: "Setting", action: "Info", area: "Admin", isBaseParent: false)]
        public ActionResult Info(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Setting setting = _storeBase.GetById((int)id);

            if (setting == null)
            {
                return HttpNotFound();
            }
            return View("_Info", setting);
        }

        [SetPermissions(nameAr: "انشاء اعداد جديد", nameEn: "Create new setting", controller: "Setting", action: "Create", area: "Admin", isBaseParent: false)]
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Setting entity)
        {
            if (!ModelState.IsValid) return PartialView("_Create", entity);

            entity.IsEditable = true;
            Setting addedEntity = _storeBase.Create(entity);

            if (addedEntity != null)
            {
                return Json(new { success = true });
            }
            ModelState.AddModelError("", "something went wrong");
            return PartialView("_Create", entity);
        }

        [SetPermissions(nameAr: "تعديل الاعداد", nameEn: "Edit of setting", controller: "Setting", action: "Edit", area: "Admin", isBaseParent: false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Setting entity = _storeBase.GetById((int)id);

            if (entity == null)
            {
                return HttpNotFound();
            }

            string inputType = "text";
            switch (entity.SettingDataType)
            {
                case DataTypes.Integer:
                case DataTypes.Fraction:
                     inputType = "number";
                    break;
                case DataTypes.Boolean:
                    inputType = "checkbox";
                    break;
                case DataTypes.Date:
                    inputType = "Date";
                    break;
                case DataTypes.Password:
                    inputType = "password";
                    break;
            }
            ViewBag.inputType = inputType;
            return PartialView("_Edit", entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Setting entity)
        {
            if (ModelState.IsValid)
            {
                Setting editedEntity = _storeBase.Update(entity);

                if (editedEntity != null)
                {
                    return Json(new { success = true });
                }
                ModelState.AddModelError("", "something went wrong");
            }
            return PartialView("_Edit", entity);
        }

    }
}