using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaborServices.ViewModel.Resources;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace LaborServices.Web.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [Required(ErrorMessageResourceName = "NewPassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessageResourceName = "ConfirmNewPassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        [Required(ErrorMessageResourceName = "Password", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string OldPassword { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [Required(ErrorMessageResourceName = "NewPassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessageResourceName = "ConfirmNewPassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Phone]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessageResourceName = "RegisterPhoneIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Display(Name = "Code")]
        [Required(ErrorMessageResourceName = "CodeRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string Code { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessageResourceName = "RegisterPhoneIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string PhoneNumber { get; set; }

        public string UserId { get; set; }
    }

    public class RegenrateCodeViewModel
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}