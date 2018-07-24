using System.Configuration;

namespace LaborServices.Web.Helpers
{
    public static class SharedClass
    {
        public static string ApiServerUrl => ConfigurationManager.AppSettings["APIServerUrl"];
        public static string EmployeeImagesUrl => ConfigurationManager.AppSettings["EmployeeImagesUrl"];
    }
}
