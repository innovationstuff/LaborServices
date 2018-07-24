using LaborServices.Entity;
using LaborServices.Managers;
using LaborServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    public class BranchesController : BaseController
    {

		private readonly BrancheStoreBase _brancheStoreBase;
		public BranchesController()
		{
			_brancheStoreBase = new BrancheStoreBase(new LaborServicesDbContext());
		}

		public ActionResult Index()
        {
            return View();
        }
		public PartialViewResult Create()
		{
			return PartialView("_Create");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Create(Branche mdl)
		{
			mdl.Id = 1;
			if (ModelState.IsValid)
			{
				var model = _brancheStoreBase.Create(mdl);
			}
			return PartialView("_Create");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult Delete(int? Id)
		{
			if (_brancheStoreBase.Delete(_brancheStoreBase.GetById(Id)))
			{
				return Json(Id);
			}
			return Json(Id);
		}

		public PartialViewResult List()
		{
			return PartialView("_List", _brancheStoreBase.EntitySet.ToList());
		}
		public PartialViewResult Edit(int? Id)
		{
			return PartialView("_Edit", _brancheStoreBase.GetById(Id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Edit(Branche branche)
		{
			if (ModelState.IsValid)
			{
				var mdl = _brancheStoreBase.GetById(branche.Id);
				if (mdl != null)
				{
					//Mapper.Map<About, About>(about);

					mdl.NameAr  = branche.NameAr;
					mdl.NameEn = branche.NameEn;
					mdl.LocationAr = branche.LocationAr;
					mdl.LocationEn = branche.LocationEn;
					mdl.MapLink = branche.MapLink;
					mdl.PhoneNumber = branche.PhoneNumber;

					mdl.FaceBookLink = branche.FaceBookLink;
					mdl.InstagramLink = branche.InstagramLink;
					mdl.TwitterLink = branche.TwitterLink;

					_brancheStoreBase.Update(mdl);
					return PartialView("_Edit");
				}
			}
			return PartialView("_Edit", branche);
		}
    }
}