using System.Collections.Generic;
using System.Web.Mvc;

namespace LaborServices.Web.Models
{
    public class PageGroupsViewModel
    {
        public string GroupName { get; set; }
        public ICollection<SelectListItem> PageList { get; set; }
        public int GroupsCount { get; set; }
    }
}