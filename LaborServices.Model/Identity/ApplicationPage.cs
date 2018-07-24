using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaborServices.Model.Identity
{
    public class ApplicationPage
    {
        public ApplicationPage()
        {
            this.ApplicationRoles = new HashSet<ApplicationRole>();
            this.ChildernPages = new HashSet<ApplicationPage>();
            this.ParentPages = new HashSet<ApplicationPage>();
        }

        public ApplicationPage(string nameAr, string nameEn)
                : this()
        {
            this.NameAr = nameAr;
            this.NameEn = nameEn;
        }

        public ApplicationPage(string nameAr, string nameEn, string controller, string action, string pageUrl)
                : this(nameAr, nameEn)
        {
            this.Controller = controller;
            this.Action = action;
            this.PageUrl = pageUrl;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long ApplicationPageId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string ImageClass { get; set; }
        public string PageUrl { get; set; }
        public string LinkTarget { get; set; }
        public bool IsBaseParent { get; set; }
        public string NoteAr { get; set; }
        public string NoteEn { get; set; }
        public bool NamesUpdated { get; set; }
        public int Order { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }
        public virtual ICollection<ApplicationPage> ChildernPages { get; set; }
        public virtual ICollection<ApplicationPage> ParentPages { get; set; }
    }

    public class ApplicationPageEqualityComparer : IEqualityComparer<ApplicationPage>
    {
        public bool Equals(ApplicationPage x, ApplicationPage y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.NameAr == y.NameAr &&
                   x.NameEn == y.NameEn &&
                   x.Controller == y.Controller &&
                   x.Action == y.Action &&
                   x.Area == y.Area &&
                   x.IsBaseParent == y.IsBaseParent;
        }

        public int GetHashCode(ApplicationPage obj)
        {
            unchecked
            {
                if (obj == null)
                    return 0;
                int hashCode = obj.ApplicationPageId.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.ApplicationPageId.GetHashCode();
                return hashCode;
            }
        }
    }

}
