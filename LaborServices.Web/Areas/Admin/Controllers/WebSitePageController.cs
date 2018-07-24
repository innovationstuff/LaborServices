using System.Linq;
using System.Net;
using System.Web.Mvc;
using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Model;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using PagedList;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class WebSitePageController : BaseController
    {
        private readonly WebSitePageStoreBase _webSitePageStoreBase;

        private int _pageSize = 10;

        public WebSitePageController()
        {
            _webSitePageStoreBase = new WebSitePageStoreBase(new LaborServicesDbContext());
        }

        [SetPermissions(nameAr: "محتويات الصفحات", nameEn: "Website pages contents", controller: "WebSitePage", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }

        [SetPermissions(nameAr: "عرض محتويات الصفحات", nameEn: "List of website pages contents", controller: "WebSitePage", action: "List", area: "Admin", isBaseParent: false)]
        public ActionResult List(int? page)
        {
            int pageNumber = page ?? 1;

            int start = (pageNumber - 1) * _pageSize;
            if (start < 0) start = 0;

            var items = _webSitePageStoreBase.EntitySet.ToList();
            var data = items.Skip(start).Take(_pageSize);
            var pagedList = new StaticPagedList<WebSitePage>(data, pageNumber, _pageSize, items.Count());

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = _pageSize;

            return PartialView("_List", pagedList);
        }

        [SetPermissions(nameAr: "تفاصيل المحتوى", nameEn: "Details of content", controller: "WebSitePage", action: "info", area: "Admin", isBaseParent: false)]
        public ActionResult Info(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            WebSitePage WebSitePage = _webSitePageStoreBase.GetById((int)id);

            if (WebSitePage == null)
            {
                return HttpNotFound();
            }

            return View("_Info", WebSitePage);
        }

        [SetPermissions(nameAr: "إنشاء محتوى جديد", nameEn: "Create new content", controller: "WebSitePage", action: "Create", area: "Admin", isBaseParent: false)]
        public ActionResult Create()
        {
            return View("_Create");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebSitePage entity)
        {
            if (!ModelState.IsValid) return View("_Create", entity);

            entity.Slug = entity.TitleEn.GenerateSlug();
            WebSitePage addedEntity = _webSitePageStoreBase.Create(entity);

            if (addedEntity != null)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "something went wrong");
            return View("_Create", entity);
        }

        [SetPermissions(nameAr: "تعديل محتوى", nameEn: "Edit of content", controller: "WebSitePage", action: "Edit", area: "Admin", isBaseParent: false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            WebSitePage entity = _webSitePageStoreBase.GetById((int)id);

            if (entity == null)
            {
                return HttpNotFound();
            }
            return View("_Edit", entity);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WebSitePage entity)
        {
            if (!ModelState.IsValid) return PartialView("_Edit", entity);
            WebSitePage editedEntity = _webSitePageStoreBase.Update(entity);
            if (editedEntity != null)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "something went wrong");
            return View("_Edit", entity);
        }

        [SetPermissions(nameAr: "حذف المحتوى", nameEn: "Delete of content", controller: "WebSitePage", action: "Delete", area: "Admin", isBaseParent: false)]
        public ActionResult Delete(int id)
        {
            var viewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "WebSitePage",
                PostDeleteAction = "DeleteConfirmed",
                PostDeleteController = "WebSitePage"
            };
            return PartialView("_DeleteConfirmation", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DeleteConfirmationViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView("_DeleteConfirmation", viewModel);
            var entity = _webSitePageStoreBase.GetById(viewModel.DeleteEntityId);
            if (entity == null) return Json(new { success = true });
            _webSitePageStoreBase.Delete(entity);
            return Json(new { success = true });
        }
    }
}