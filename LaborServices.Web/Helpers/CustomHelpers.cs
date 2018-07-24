using LaborServices.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using LaborServices.Utility;
using LaborServices.Web.Models;
using Newtonsoft.Json;

namespace LaborServices.Web.Helpers
{
    public static class CustomHelpers
    {
        public static string IsActive(this HtmlHelper htmlHelper, string controllers)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeController = routeData.Values["controller"].ToString();

            var returnActive = controllers.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Any(c => c.Equals(routeController));

            return returnActive ? "active" : "";
        }

        public static string IsActionActive(this HtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeController = routeData.Values["controller"].ToString();
            var routeAction = routeData.Values["action"].ToString();

            var returnActive = (routeAction == action && routeController == controller);

            return returnActive ? "active" : "";
        }

        public static object GetPropertyValue(this HtmlHelper htmlHelper, Type objectType, string propertyName, dynamic instance)
        {
            PropertyInfo pinfo = objectType.GetProperty(propertyName);
            object value = pinfo.GetValue(instance, null);
            return value;
        }

        public static MvcHtmlString BuildNestedMenu(this HtmlHelper htmlHelper, List<ApplicationPage> items, string id, Language lang = Language.English)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            StringBuilder output = new StringBuilder();
            if (items.Count > 0)
            {
                if (string.IsNullOrEmpty(id))
                    output.Append("<ul>");
                else
                    output.AppendFormat("<ul id='{0}'>", id);

                foreach (ApplicationPage subItem in items)
                {
                    output.Append("<li>");
                    var menuName = lang == Language.English ? subItem.NameEn : subItem.NameAr;
                    var langSegment = lang == Language.English ? "en" : "ar";

                    if (string.IsNullOrEmpty(subItem.Controller))
                        output.AppendFormat("<a href='{0}' target='{1}'>{2}</a>", subItem.PageUrl, subItem.LinkTarget, menuName);
                    else
                    {
                        var url = urlHelper.Action(subItem.Action, subItem.Controller, routeValues: new { lang = langSegment, area = subItem.Area });
                        output.AppendFormat("<a href='{0}'>{1}</a>", url, menuName);
                    }
                    output.Append(BuildNestedMenu(htmlHelper, subItem.ChildernPages.ToList(), null, lang));
                    output.Append("</li>");
                }
                output.Append("</ul>");
            }
            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString BuildNestedMenuFrontEnd(this HtmlHelper htmlHelper, List<ApplicationPage> items, Language lang = Language.English, bool haveParent = false)
        {
            //var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var langSegment = lang == Language.English ? "en" : "ar";



            StringBuilder output = new StringBuilder();
            if (items != null && items.Count > 0)
            {
                foreach (ApplicationPage subItem in items)
                {
                    if (subItem.Order != 5000)
                    {
                        if (subItem.Area == "Admin") continue;
                        var menuName = lang == Language.English ? subItem.NameEn : subItem.NameAr;
                        var css = subItem.ImageClass == null ? "fa fa-circle" : subItem.ImageClass;
                        if (subItem.ChildernPages.Any())
                        {

                            string dropDownId = Guid.NewGuid().ToString("N");
                            output.Append(" <span class='dropdown-item subMenu'>");
                            output.AppendFormat("<i class='fa fa-list' id='{0}'>", dropDownId);
                            output.AppendFormat("</i>{0}", menuName);
                            output.Append("<i class='fa fa-angle-down'></i></span><div class='list'>");
                            output.Append(BuildNestedMenuFrontEnd(htmlHelper, subItem.ChildernPages.ToList(), lang, true));
                            output.Append("</div>");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(subItem.Controller))
                            {
                                output.AppendFormat("<a class='dropdown-item' href='{0}' target='{1}'><i class='{2}'></i> {3}</a>", subItem.PageUrl, subItem.LinkTarget, css, menuName);
                            }
                            else
                            {
                                var url = urlHelper.Action(subItem.Action, subItem.Controller, routeValues: new { lang = langSegment, area = subItem.Area });
                                output.AppendFormat("<a class='dropdown-item' href='{0}' target='{1}'><i class='{2}'></i> {3}</a>", url, subItem.LinkTarget, css, menuName);
                            }

                            //string linkClass = haveParent ? "dropdown-item" : "nav-link";
                            //if (!haveParent)
                            //{
                            //    output.Append("<li class='nav-item'>");
                            //}

                            //if (string.IsNullOrEmpty(subItem.Controller))
                            //{
                            //    output.AppendFormat("<a href='{0}' target='{1}' class='{2}'>{3}</a>", subItem.PageUrl, subItem.LinkTarget, linkClass, menuName);
                            //}
                            //else
                            //{
                            //    output.Append(htmlHelper.ActionLink(menuName, subItem.Action, subItem.Controller, new { area = subItem.Area }, new { @class = linkClass }));
                            //}
                            //if (!haveParent)
                            //{
                            //    output.Append("</li>");
                            //}
                        }
                    }
                }
            }
            return new MvcHtmlString(output.ToString());
        }


        public static MvcHtmlString BuildMenuFrontEnd(this HtmlHelper htmlHelper, List<ApplicationPage> items, Language lang = Language.English, bool haveParent = false)
        {
            //var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var langSegment = lang == Language.English ? "en" : "ar";
            var IconDirection = langSegment == "ar" ? "left" : "right";


            StringBuilder output = new StringBuilder();
            if (items != null && items.Count > 0)
            {
                ApplicationPage last = items.Where(a => a.Order == 5000 && a.IsBaseParent == true).LastOrDefault();
                foreach (ApplicationPage subItem in items)
                {
                    if (subItem.Order == 5000)
                    {
                        var url = urlHelper.Action(subItem.Action, subItem.Controller, routeValues: new { lang = langSegment, area = subItem.Area });
                        var active = IsActionActive(htmlHelper, subItem.Controller, subItem.Action);
                        if (subItem.Area == "Admin") continue;
                        var menuName = lang == Language.English ? subItem.NameEn : subItem.NameAr;
                        var css = subItem.ImageClass == null ? "fa fa-circle" : subItem.ImageClass;
                        if (subItem.ChildernPages.Any())
                        {
                            if (items.Where(a => a.Order != 5000).ToList().Count == 0 && last != null && subItem == last)
                            {
                                output.AppendFormat(" <li class='nav-item dropdown {0}' style=\"border-left:  none;\"><a class='nav-link dropdown-toggle' href='#' id='navbarDropdown' role='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'><i class='fa fa-angle-double-{2} d-lg-none d-xl-none'></i>{1}</a><div class='dropdown-menu' aria-labelledby='navbarDropdown'>", active, menuName, IconDirection);
                                output.Append(BuildMenuFrontEnd(htmlHelper, subItem.ChildernPages.ToList(), lang, true));
                                //foreach (ApplicationPage item1 in subItem.ChildernPages)
                                //{
                                //    output.AppendFormat(" <a class='dropdown-item childNav' href='{0}'><i class='{1}'></i> {2} </a>", url, css, menuName);
                                //}


                                output.Append("</div></li>");
                            }
                            else
                            {
                                output.AppendFormat(" <li class='nav-item dropdown {0}'><a class='nav-link dropdown-toggle' href='#' id='navbarDropdown' role='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'><i class='fa fa-angle-double-{2} d-lg-none d-xl-none'></i>{1}</a><div class='dropdown-menu' aria-labelledby='navbarDropdown'>", active, menuName, IconDirection);
                                output.Append(BuildMenuFrontEnd(htmlHelper, subItem.ChildernPages.ToList(), lang, true));
                                //foreach (ApplicationPage item1 in subItem.ChildernPages)
                                //{
                                //    output.AppendFormat(" <a class='dropdown-item childNav' href='{0}'><i class='{1}'></i> {2} </a>", url, css, menuName);
                                //}


                                output.Append("</div></li>");
                            }
                        }
                        else
                        {
                            if (subItem.IsBaseParent)
                            {
                                if (string.IsNullOrEmpty(subItem.Controller))
                                {
                                    if (items.Where(a => a.Order != 5000).ToList().Count == 0 && last != null && subItem == last)
                                    {
                                        output.AppendFormat("<li class='nav-item {0}' style=\"border-left:  none;\">", active);
                                    }
                                    else
                                    {
                                        output.AppendFormat("<li class='nav-item {0}'>", active);
                                    }
                                    output.AppendFormat("<a class='nav-link' href='{0}'>", subItem.PageUrl);
                                    output.AppendFormat("<i class='fa fa-caret-{0} d-xl-none d-lg-none'>", IconDirection);
                                    output.AppendFormat("</i>{0}", menuName);
                                    output.Append("<span class='sr-only'>(current)</span></a></li>");
                                }

                                else
                                {
                                    if (items.Where(a => a.Order != 5000).ToList().Count == 0 && last != null && subItem == last)
                                    {
                                        output.AppendFormat("<li class='nav-item {0}' style=\"border-left:  none;\">", active);
                                    }
                                    else
                                    {
                                        output.AppendFormat("<li class='nav-item {0}'>", active);
                                    }
                                   
                                    output.AppendFormat("<a class='nav-link' href='{0}'>", url);
                                    output.AppendFormat("<i class='fa fa-caret-{0} d-xl-none d-lg-none'>", IconDirection);
                                    output.AppendFormat("</i>{0}", menuName);
                                    output.Append("<span class='sr-only'>(current)</span></a></li>");
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(subItem.Controller))
                                {
                                    if (subItem.NameEn == "Logout")
                                    {
                                        output.Append("<a class=\"nav-link\" href=\"javascript:document.getElementById('logoutForm').submit()\" style=\"border-left:  none;\"><i class=\"fa fa-sign-out\"></i>تسجيل خروج</a>");
                                    }
                                    else
                                    {
                                        output.AppendFormat(" <a class='dropdown-item childNav' href='{0}'><i class='{1}'></i> {2} </a>", subItem.PageUrl, css, menuName);
                                    }
                                }
                                else
                                {
                                    output.AppendFormat(" <a class='dropdown-item childNav' href='{0}'><i class='{1}'></i> {2} </a>", url, css, menuName);

                                }

                            }
                        }
                    }
                }
            }
            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString BuildAdminNestedCheckBoxMenu(this HtmlHelper htmlHelper, List<ApplicationPage> items, string name, long parentId = 0)
        {
            //var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var output = new StringBuilder();
            if (items == null || items.Count <= 0) return new MvcHtmlString(output.ToString());
            output.Append("<ul>");

            foreach (var subItem in items)
            {
                string elementId = RandomString(5);

                output.Append("<li>");
                if (parentId == 0)
                {
                    output.AppendFormat("<input type='checkbox' id='node-{0}' name='{1}' value='{2}'/>", elementId, name, subItem.ApplicationPageId);
                }
                else
                {
                    output.AppendFormat("<input type='checkbox' id='node-{0}' name='{1}' value='{3}_{2}'/>", elementId, name, subItem.ApplicationPageId, parentId);
                }

                output.Append("<label><input type='checkbox' /><span></span></label>");
                output.AppendFormat("<label for='node-{2}' class='treeview-label'>{0}({1})</label>", subItem.NameEn, subItem.NameAr, elementId);
                if (subItem.ChildernPages != null && subItem.ChildernPages.Any())
                {
                    output.Append(BuildAdminNestedCheckBoxMenu(htmlHelper, subItem.ChildernPages.ToList(), name, subItem.ApplicationPageId));
                }
                output.Append("</li>");
            }
            output.Append("</ul>");
            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString EnumToString<TEnum, TEnumType>(this HtmlHelper helper)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnumType>();
            var enumDictionary = values.ToDictionary(value => Enum.GetName(typeof(TEnum), value));

            return new MvcHtmlString(
                JsonConvert.SerializeObject(enumDictionary));
        }

        //public static MvcHtmlString BuildAdminNestedCheckBoxMenu(this HtmlHelper htmlHelper, List<ApplicationPage> items, List<ApplicationRolePages> selectedItems, string name, long parentId = 0)
        //{
        //    //var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
        //    StringBuilder output = new StringBuilder();
        //    if (items.Count() > 0)
        //    {
        //        output.Append("<ul class='treeview'>");

        //        foreach (var subItem in items)
        //        {
        //            string elementId = RandomString(5);

        //            output.Append("<li>");
        //            bool isSelected = false;
        //            if (parentId == 0)
        //            {
        //                isSelected = selectedItems != null && selectedItems.Any(p => p.Pages == subItem.ApplicationPageId.ToString());
        //                var checkedProp = isSelected ? "checked='checked'" : null;
        //                output.AppendFormat("<input type='checkbox' id='node-{0}' name='{1}' value='{2}'  {3}/>", elementId, name, subItem.ApplicationPageId, checkedProp);
        //            }
        //            else
        //            {
        //                var parentList = Traverse(subItem).Reverse().ToList();
        //                var genIds = string.Join("_", parentList.Select(x => x.ApplicationPageId.ToString()));

        //                isSelected = selectedItems != null && selectedItems.Any(p => p.Pages == genIds);
        //                var checkedProp = isSelected ? "checked='checked'" : null;

        //                output.AppendFormat("<input type='checkbox' id='node-{0}' name='{1}' value='{2}' {3} />", elementId, name,  genIds, checkedProp);
        //            }

        //            string checkClass = isSelected ? "custom-checked" : "custom-unchecked";
        //            output.AppendFormat("<label for='node-{2}' class='{3}'>{0}({1})</label>", subItem.NameEn, subItem.NameAr, elementId, checkClass);
        //            output.Append(BuildAdminNestedCheckBoxMenu(htmlHelper, subItem.ChildernPages.ToList(), selectedItems, name, subItem.ApplicationPageId));
        //            output.Append("</li>");
        //        }
        //        output.Append("</ul>");
        //    }
        //    return new MvcHtmlString(output.ToString());
        //}

        public static MvcHtmlString SwitchBox(this HtmlHelper htmlHelper, string name, bool value, object htmlAttributes = null)
        {
            var label = new TagBuilder("label");
            label.AddCssClass("switch");

            var checkbox = new TagBuilder("input");
            checkbox.Attributes.Add("type", "checkbox");
            checkbox.Attributes.Add("name", name);
            checkbox.Attributes.Add("id", name);

            if (value)
            {
                checkbox.Attributes.Add("checked", "checked");
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                checkbox.MergeAttributes(attributes);
            }

            var span = new TagBuilder("span");
            span.AddCssClass("slider round");
            label.InnerHtml += checkbox.ToString();
            label.InnerHtml += span.ToString();

            return new MvcHtmlString(label.ToString());
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static IEnumerable<ApplicationPage> Traverse(ApplicationPage root)
        {
            var stack = new Stack<ApplicationPage>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                if (current.ParentPages == null) continue;
                foreach (var child in current.ParentPages)
                    stack.Push(child);
            }
        }

        public static string CreateApiException(this HttpResponseMessage response)
        {
            var sb = new StringBuilder();

            try
            {
                var ex = new ApiException(response);

                var httpErrorObject = response.Content.ReadAsStringAsync().Result;

                // Create an anonymous object to use as the template for deserialization:
                var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };

                // Deserialize:
                var deserializedErrorObject =
                    JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

                // Now wrap into an exception which best fullfills the needs of your application:

                // Sometimes, there may be Model Errors:
                if (deserializedErrorObject.ModelState != null)
                {
                    var errors = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value));
                    for (int i = 0; i < errors.Count(); i++)
                    {
                        // Wrap the errors up into the base Exception.Data Dictionary:
                        ex.Data.Add(i, errors.ElementAt(i));
                    }
                }
                // Othertimes, there may not be Model Errors:
                else
                {
                    var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);
                    foreach (var kvp in error)
                    {
                        // Wrap the errors up into the base Exception.Data Dictionary:
                        ex.Data.Add(kvp.Key, kvp.Value);
                    }
                }

                foreach (var error in ex.Errors)
                {
                    sb.AppendLine("      " + error);
                }
            }
            catch (Exception)
            {
                sb.AppendLine("Error happend");
            }


            return sb.ToString();
        }

        #region star rating helpers

        public static MvcHtmlString SuperStars(this HtmlHelper htmlHelper, int count, double point, string @class, bool isRating = false, bool isRequired = false)
        {
            if (isRating) @class += " rating ";

            string htmlResult = "<div class='super-stars-container'><span class='super-stars " + @class + "'>";

            if (isRating) htmlResult += "<input type='hidden' name='Rating' " + (isRequired ? "required" : "") + " />";

            int ceilingPoint = (int)Math.Round(point, MidpointRounding.AwayFromZero);

            for (int sc = 1; sc <= count; sc++)
            {
                if (sc <= point)
                {
                    htmlResult += "<i data-index='" + sc + "' class='fa fa-star' aria-hidden='true'></i>";
                }
                else if (ceilingPoint > point && sc <= ceilingPoint)
                {
                    htmlResult += "<i data-index='" + sc + "' class='fa fa-star-half-o' aria-hidden='true'></i>";
                }
                else
                {
                    htmlResult += "<i data-index='" + sc + "' class='fa fa-star-o' aria-hidden='true'></i>";
                }
            }

            htmlResult += "</span></div>";

            return new MvcHtmlString(htmlResult);
        }

        public static MvcHtmlString SuperStars(this HtmlHelper htmlHelper, int count, double point)
        {
            return SuperStars(htmlHelper, count, point, "default");
        }

        public static MvcHtmlString SuperStars(this HtmlHelper htmlHelper, double point)
        {
            return SuperStars(htmlHelper, 5, point);
        }

        public static MvcHtmlString SuperStars(this HtmlHelper htmlHelper, int count, bool isRating)
        {
            return SuperStars(htmlHelper, count, 0, "default", true);
        }

        public static MvcHtmlString SuperStars(this HtmlHelper htmlHelper, int count, bool isRating, bool isRequired)
        {
            return SuperStars(htmlHelper, count, 0, "default", true, isRequired);
        }

        #endregion
    }

}