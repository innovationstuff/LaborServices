using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LaborServices.Model.Identity
{
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }


    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
            ApplicationPages = new HashSet<ApplicationPage>();
        }

        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }
        public virtual ICollection<ApplicationPage> ApplicationPages { get; set; }
    }


}
