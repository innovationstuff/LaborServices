using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LaborServices.Entity;
using LaborServices.Entity.Identity;
using LaborServices.Managers.Identity;
using LaborServices.Model.Identity;
using LaborServices.Utility;
using LaborServices.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LaborServices.Web.Controllers
{

    public class SetupController : Controller
    {

        public SetupController()
        {

        }

        public SetupController(ApplicationUserManager userManager, ApplicationRoleManager roleManager, ApplicationGroupManager groupManager, ApplicationPageManager pageManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            GroupManager = groupManager;
            PageManager = pageManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
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


        private ApplicationGroupManager _groupManager;
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager(HttpContext.GetOwinContext());
            }
            private set
            {
                _groupManager = value;
            }
        }

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

        // GET: Setup
        public ActionResult DefaultUsers()
        {
            var user = UserManager.FindByName(AppConstants.DefaultUserName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = AppConstants.DefaultUserName,
                    Email = AppConstants.DefaultUserEmail,
                    EmailConfirmed = true,
                    PhoneNumber = AppConstants.MobilePhone,
                    PhoneNumberConfirmed = true
                };
                var result = UserManager.Create(user, AppConstants.DefaultPass);
                result = UserManager.SetLockoutEnabled(user.Id, false);
            }

            var adminRole = RoleManager.FindByName(AppConstants.AdminRoleName);
            if (adminRole == null)
            {
                adminRole = new ApplicationRole(AppConstants.AdminRoleName);
                var roleresult = RoleManager.Create(adminRole);
            }

            var group = GroupManager.FindByName(AppConstants.SuperAdminsGroup);

            if (group == null)
            {
                group = new ApplicationGroup(AppConstants.SuperAdminsGroup, "Full Access to All");
                GroupManager.CreateGroup(group);
            }

            var userBlongToGroup = GroupManager.IsUserBlongToGroup(user.Id, group.Id);
            if (userBlongToGroup == false)
            {
                GroupManager.SetUserGroups(user.Id, new string[] { group.Id });
            }

            var groupHaveRole = GroupManager.IsGroupHasRole(group.Id, adminRole.Id);
            if (groupHaveRole == false)
            {
                GroupManager.SetGroupRoles(group.Id, new string[] { adminRole.Id });
            }
           return Content("default Users , Groups  and Roles Creation success");
        }

        public async Task<ActionResult> Permissions()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            //get pages that have SetPermissionsAttribute  and have public actions
            var allPages = asm.GetTypes()
                                 .Where(type => typeof(Controller).IsAssignableFrom(type)) //filter controllers
                                 .SelectMany(type => type.GetMethods())
                                 .Where(method => method.IsPublic && method.IsDefined(typeof(SetPermissionsAttribute)))
                                 .Select(m => new ApplicationPage()
                                 {
                                     NameAr = string.IsNullOrEmpty(m.GetCustomAttribute<SetPermissionsAttribute>().NameAr) ?
                                                 m.Name :
                                                 m.GetCustomAttribute<SetPermissionsAttribute>().NameAr,
                                     NameEn = string.IsNullOrEmpty(m.GetCustomAttribute<SetPermissionsAttribute>().NameEn) ?
                                                 m.Name :
                                                 m.GetCustomAttribute<SetPermissionsAttribute>().NameEn,
                                     Controller = string.IsNullOrEmpty(m.GetCustomAttribute<SetPermissionsAttribute>().Controller) ?
                                                 m.Name :
                                                 m.GetCustomAttribute<SetPermissionsAttribute>().Controller,
                                     Action = string.IsNullOrEmpty(m.GetCustomAttribute<SetPermissionsAttribute>().Action) ?
                                                 m.Name :
                                                 m.GetCustomAttribute<SetPermissionsAttribute>().Action,

                                     Area = m.GetCustomAttribute<SetPermissionsAttribute>().Area,
                                     IsBaseParent = false,
                                     NamesUpdated = false
                                 }).ToList();

            await AddMenus(allPages);
            return Content("Menu and Pages Creation Success");
        }

        /// <summary>
        /// @TODo make sure that menu items or pages not repeated also make sure that if user changed the name of exisiting page (NamesUpdated = true) to exlude from insertion
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        public async Task AddMenus(List<ApplicationPage> pages)
        {
            try
            {
                using (var context = new LaborServicesDbContext())
                {
                    var allPages = context.ApplicationPages.ToList();


                    var pageList = (from p in allPages
                                    select new ApplicationPage()
                                    {
                                        ApplicationPageId = p.ApplicationPageId,
                                        NameAr = p.NameAr,
                                        NameEn = p.NameEn,
                                        Controller = p.Controller,
                                        Action = p.Action,
                                        Area = p.Area,
                                        IsBaseParent = p.IsBaseParent,
                                        NamesUpdated = p.NamesUpdated
                                    }).ToList();

                    var deletedIds = pageList.Where(o => string.IsNullOrEmpty(o.Controller) == false && !pages.Any(n => n.Controller == o.Controller && n.Action == o.Action && n.Area == o.Area)).Select(x => x.ApplicationPageId).ToList();
                    var newlyAdded = pages.Where(n => !pageList.Any(o => n.Controller == o.Controller && n.Action == o.Action && n.Area == o.Area)).ToList();

                    var newItems = pageList.Any() == false
                        ? pages
                        : newlyAdded;



                    if (deletedIds.Any())
                    {
                        await PageManager.DeletePagesAsync(deletedIds);
                    }

                    if (newItems.Any() == false) return;

                    foreach (var page in newItems)
                    {
                        context.ApplicationPages.Add(page);
                    }


                    var pagesAdded = await context.SaveChangesAsync();

                    if (pagesAdded <= 0) return;
                    var defaultRole = await context.Roles
                        .FirstOrDefaultAsync(r => r.Name == AppConstants.AdminRoleName);

                    await PageManager.SetRolePagesAsync(defaultRole.Id, newItems.Select(x => x.ApplicationPageId).ToArray());
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}