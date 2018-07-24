using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Model;
using LaborServices.Utility;
using LaborServices.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Areas.Admin.Controllers
{
	[Authorize]
	public class AboutController : BaseController
	{
		private readonly AboutStoreBase _AboutStoreBase;
		public AboutController()
		{
			_AboutStoreBase = new AboutStoreBase(new LaborServicesDbContext());
		}

		#region Actions
		public ActionResult WhoAreWe()
		{
			return View();
		}

		public ActionResult CompanyValues()
		{
			return View();
		}

		public ActionResult WhyAbdal()
		{
			return View();
		}

		public PartialViewResult List(int? Type)
		{
			ViewBag.typ = Type;
			return PartialView("_List", _AboutStoreBase.EntitySet.Where(x => x.Type == Type).ToList());
		}

		public PartialViewResult Create(int? Type)
		{
			ViewBag.Typ = Type;
			return PartialView("_Create");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Create(About mdl, HttpPostedFileBase ImgFile)
		{
			ViewBag.Typ = mdl.Type;
			mdl.Id = 1;
			if (ModelState.IsValid)
			{
				var model = _AboutStoreBase.Create(mdl);
			}
			return PartialView("_Create");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult Delete(int? Id)
		{
			if (_AboutStoreBase.Delete(_AboutStoreBase.GetById(Id)))
			{
				return Json(Id);
			}
			return Json(Id);
		}

		public PartialViewResult Edit(int? Id)
		{
			return PartialView("_Edit", _AboutStoreBase.GetById(Id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Edit(About about)
		{
			if (ModelState.IsValid)
			{
				var mdl = _AboutStoreBase.GetById(about.Id);
				if (mdl != null)
				{
					//Mapper.Map<About, About>(about);
					if (about.ImgUrl != null && (mdl.ImgUrl.Trim() != about.ImgUrl.Trim()))
					{
						string path = AppConstants.aboutFolder + mdl.ImgUrl;
						if (System.IO.File.Exists(path))
						{
							System.IO.File.Delete(path);
						}
						mdl.ImgUrl = about.ImgUrl;
					}
					mdl.DescriptionAr = about.DescriptionAr;
					mdl.DescriptionEN = about.DescriptionEN;
					mdl.TitleAr = about.TitleAr;
					mdl.TitleEN = about.TitleEN;
					_AboutStoreBase.Update(mdl);
					return PartialView("_Edit");
				}
			}
			return PartialView("_Edit");
		}

		#endregion

		[HttpPost]
		public JsonResult SaveImage(string OldImg , string operation)
		{
			if (OldImg.Trim() != "")
			{
				string path = AppConstants.aboutFolder + OldImg;
				if (operation == "0" && System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
			}
			var files = Request.Files;
			string newFileName = "";
			if (files.Count > 0)
			{
				string fileName = DateTime.Now.ToFileTime() + System.IO.Path.GetExtension(files[0].FileName);
				files[0].SaveAs(Server.MapPath(AppConstants.aboutFolder + fileName));
				newFileName = fileName;
			}
			return Json(newFileName);
		}

	}
}