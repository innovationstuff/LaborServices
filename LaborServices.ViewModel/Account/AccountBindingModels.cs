using LaborServices.ViewModel.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace LaborServices.ViewModel
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        // [Required]
        [Required(ErrorMessageResourceName = "Externalaccesstoken", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        //  [Required]
        [Required(ErrorMessageResourceName = "CurrentPassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "Password", ErrorMessageResourceType = typeof(ValidationsResources))]
        [StringLength(100, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(ValidationsResources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [Compare("NewPassword", ErrorMessageResourceName = "ComparePassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        // [Required]
        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //  [Required]
        // [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Required(ErrorMessageResourceName = "Password", ErrorMessageResourceType = typeof(ValidationsResources))]
        [StringLength(100, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(ValidationsResources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        //   [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Compare("Password", ErrorMessageResourceName = "ComparePassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        //  [Required]
        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        //  [Required]
        [Required(ErrorMessageResourceName = "Loginprovider", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        // [Required]
        [Required(ErrorMessageResourceName = "Providerkey", ErrorMessageResourceType = typeof(ValidationsResources))]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Required(ErrorMessageResourceName = "Password", ErrorMessageResourceType = typeof(ValidationsResources))]
        [StringLength(100, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(ValidationsResources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        //  [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [Compare("NewPassword", ErrorMessageResourceName = "ComparePassword", ErrorMessageResourceType = typeof(ValidationsResources))]
        public string ConfirmPassword { get; set; }
    }
}
