using System.Collections.Generic;
using System.Web;
using LaborServices.Model.Identity;

namespace LaborServices.Web.Helpers
{
    public static class HttpContextExtensions
    {
        public static List<ApplicationPage> GetNavbarItems(this HttpContextBase context)
        {
            return (List<ApplicationPage>)context.Items["NavbarItems"];
        }
    }
}