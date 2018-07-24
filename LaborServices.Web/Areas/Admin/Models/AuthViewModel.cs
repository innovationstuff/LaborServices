using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using LaborServices.Model.Identity;
using LaborServices.Utility;
using LaborServices.Web.Models;

namespace LaborServices.Web.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public RoleViewModel()
        {
            this.PageList = new List<SelectListItem>();
        }

        public ApplicationRole Role { get; set; }
        public ICollection<SelectListItem> PageList { get; set; }
        public ICollection<PageGroupsViewModel> PageGroupsViewModels { get; set; }
    }



    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            this.RolesList = new List<SelectListItem>();
            this.GroupsList = new List<SelectListItem>();
        }
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Phone Number")]
        [RegularExpression(AppConstants.MobilePhoneRex, ErrorMessage = "Entered Mobile  is not valid.")]
        public string UserName { get; set; }


        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Customer Type")]
        public byte UserType { get; set; }

        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        // We will still use this, so leave it here:
        public ICollection<SelectListItem> RolesList { get; set; }

        // Add a GroupsList Property:
        public ICollection<SelectListItem> GroupsList { get; set; }
    }

    public class GroupViewModel
    {
        public GroupViewModel()
        {
            this.UsersList = new List<SelectListItem>();
            this.RolesList = new List<SelectListItem>();
        }

        public ApplicationGroup Group { get; set; }
        public ICollection<SelectListItem> UsersList { get; set; }
        public ICollection<SelectListItem> RolesList { get; set; }
    }

    public class PagesViewModel
    {
        public PagesViewModel()
        {
            this.ChildernPagesList = new List<SelectListItem>();
            this.ParentPagesList = new List<SelectListItem>();
        }

        public ApplicationPage Page { get; set; }

        public ICollection<SelectListItem> ChildernPagesList { get; set; }
        public ICollection<SelectListItem> ParentPagesList { get; set; }
    }
}
