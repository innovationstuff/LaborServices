using LaborServices.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Filters
{
    public class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            IExceptionLogger logger = new DefaultExceptionLogger();
            logger.Log("Error", filterContext.Exception);
        }
    }
}