using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LaborServices.Entity.Identity;
using LaborServices.Managers.Identity;
using LaborServices.Model.Identity;
using LaborServices.Utility;
using LaborServices.ViewModel;
using LaborServices.Web.Areas.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using LaborServices.Web.Helpers;
using System;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class UsersAdminController : BaseController
    {
        private const int PageSize = 5;

        #region constructors
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationGroupManager groupManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            GroupManager = groupManager;
        }
        #endregion

        #region managers
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        private ApplicationGroupManager _groupManager;
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager(HttpContext.Request.GetOwinContext());
            }
            private set
            {
                _groupManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext()
                    .Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion


        [SetPermissions(nameAr: "المستخدمين", nameEn: "Users", controller: "UsersAdmin", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index(string searchString = "")
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.Keyword = searchString;
            }
            return View();
        }

        [SetPermissions(nameAr: "عرض كل المستخدمين", nameEn: "List of users", controller: "UsersAdmin", action: "List", area: "Admin", isBaseParent: false)]
        public ActionResult List(int? page, string keyword)
        {
            int pageNumber = page ?? 1;
            KeyValuePair<int, List<ApplicationUser>> data = UserManager.SearchAllPaging(pageNumber: pageNumber, pageSize: PageSize, keyword: keyword);
            var pagedList = new StaticPagedList<ApplicationUser>(data.Value, pageNumber, PageSize, data.Key);

            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = PageSize;

            return PartialView("_List", pagedList);
        }

        [SetPermissions(nameAr: "تفاصيل المستخدم", nameEn: "Details of user", controller: "UsersAdmin", action: "Details", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            // Show the groups the user belongs to:
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            ViewBag.GroupNames = userGroups.Select(u => u.Name).ToList();
            return View(user);
        }

        [SetPermissions(nameAr: "انشاء مستخدم جديد", nameEn: "Create new user", controller: "UsersAdmin", action: "Create", area: "Admin", isBaseParent: false)]
        public ActionResult Create()
        {
            // Show a list of available groups:
            ViewBag.GroupsList = new SelectList(this.GroupManager.Groups, "Id", "Name");
            ViewBag.RoleList = new SelectList(this.RoleManager.Roles, "Id", "Name");
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel,
            string[] selectedGroups,
            string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                // var user = new ApplicationUser { UserName = userViewModel.UserName,Name = userViewModel.Name, Email = userViewModel.Email, UserType = userViewModel.UserType, PhoneNumber = userViewModel.PhoneNumber };
                var user = new ApplicationUser { UserName = userViewModel.UserName, Name = userViewModel.Name, Email = userViewModel.Email };

                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);


                //Add User to the selected Groups 
                if (adminresult.Succeeded)
                {
                    if (selectedGroups != null)
                    {
                        await this.GroupManager.SetUserGroupsAsync(user.Id, selectedGroups);
                    }

                    if (selectedRoles != null)
                    {
                        await this.GroupManager.SetUserRolesAsync(user.Id, selectedRoles);
                    }
                    return RedirectToAction("Index");
                }
                AddErrors(adminresult);
            }

            ViewBag.GroupsList = new SelectList(this.GroupManager.Groups, "Id", "Name");
            ViewBag.RoleList = new SelectList(this.RoleManager.Roles, "Id", "Name");

            return View(userViewModel);
        }

        [SetPermissions(nameAr: "تعديل مستخدم", nameEn: "Edit of user", controller: "UsersAdmin", action: "Edit", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Display a list of available Groups:
            var allGroups = this.GroupManager.Groups;
            var userGroups = await this.GroupManager.GetUserGroupsAsync(id);
            var userGroupsList = userGroups as List<ApplicationGroup> ?? userGroups.ToList();

            var allRoles = this.RoleManager.Roles.Where(r => r.Name.ToLower() != AppConstants.AdminRoleName);
            var userRoles = await this.UserManager.GetRolesAsync(id);


            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Name = user.Name,
                UserType = user.UserType,
                PhoneNumber = user.PhoneNumber
            };

            foreach (var group in allGroups)
            {
                var listItem = new SelectListItem()
                {
                    Text = group.Name,
                    Value = group.Id,
                    Selected = userGroupsList.Any(g => g.Id == group.Id)
                };
                model.GroupsList.Add(listItem);
            }

            foreach (var role in allRoles)
            {
                var listItem = new SelectListItem()
                {
                    Text = role.Name,
                    Value = role.Id,
                    Selected = userRoles.Any(g => g == role.Name)
                };
                model.RolesList.Add(listItem);
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
             EditUserViewModel editUser,
            string[] selectedGroups,
            string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Update the User:
                user.UserName = editUser.UserName;
                user.Name = editUser.Name;
                user.Email = editUser.Email;
                user.PhoneNumber = editUser.UserName;
                //user.UserType = editUser.UserType;
                //user.PhoneNumber = editUser.PhoneNumber;
                await this.UserManager.UpdateAsync(user);

                // Update the Groups:
                selectedGroups = selectedGroups ?? new string[] { };
                selectedRoles = selectedRoles ?? new string[] { };

                await this.GroupManager.SetUserGroupsAsync(user.Id, selectedGroups);
                await this.GroupManager.SetUserRolesAsync(user.Id, selectedRoles);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View(editUser);
        }

        [SetPermissions(nameAr: "حذف  مستخدم", nameEn: "Delete of user", controller: "UsersAdmin", action: "Delete", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Remove all the User Group references:
                await this.GroupManager.ClearUserGroupsAsync(id);

                // Then Delete the User:
                //var result = await UserManager.DeleteAsync(user);


                // Update the User:

                user.IsDeleted = true;
                user.IsActivated = false;
                var result = this.UserManager.Update(user);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Activate(string id)
        {
            await UserManager.Activate(id);
            return null;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        
    }
}