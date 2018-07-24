using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LaborServices.Entity.Identity;
using LaborServices.Managers.Identity;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LaborServices.Web.Helpers;
using LaborServices.ViewModel;

namespace LaborServices.Web.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Index

        //[SetPermissions(nameAr: "ادارة الحساب", nameEn: "Manage", controller: "Manage", action: "Index", area: null, isBaseParent: false)]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two factor provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? " phone number verified."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId()),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
                Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId())
            };
            return View( model);
        }

        //
        // GET: /Account/AddPhoneNumber
        /// <summary>
        /// @TODO handle crm phoneNumber after update phone through user profile
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Account/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Number = model.Number.RemoveWhiteSpace();
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                //var message = new IdentityMessage
                //{
                //    Destination = model.Number,
                //    Body = "Your security code is: " + code
                //};
                if (_settingManager.IsSMSEnabled())
                {
                    await SendSMSAsync(new IdentityMessage
                    {
                        Body = "use the following code " + code,
                        Destination = model.Number
                    });
                }

                //await UserManager.SmsService.SendAsync(message);
            }
            TempData["AddedPhoneNumber"] = model.Number;
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/RememberBrowser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RememberBrowser()
        {
            var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(User.Identity.GetUserId());
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, rememberBrowserIdentity);
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/ForgetBrowser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetBrowser()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/EnableTFA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTFA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Account/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var sessionPhone = TempData["AddedPhoneNumber"] as string;

            //security check to prevent verify by another phone
            if (phoneNumber == null || sessionPhone != phoneNumber)
                return HttpNotFound();

            phoneNumber = phoneNumber.RemoveWhiteSpace();
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);

            if (_settingManager.IsSMSEnabled())
            {
                ViewBag.Status = " تم ارسال كود التحقق على الجوال";
                await SendSMSAsync(new IdentityMessage
                {
                    Body = "   من فضلك أدخل كود التحقق  من الجوال الأتى  " + code,
                    Destination = phoneNumber
                });
            }
            else
            {
                ViewBag.Status = "  من فضلك أدخل  الكود الاتى للتحقق من الجوال   " + code;
            }
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, UserId = User.Identity.GetUserId() });
        }

        
        [AllowAnonymous]
        public async Task<ActionResult> VerifyPhoneNumberByUserId(string phoneNumber, string userId)
        {
            // This code allows you exercise the flow without actually sending codes

            var sessionPhone = TempData["PhoneNumber"] as string;
            var sessionUser  = TempData["userId"] as string;
            var code  = TempData["code"] as string;

            //security check to prevent verify by another phone
            if (phoneNumber == null || userId == null || sessionPhone  != phoneNumber || sessionUser != userId)
                return HttpNotFound();

            phoneNumber = phoneNumber.RemoveWhiteSpace();
            //var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);

            if (_settingManager.IsSMSEnabled())
            {
                ViewBag.Status = " تم ارسال كود التحقق على الجوال";
                await SendSMSAsync(new IdentityMessage
                {
                    Body =  "   من فضلك أدخل كود التحقق  من الجوال الأتى  " + code,
                    Destination = phoneNumber
                });
            }
            else
            {
                ViewBag.Status = "  من فضلك أدخل  الكود الاتى للتحقق من الجوال   " + code;
            }
            return phoneNumber == null ? View("Error") : View("VerifyPhoneNumber", new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, UserId = userId });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ReGenrateCode(RegenrateCodeViewModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.UserId))
                return Content("هناك خطأ فى اعادة توليد الرمز الجديد");

            model.PhoneNumber = model.PhoneNumber.RemoveWhiteSpace();
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(model.UserId, model.PhoneNumber);

            if (string.IsNullOrEmpty(code))
                return Content("غير قادر على اعادة توليد الرمز الجديد");

            if (_settingManager.IsSMSEnabled())
            {

                await SendSMSAsync(new IdentityMessage
                {
                    Body = "   من فضلك أخل كود التحقق  من الجوال الأتى  " + code,
                    Destination = model.PhoneNumber
                });
                return Content("تم اعادة ارسال الرمز الجديد");
            }
            return Content(string.Format("الرمز الجديد هو  {0}", code));
        }

        //
        // POST: /Account/VerifyPhoneNumber
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isValidCode = await UserManager.VerifyChangePhoneNumberTokenAsync(model.UserId, model.Code, model.PhoneNumber);
            if (isValidCode)
            {
                var user = await UserManager.FindByIdAsync(model.UserId);
                if (user != null)
                {

                    RegisterViewModel userRegistered = Session["userRegistered"] as RegisterViewModel;
                    string userFullName = user.Name;

                    if (userRegistered != null)
                    {
                        userFullName = userRegistered.FirstName.Replace(" ", "") + " " + userRegistered.MiddleName.Replace(" ", "") + " " + userRegistered.LastName.Replace(" ", "");
                    }

                    var crmUserEntity = new ContactViewModel()
                    {
                        ContactId = "0",
                        MobilePhone = user.PhoneNumber,
                        FullName = userFullName,
                        Email = user.Email
                    };

                    var crmUser = await PostResourceAsync<ContactViewModel>("api/contact", crmUserEntity);

                    if (crmUser.StatusCode != HttpStatusCode.OK)
                    {
                        ModelState.AddModelError("", "Failed to verify phone");
                        //await UserManager.DeleteAsync(user);
                        return View(model);
                    }

                    var result = await UserManager.ChangePhoneNumberAsync(model.UserId, model.PhoneNumber, model.Code);
                    if (result.Succeeded)
                    {
                        user.CrmUserId = crmUser.Result.ContactId;
                        await UserManager.UpdateAsync(user);
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index","Home", new { Message = ManageMessageId.AddPhoneSuccess });
                    }
                    return View(model);
                }
                return RedirectToAction("Index", "Home", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Invalid code");
            return View(model);
        }

        //
        // GET: /Account/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

		//  Contracts
		public	ActionResult Contracts()
		{
			return View();
		}

		//Complaints
		public ActionResult Complaints()
		{
			return View();
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            //var newUserIdedntity = await user.GenerateUserIdentityAsync(UserManager);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await UserManager.GenerateUserIdentityAsync(user));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}