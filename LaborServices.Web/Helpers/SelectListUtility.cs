using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LaborServices.Entity;
using LaborServices.Utility;

namespace LaborServices.Web.Helpers
{
    public static class SelectListUtility
    {
        private static readonly LaborServicesDbContext Context = new LaborServicesDbContext();


        //public static List<ApplicationPage> NavbarItems { get; set; }

        public static IEnumerable<SelectListItem> GetPages(long? ExcludeApplicationPageId = null)
        {
            var pages = Context.ApplicationPages.AsQueryable();
            if (ExcludeApplicationPageId.HasValue && ExcludeApplicationPageId != null)
            {
                pages = pages.Where(p => p.ApplicationPageId != ExcludeApplicationPageId.Value);
            }
            return new SelectList(pages.ToList(), "ApplicationPageId", "NameAr");
        }

        public static IEnumerable<SelectListItem> GetSortList()
        {
            var selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem() { Text = "Featured", Value = "1000" });
            selectListItems.Add(new SelectListItem() { Text = "High Priority", Value = "500" });
            selectListItems.Add(new SelectListItem() { Text = "Main Menu Page", Value = "5000" });
            Enumerable.Range(1, 200)
                .ToList()
                .ForEach(n => selectListItems.Add(new SelectListItem { Text = n.ToString(), Value = n.ToString() }));

            return selectListItems;
        }
        public static IEnumerable<SelectListItem> GetLinksTargetList()
        {
            var selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem() { Text = "new window or tab", Value = "_blank" });
            selectListItems.Add(new SelectListItem() { Text = "same frame as it was clicked (this is default)", Value = "_self", Selected = true });
            selectListItems.Add(new SelectListItem() { Text = "in the parent frame", Value = "_parent" });
            selectListItems.Add(new SelectListItem() { Text = "in the full body of the window", Value = "_top" });

            return selectListItems;
        }
        public static IEnumerable<SelectListItem> GetUsersTypeList()
        {
            var selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem() { Text = "Individaul", Value = "0" });
            selectListItems.Add(new SelectListItem() { Text = "Business", Value = "1" });
            return selectListItems;
        }

        /// <summary>
        /// Get list of Types used in Enum DataTypes
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSettingType(Language lang= Language.English)
        {
            var selectListItems = Enum.GetValues(typeof(DataTypes)).Cast<DataTypes>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            return selectListItems;
        }


    }
}