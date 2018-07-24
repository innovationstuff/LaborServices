using System;
using System.IO;
using PagedList;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Model;
using LaborServices.Utility;
using LaborServices.Web.Models;
using LaborServices.Web.Helpers;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class SliderController : BaseController
    {
        private readonly SliderStoreBase _sliderStoreBase;

        private int _pageSize = 10;

        public SliderController()
        {
            _sliderStoreBase = new SliderStoreBase(new LaborServicesDbContext());
        }

        [SetPermissions(nameAr: "شرائح العرض", nameEn: "SliderShow", controller: "Slider", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }

        [SetPermissions(nameAr: "عرض كل شرائح العرض", nameEn: "List all sliderShow", controller: "Slider", action: "List", area: "Admin", isBaseParent: false)]
        public ActionResult List(int? page)
        {
            int pageNumber = page ?? 1;

            int start = (pageNumber - 1) * _pageSize;
            if (start < 0) start = 0;

            var items = _sliderStoreBase.EntitySet.ToList();
            var data = items.Skip(start).Take(_pageSize);
            var pagedList = new StaticPagedList<Slider>(data, pageNumber, _pageSize, items.Count());

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = _pageSize;

            return PartialView("_List", pagedList);
        }

        [SetPermissions(nameAr: "تفاصيل شريحة العرض", nameEn: "Details of sliderShow", controller: "Slider", action: "Info", area: "Admin", isBaseParent: false)]
        public ActionResult Info(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Slider entity = _sliderStoreBase.GetById((int)id);

            if (entity == null)
            {
                return HttpNotFound();
            }

            return View("_Info", new SliderViewModel() { Slider = entity });
        }

        [SetPermissions(nameAr: "إنشاء شريحة عرض جديدة", nameEn: "Create new sliderShow", controller: "Slider", action: "Create", area: "Admin", isBaseParent: false)]
        public ActionResult Create()
        {
            return View("_Create");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SliderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Slider addedEntity = _sliderStoreBase.Create(viewModel.Slider);

                if (addedEntity != null)
                {
                    var savedImage = SaveImage(viewModel, true);

                    if (string.IsNullOrEmpty(savedImage) == false)
                    {
                        addedEntity.ImageName = savedImage;
                        _sliderStoreBase.Update(addedEntity);
                    }
                    return Json(new { success = true, createAnother = Request.Form.AllKeys.Contains("CreateAnother") && Request.Form["CreateAnother"].Contains("true") });
                }
                else { ModelState.AddModelError("", "something went wrong"); }
            }

            return View("_Create", viewModel);
        }

        [SetPermissions(nameAr: "تعديل شريحة العرض", nameEn: "Edit of sliderShow", controller: "Slider", action: "Edit", area: "Admin", isBaseParent: false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Slider entity = _sliderStoreBase.GetById((int)id);

            if (entity == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Edit", new SliderViewModel { Slider = entity });
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SliderViewModel viewModel)
        {
            CheckImageState(viewModel);

            if (ModelState.IsValid)
            {
                Slider editedEntity = _sliderStoreBase.Update(viewModel.Slider);

                if (editedEntity != null)
                {
                    if (Request.IsAjaxRequest())
                    {
                        var savedImage = SaveImage(viewModel, false);

                        if (string.IsNullOrEmpty(savedImage) == false)
                        {
                            editedEntity.ImageName = savedImage;
                            _sliderStoreBase.Update(editedEntity);
                        }

                        return Json(new { success = true });
                    }
                }
                else { ModelState.AddModelError("", "something went wrong"); }
            }

            return PartialView("_Edit", viewModel);
        }

        [SetPermissions(nameAr: "حذف شريحة العرض", nameEn: "Delete of slideshow", controller: "Slider", action: "Delete", area: "Admin", isBaseParent: false)]
        public ActionResult Delete(int id)
        {
            var viewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Slider",
                PostDeleteAction = "DeleteConfirmed",
                PostDeleteController = "Slider"
            };

            return PartialView("_DeleteConfirmation", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DeleteConfirmationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var slider = _sliderStoreBase.GetById(viewModel.DeleteEntityId);
                if (slider == null) return Json(new { success = true });

                string imageName = slider.ImageName;

                var deleted = _sliderStoreBase.Delete(slider);

                if (deleted)
                    DeleteImage(imageName);

                return Json(new { success = true });
            }

            return PartialView("_DeleteConfirmation", viewModel);
        }



        #region Helpers

        public void CheckImageState(SliderViewModel sliderViewModel)
        {
            string path = string.IsNullOrEmpty(sliderViewModel.SliderImageUrl) ? "" : Server.MapPath(sliderViewModel.SliderImageUrl);

            if (string.IsNullOrEmpty(path) == false && System.IO.File.Exists(path) && sliderViewModel.SliderImage == null)
            {
                ModelState.Remove("SliderImage");
            }
        }

        public string SaveImage(SliderViewModel viewModel, bool isAddOperation)
        {
            try
            {
                if (viewModel.SliderImage == null) return "";

                string ext = Path.GetExtension(viewModel.SliderImage.FileName);
                var fileName = DateTime.Now.ToFileTime();
                string fileNameWithEx = fileName + ext;
                string path = Path.Combine(Server.MapPath(AppConstants.SliderFolder), fileNameWithEx);

                if (isAddOperation == false)
                {
                    string oldFilePath = string.IsNullOrEmpty(viewModel.Slider.ImageName)
                        ? ""
                        : Path.Combine(Server.MapPath(AppConstants.SliderFolder), viewModel.Slider.ImageName);

                    if (string.IsNullOrEmpty(oldFilePath) == false && System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                viewModel.SliderImage.SaveAs(path);
                return fileNameWithEx;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void DeleteImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return;

            string path = Path.Combine(Server.MapPath(AppConstants.SliderFolder), imageName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        #endregion
    }
}