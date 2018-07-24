using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LaborServices.Managers.Identity;
using LaborServices.Model.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using LaborServices.Web.Areas.Admin.Models;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class PagesAdminController : BaseController
    {
        private const int PageSize = 5;

        #region managers
        private ApplicationPageManager _pageManager;
        public ApplicationPageManager PageManager
        {
            get
            {
                return _pageManager ?? new ApplicationPageManager(HttpContext.Request.GetOwinContext());
            }
            private set
            {
                _pageManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        #endregion

        [SetPermissions(nameAr: "القوائم", nameEn: "Menu", controller: "PagesAdmin", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }

        [SetPermissions(nameAr: "عرض كل القوائم", nameEn: "List all menus", controller: "PagesAdmin", action: "List", area: "Admin", isBaseParent: false)]
        public ActionResult List(int? page, string keyword)
        {
            int pageNumber = page ?? 1;
            KeyValuePair<int, List<ApplicationPage>> data = PageManager.SearchAllPaging(pageNumber: pageNumber, pageSize: PageSize, keyword: keyword);
            var pagedList = new StaticPagedList<ApplicationPage>(data.Value, pageNumber, PageSize, data.Key);

            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = PageSize;

            return PartialView("_List", pagedList);
        }

        [SetPermissions(nameAr: "عرض كل القوائم الهرمية", nameEn: "List all menus drawer", controller: "PagesAdmin", action: "Drawer", area: "Admin", isBaseParent: false)]
        public ActionResult Drawer()
        {
            return View();
        }


        [SetPermissions(nameAr: "عرض كل القوائم الهرمية", nameEn: "List all menus drawer", controller: "PagesAdmin", action: "ListDrawer", area: "Admin", isBaseParent: false)]
        public ActionResult ListDrawer()
        {
            var allPages = PageManager.Pages
                .Include(x => x.ChildernPages)
                .Include(x => x.ParentPages)
                .Where(x => x.ParentPages.Any() == false).ToList();
            return PartialView("_ListDrawer", allPages);
        }

        //  [SetPermissions(nameAr: AppConstants.AgentIndexAr, nameEn: AppConstants.AgentIndexEn, controller: "Agent", action: "Index", area: "Admin", isBaseParent: false)]

        [SetPermissions(nameAr: "تفاصيل القائمة", nameEn: "details of menu", controller: "PagesAdmin", action: "Details", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Details(long id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationPage applicationPage = await this.PageManager.FindByIdAsync(id);
            if (applicationPage == null)
            {
                return HttpNotFound();
            }

            var pageRoles = this.PageManager.GetPageRoles(applicationPage.ApplicationPageId);
            var roleNames = pageRoles.Select(p => p.Name).ToArray();
            ViewBag.RolesList = roleNames;
            ViewBag.RolesCount = roleNames.Count();
            return View(applicationPage);
        }


        [SetPermissions(nameAr: "انشاء رابط خارجى او تصنيف", nameEn: "create new external link or category", controller: "PagesAdmin", action: "Create", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Create()
        {
            var pagesViewModel = new PagesViewModel();
            var allPages = await this.PageManager.Pages.ToListAsync();

            foreach (var page in allPages)
            {
                var listItem = new SelectListItem()
                {
                    Text = page.NameEn + "|" + page.NameAr,
                    Value = page.ApplicationPageId.ToString(),
                };
                pagesViewModel.ChildernPagesList.Add(listItem);
            }

            return View(pagesViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            PagesViewModel applicationPage,
             long[] childernPages)
        {
            if (ModelState.IsValid)
            {
                applicationPage.Page.Active = true;
                applicationPage.Page.NamesUpdated = true;
                await PageManager.CreatePageAsync(page: applicationPage.Page, parentPages: null, childernPages: childernPages);
                return RedirectToAction("Index");
            }
            return View(applicationPage);
        }


        [SetPermissions(nameAr: "تعديل رابط خارجى او تصنيف", nameEn: "Edit category or external link", controller: "PagesAdmin", action: "Edit", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = await this.PageManager.GetPageWithRelationsAsync(id.Value);

            if (item == null)
            {
                return HttpNotFound();
            }

            var model = new PagesViewModel()
            {
                Page = item
            };

            var allPages = await this.PageManager.Pages.Where(p => p.ApplicationPageId != id).ToListAsync();

            foreach (var p in allPages)
            {
                var listItem = new SelectListItem()
                {
                    Text = p.NameEn + "|" + p.NameAr,
                    Value = p.ApplicationPageId.ToString(),
                    Selected = item.ChildernPages.Any(g => g.ApplicationPageId == p.ApplicationPageId)
                };
                model.ChildernPagesList.Add(listItem);
            }

            //foreach (var p in allPages)
            //{
            //    var listItem = new SelectListItem()
            //    {
            //        Text = p.NameEn + "|" + p.NameAr,
            //        Value = p.ApplicationPageId.ToString(),
            //        Selected = item.ParentPages.Any(g => g.ApplicationPageId == p.ApplicationPageId)
            //    };
            //    model.ParentPagesList.Add(listItem);
            //}

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PagesViewModel applicationPage, long[] childernPages)
        {

            if (!ModelState.IsValid) return View(applicationPage);
            applicationPage.Page.NamesUpdated = true;
            var result = await PageManager.UpdatePageAsync(applicationPage.Page);
            if (result != IdentityResult.Success) return RedirectToAction("Index");
            childernPages = childernPages ?? new long[] { };
            await PageManager.SetPageParentsChildernAsync(applicationPage.Page.ApplicationPageId, childernPages);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> Activate(long id)
        {
            await PageManager.ActivateAsync(id);
            return null;
        }

        [SetPermissions(nameAr: "حذف الرابط او التصنيف", nameEn: "Delete menu link or category", controller: "PagesAdmin", action: "Delete", area: "Admin", isBaseParent: false)]
        public ActionResult Delete(int id)
        {

            //var pageHaveChildern = PageManager.FindById(id);

            //if (pageHaveChildern.ChildernPages.Any())
            //{
            //    ModelState.AddModelError("", " لايمكن حذف ");
            //}

            var viewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Pages",
                PostDeleteAction = "DeleteConfirmed",
                PostDeleteController = "PagesAdmin"
            };

            return PartialView("_DeleteConfirmation", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DeleteConfirmationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = PageManager.FindById(viewModel.DeleteEntityId);
                if (entity == null) return Json(new { success = true });

                PageManager.DeletePage(viewModel.DeleteEntityId);

                return Json(new { success = true });
            }

            return PartialView("_DeleteConfirmation", viewModel);
        }

    }
}