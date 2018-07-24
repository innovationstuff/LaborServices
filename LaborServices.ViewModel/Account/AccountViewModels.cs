using LaborServices.Utility;
using LaborServices.ViewModel.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Resources;


namespace LaborServices.ViewModel
{


    public class ExternalLoginConfirmationViewModel
    {
        //  [Required]
        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        //  [Required]
        [Required(ErrorMessageResourceName = "CodeRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        //  [Required]
        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        //  [Required(ErrorMessage = "phone")]
        [Required(ErrorMessageResourceName = "PhoneRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "phone")]
        public string UserName { get; set; }

        //  [Required]
        [Required(ErrorMessageResourceName = "Password", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceName = "RegisterPhoneIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Mobile Phone")]
        [RegularExpression(AppConstants.MobilePhoneRex, ErrorMessageResourceName = "RegisterPhoneIsNotValed", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string UserName { get; set; }

        //[Display(Name = "Full name")]
        //[Required(ErrorMessageResourceName = "RegisterNameIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string Name { get; set; }

        [Display(Name = "First name")]
        [RegularExpression(@"[^0-9~`!@#$%\^&\*\(\)\-+=\\\|\}\]\{\['&quot;:?.>,</]+", ErrorMessageResourceName = "RegisterFirstNameExpresstion", ErrorMessageResourceType = typeof(ValidationsResources))]
        [MinLength(2, ErrorMessageResourceName = "RegisterFirstNameLength", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Required(ErrorMessageResourceName = "RegisterFirstNameIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string FirstName { get; set; }

        [Display(Name = "Middle name")]
        [RegularExpression(@"[^0-9~`!@#$%\^&\*\(\)\-+=\\\|\}\]\{\['&quot;:?.>,</]+", ErrorMessageResourceName = "RegisterMiddleNameExpresstion", ErrorMessageResourceType = typeof(ValidationsResources))]
        [MinLength(2, ErrorMessageResourceName = "RegisterMiddleNameLength", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Required(ErrorMessageResourceName = "RegisterMiddleNameIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string MiddleName { get; set; }

        [Display(Name = "Last name")]
        [RegularExpression(@"[^0-9~`!@#$%\^&\*\(\)\-+=\\\|\}\]\{\['&quot;:?.>,</]+", ErrorMessageResourceName = "RegisterLastNameExpresstion", ErrorMessageResourceType = typeof(ValidationsResources))]
        [MinLength(2, ErrorMessageResourceName = "RegisterLastNameLength", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Required(ErrorMessageResourceName = "RegisterLastNameIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RegisterPasswordIsRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessageResourceName = "RegisterComparePassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "PhoneRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Phone number")]
        [RegularExpression(AppConstants.MobilePhoneRex, ErrorMessageResourceName = "RegisterPhoneIsNotValed", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceName = "Password", ErrorMessageResourceType = typeof(ValidationsResources))]
        [StringLength(100, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(ValidationsResources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password, ErrorMessageResourceName = "ValidDate", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessageResourceName = "ComparePassword", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ResetPasswordByPhoneViewModel
    {
        [Required(ErrorMessageResourceName = "PhoneRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Phone number")]
        [RegularExpression(AppConstants.MobilePhoneRex, ErrorMessageResourceName = "RegisterPhoneIsNotValed", ErrorMessageResourceType = typeof(ValidationsResources))]

        public string PhoneNumber { get; set; }


        public string Code { get; set; }

        public string UserId { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "PhoneRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Phone Number")]
        [RegularExpression(AppConstants.MobilePhoneRex, ErrorMessageResourceName = "RegisterPhoneIsNotValed", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string PhoneNumber { get; set; }
    }
}

