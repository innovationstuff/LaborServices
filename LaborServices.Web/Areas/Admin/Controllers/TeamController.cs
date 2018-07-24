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
	[Authorize]
	public class TeamController : BaseController
	{

		private readonly TeamStoreBase _teamStoreBase;
		public TeamController()
		{
			_teamStoreBase = new TeamStoreBase(new LaborServicesDbContext());
		}

		[ActionName("Index")]
		public ActionResult Team()
		{
			return View();
		}
		public PartialViewResult Create()
		{
			return PartialView("_Create");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Create(Team mdl)
		{
			mdl.Id = 1;
			if (ModelState.IsValid)
			{
				var model = _teamStoreBase.Create(mdl);
			}
			return PartialView("_Create");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult Delete(int? Id)
		{
			if (_teamStoreBase.Delete(_teamStoreBase.GetById(Id)))
			{
				return Json(Id);
			}
			return Json(Id);
		}

		public PartialViewResult List()
		{
			return PartialView("_List", _teamStoreBase.EntitySet.ToList());
		}
		public PartialViewResult Edit(int? Id)
		{
			return PartialView("_Edit", _teamStoreBase.GetById(Id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Edit(Team team)
		{
			if (ModelState.IsValid)
			{
				var mdl = _teamStoreBase.GetById(team.Id);
				if (mdl != null)
				{
					//Mapper.Map<About, About>(about);
					mdl.DescriptionAr = team.DescriptionAr;
					mdl.DescriptionEn = team.DescriptionEn;
					mdl.NameAr= team.NameAr;
					mdl.NameEn = team.NameEn;
					_teamStoreBase.Update(mdl);
					return PartialView("_Edit");
				}
			}
			return PartialView("_Edit", team);
		}

	}
}