using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaborServices.Web.Models
{
    public class ResultMessageVM
    {
        public string Title  { get; set; }
        public string Message { get; set; }
        public string UrlToRedirect { get; set; }
        public int RedirectTimeout { get; set; }
        public bool IsWithAutoRedirect { get; set; }
        public string Footer { get; set; }
        public string HtmlContent { get; set; }


    }
}