using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using LaborServices.Model.Identity;

namespace LaborServices.Managers.Identity
{
    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    //public class ApplicationRoleManager : RoleManager<IdentityRole>
    //{
    //    public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
    //        : base(roleStore)
    //    {
    //    }

    //    public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
    //    {
    //        return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<LaborServicesDbContext>()));
    //    }
    //}

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<LaborServicesDbContext>()));
        }
        public KeyValuePair<int, List<ApplicationRole>> SearchAllPaging(string keyword, int pageSize = 10, int pageNumber = 1)
        {
            int start = ((pageNumber - 1) * pageSize);
            if (start < 0) start = 0;

            var entity = Roles;

            if (string.IsNullOrEmpty(keyword) == false)
            {
                keyword = keyword.ToLower();
                entity = entity.Where(p => p.Name.ToLower().Contains(keyword));
            }
            var filterdItems = entity.ToList();
            return new KeyValuePair<int, List<ApplicationRole>>(filterdItems.Count(), pageSize == 0 ? filterdItems : filterdItems.Skip(start).Take(pageSize).ToList());
        }

    }
}
