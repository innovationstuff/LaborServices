using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LaborServices.Web.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetQueryString(this object obj)
        {
            //var properties = from p in obj.GetType().GetProperties()
            //                 where p.GetValue(obj, null) != null
            //                 select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString()).ToList();

            var paramList = new List<string>();
            foreach (var item in obj.GetType().GetProperties())
            {
                if (item.GetValue(obj, null) == null) continue;
                var par = "";
                par = item.Name + "=";
                if (item.PropertyType.IsArray)
                {
                    foreach (var itemInstance in (IEnumerable)item.GetValue(obj, null))
                    {
                        par += HttpUtility.UrlEncode(itemInstance.ToString()) + ",";
                    }
                    par = par.TrimEnd(',');
                }
                else
                {
                    DateTime value;

                    if (DateTime.TryParse(item.GetValue(obj, null).ToString(), out value))
                    {
                        par += value.ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        par += HttpUtility.UrlEncode(item.GetValue(obj, null).ToString());
                    }
                }
                paramList.Add(par);
            }
            return string.Join("&", paramList.ToArray());
        }

        public static void RemoveFor<TModel>(this ModelStateDictionary modelState,
            Expression<Func<TModel, object>> expression)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);

            foreach (var ms in modelState.ToArray())
            {
                if (ms.Key.StartsWith(expressionText + ".") || ms.Key == expressionText)
                {
                    modelState.Remove(ms);
                }
            }
        }

        public static string RemoveWhiteSpace(this string item)
        {
            if (string.IsNullOrEmpty(item)) return item;
            return Regex.Replace(item, @"\s+", "");
        }

        public static string ToDateTimeString(this DateTime? dateTime)
        {
            return dateTime?.ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("en")) ?? "";
        }

        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("en")) ?? "";
        }

        public static string GenerateSlug(this string phrase, int maxLength = 100)
        {
            string str = phrase.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }
    }
}