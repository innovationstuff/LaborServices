using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LaborServices.Entity.Identity;
using LaborServices.Managers.Identity;
using LaborServices.Model.Identity;
using LaborServices.ViewModel;
using LaborServices.Web.Areas.Admin.Models;
using LaborServices.Web.Helpers;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LaborServices.Utility;
using System.Text;
using System;
using System.Globalization;
using System.Configuration;

namespace LaborServices.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #region Properties
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

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        private ApplicationPageManager _pageManager;
        public ApplicationPageManager PageManager
        {
            get
            {
                return _pageManager ?? new ApplicationPageManager(HttpContext.Request.GetOwinContext());
            }
            private set
            {
                _pageManager = value;
            }
        }

        #endregion


        [AllowAnonymous]
        public ActionResult EnableCookies(string returnurl)
        {
            ViewBag.returnurl = returnurl;
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToLocal("/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true

            HttpCookie userInfo = new HttpCookie("userInfo");
            userInfo["userloggedInMobile"] = model.UserName.RemoveWhiteSpace();
            userInfo.Expires.Add(new TimeSpan(30, 0, 0, 0));
            Response.Cookies.Add(userInfo);

            string userMobile = model.UserName.RemoveWhiteSpace();
            ApplicationUser user = UserManager.Users.FirstOrDefault(t => t.PhoneNumber == userMobile);

            var IsPhoneNumberConfirmed = user != null ? UserManager.Users.FirstOrDefault(t => t.PhoneNumber == userMobile).PhoneNumberConfirmed : true;

            var result = await SignInManager.PasswordSignInAsync(model.UserName.RemoveWhiteSpace(), model.Password, model.RememberMe, shouldLockout: false);
            //if (user != null && user.Roles.Count == 0)
            //    await this.UserManager.AddToRoleAsync(user.Id, "User");
            if (IsPhoneNumberConfirmed)
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        string langText = Lang == Language.Arabic ? "ar" : "en";
                        returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" + langText + "/Home/IndividualStart" : returnUrl;
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                    case SignInStatus.Failure:
                    default:
                        if (Lang == Language.Arabic)
                        {
                            ModelState.AddModelError("", "رقم الجوال أو كلمة المرور غير صحيحة");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Mobile or password is incorrect.");
                        }
                        return View(model);
                }
            }
            else
            {
                string langText = Lang == Language.Arabic ? "ar" : "en";
                returnUrl = string.IsNullOrEmpty(returnUrl) ? "/" + langText + "/Home/IndividualStart" : returnUrl;
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());


            if (user != null)
            {
                ViewBag.Status = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider) + "";
            }

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ModelState.RemoveFor<RegisterViewModel>(m => m.Email);
            if (ModelState.IsValid)
            {
               var  userFullName = model.FirstName.Replace(" ", "") + " " + model.MiddleName.Replace(" ", "") + " " + model.LastName.Replace(" ", "");

                //  var user = new ApplicationUser { UserName = model.UserName, Name = model.Name, Email = model.Email, UserType = model.UserType, PhoneNumber = model.PhoneNumber };
                var userEntity = new ApplicationUser { UserName = model.UserName.RemoveWhiteSpace(), Name = userFullName, Email = model.Email, PhoneNumber = model.UserName.RemoveWhiteSpace() ,CreatedOn = DateTime.Now };
                var result = await UserManager.CreateAsync(userEntity, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(userEntity, isPersistent: false, rememberBrowser: false);
                    
                    await this.UserManager.AddToRoleAsync(userEntity.Id, "User");
                    //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    //ViewBag.Link = callbackUrl;
                    //return View("DisplayEmail");

                    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userEntity.Id, model.UserName.RemoveWhiteSpace());

                    string ToEmails = model.Email;
                    string CCEmail = "";
                    string subject = " كود التفعيل الخاص بك علي موقع أبدال ";
                    string body = "<div style='direction:rtl;'>";

                    body += "<p><b>";
                    body += " عزيزي العميل ";
                    body += model.Name;
                    body += "</b></p><br />";

                    body += "<p>";
                    body += " تم فتح سجل خاص بك بنجاح على موقع ايدال التوظيف و الاستقدام و الخدملت العمالية تحت اسم المستخدم : ";
                    body += model.UserName.RemoveWhiteSpace();
                    body += "</p>";

                    body += "<p>";
                    body += " الرجاء اتمام تفعيل سجلك بادخال رقم التفعيل ";
                    body += "<b>";
                    body += code.ToString();
                    body += "</b>";
                    body += " على الرابط التالي ";
                    body += "&nbsp;<a href='https://abdal.sa/ar/Manage/VerifyPhoneNumberByUserId?PhoneNumber="+ model.UserName.RemoveWhiteSpace() + "&userId=" + userEntity.Id + "'>موقع أبدال</a>&nbsp;";
                    body += " و من ثم اتمام المعلوملت الخاصة بك. ";
                    body += "</p>";

                    body += "<p>";
                    body += " شكرا لاختياركم شركة ابدال ";
                    body += "</p>";

                    body += "<p>";
                    body += " ابدال،،،راحة بال،،، ";
                    body += "</p><br />";

                    body += "<p><b>";
                    body += " الرجاء تجاهل الاميل في حال انه وصلكم عن طريق الخطأ ";
                    body += "</b></p>";
                    body += "</div>";

                    try
                    {
                        MailSender.SendEmail02(ToEmails, CCEmail, subject, body, true, "");
                    }
                    catch (Exception ex)
                    {

                    }

                    TempData["PhoneNumber"] = model.UserName.RemoveWhiteSpace();
                    TempData["userId"] = userEntity.Id;
                    TempData["code"] = code;

                    Session["userRegistered"] = model;

                    return RedirectToAction("VerifyPhoneNumberByUserId", "Manage", new { PhoneNumber = model.UserName.RemoveWhiteSpace(), userId = userEntity.Id });
                }
                else
                {
                    AddErrors(result);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> Edit()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var result = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);
            var model = result == null ? new ContactViewModel() : result.ToObject<ContactViewModel>();
            return PartialView("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ContactViewModel model)
        {
            ModelState.RemoveFor<ContactViewModel>(x => model.FullName);

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                model.MobilePhone = model.MobilePhone.RemoveWhiteSpace();
                // save to crm
                var result = await PostResourceAsync<ContactViewModel>("api/contact", model);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //update user in our db
                    user.UserName = model.MobilePhone;
                    user.Name = model.FullName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.MobilePhone;
                    await this.UserManager.UpdateAsync(user);
                    return Json(new { success = true, message = "Info saved successfully" });
                }
                return Json(new { success = false, message = result.StatusMessage });
            }
            return PartialView("Edit", model);
        }

        public async Task<ActionResult> EditProfile()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var result = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);
            var model = result == null ? new ContactViewModel() : result.ToObject<ContactViewModel>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(ContactViewModel model)
        {
            ModelState.RemoveFor<ContactViewModel>(x => model.FullName);

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                model.MobilePhone = model.MobilePhone.RemoveWhiteSpace();
                // save to crm
                var result = await PostResourceAsync<ContactViewModel>("api/contact", model);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //update user in our db
                    user.UserName = model.MobilePhone;
                    user.Name = model.FullName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.MobilePhone;
                    await this.UserManager.UpdateAsync(user);
                    return Json(new { success = true, message = "Info saved successfully" });
                }
                return Json(new { success = false, message = result.StatusMessage });
            }
            return View(model);
        }

        public ActionResult MyContracts()
        {
            return View();
        }

        public async Task<ActionResult> CompleteProfile(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            var result = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);

            var model = result == null ? new ContactViewModel() : result.ToObject<ContactViewModel>();

            model.ContactId = currentUser.CrmUserId;
            //model.FullName = currentUser.Name;
            model.MobilePhone = currentUser.PhoneNumber;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompleteProfile(ContactViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var userloggedInPortal = UserManager.FindById(User.Identity.GetUserId());
            var result = await GetResourceAsync<dynamic>("api/contact/" + userloggedInPortal.CrmUserId);

            ContactViewModel userloggedIn = result == null ? new ContactViewModel() : result.ToObject<ContactViewModel>();

            model.ContactId = userloggedIn.ContactId;
            model.FullName = userloggedIn.FullName;
            model.MobilePhone = userloggedIn.MobilePhone;

            if (ModelState.IsValid)
            {
             var result1 =   await PostResourceAsync<ContactViewModel>("api/contact/UpdateProfile", model);

                if (!string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Finish", "HourlyWorkers");
                return RedirectToAction("Index", "Home"); // or whatever
            }
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.PhoneNumber = model.PhoneNumber.RemoveWhiteSpace();
                var user = await UserManager.FindByNameAsync(model.PhoneNumber);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ModelState.AddModelError("", " هذا المستخدم غير موجود   ");
                    return View("ForgotPassword");
                }

                // var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, model.PhoneNumber);

                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                // ViewBag.Link = callbackUrl;

                ResetPasswordByPhoneViewModel resetModel = new ResetPasswordByPhoneViewModel()
                {
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id
                };

                if (_settingManager.IsSMSEnabled())
                {
                    ViewBag.Status = " تم ارسال كود تعين على الجوال";
                    await SendSMSAsync(new IdentityMessage
                    {
                        Body = "   من فضلك استخدم الكود الأتى فى اعادة تعين  كلمة المرور  " + code,
                        Destination = model.PhoneNumber
                    });
                }
                else
                {
                    ViewBag.Status = "  من فضلك أدخل  الكود الاتى لتعين كلمة المرور   " +  code;
                }

                return View("ForgotPasswordConfirmation", resetModel);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordConfirmation(ResetPasswordByPhoneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.PhoneNumber = model.PhoneNumber.RemoveWhiteSpace();
            var user = await UserManager.FindByNameAsync(model.PhoneNumber);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError("", "هذا المستخدم غير موجود");
                return View("ForgotPassword");
            }

            if (!await UserManager.VerifyChangePhoneNumberTokenAsync(user.Id, model.Code, model.PhoneNumber))
            {
                ModelState.AddModelError("code", " غير قادر على التحقق من الكود");
                return View(model);
            }
            ResetPasswordViewModel resetModel = new ResetPasswordViewModel()
            {
                PhoneNumber = model.PhoneNumber,
                Code = model.Code,
            };
            return View("ResetPassword", resetModel);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ReGenrateCode(RegenrateCodeViewModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
                return Content("هناك خطأ فى اعادة توليد الرمز الجديد");

            model.PhoneNumber = model.PhoneNumber.RemoveWhiteSpace();
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(model.UserId, model.PhoneNumber);

            if (string.IsNullOrEmpty(code))
                return Content("غير قادر على اعادة توليد الرمز الجديد");

            if (_settingManager.IsSMSEnabled())
            {
                await SendSMSAsync(new IdentityMessage
                {
                    Body = "   من فضلك استخدم الكود الأتى فى اعادة تعين  كلمة المرور  " + code,
                    Destination = model.PhoneNumber
                });
                return Content("تم اعادة ارسال الرمز الجديد");
            }
            return Content(string.Format("الرمز الجديد هو  {0}", code));
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.PhoneNumber = model.PhoneNumber.RemoveWhiteSpace();
            var user = await UserManager.FindByNameAsync(model.PhoneNumber);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            if (!await UserManager.VerifyChangePhoneNumberTokenAsync(user.Id, model.Code, model.PhoneNumber))
            {
                ModelState.AddModelError("code", " invalid code resend again");
                return View(model);
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                if (!user.PhoneNumberConfirmed)
                {
                    user.PhoneNumberConfirmed = true;
                    await this.UserManager.UpdateAsync(user);
                }
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            HttpContext.Items["NavbarItems"] = new List<ApplicationPage>();
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Details()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var result = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);
            var model = result == null ? new ContactViewModel() : result.ToObject<ContactViewModel>();
            return View(model);
        }


        //
        // GET: /Account/ChangePassword
        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            return View();
        }


        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                ModelState.AddModelError("User Not Found", "User Not Found");
                return RedirectToAction("ChangePassword", "Account");
            }

            var result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("ChangePassword", "Account");
            }

            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetUserHourlyContracts()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            StringBuilder urlBuilder = new StringBuilder();

            urlBuilder.AppendFormat("api/Profile/HourlyContract/All?userId={0}", currentUser.CrmUserId);

            var result = await GetResourceAsync<List<ServiceContractPerHour>>(urlBuilder.ToString());

            result.All(contract =>
            {
                CultureInfo info = new CultureInfo("en-us");
                contract.CreatedOnText = contract.CreatedOn.ToString("dd/MM/yyyy HH:MM", info.DateTimeFormat);
                return true;
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetUserComplaints()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            StringBuilder urlBuilder = new StringBuilder();

            urlBuilder.AppendFormat("api/CustomerTicket/Dalal/GetTickets?sectorId=4&userId={0}", currentUser.CrmUserId);

            var result = await GetResourceAsync<List<CustomerTicket>>(urlBuilder.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetUserDetails()
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            var result = await GetResourceAsync<dynamic>("api/contact/" + currentUser.CrmUserId);

            var model = result == null ? new ContactViewModel() : result.ToObject<ContactViewModel>();
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult CheckPhoneNumber(string phonenumber)
        {
            var currentUser = UserManager.FindByName(phonenumber);
            if (currentUser == null)
            {
                return Json(new { valid = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { valid = false }, JsonRequestBehavior.AllowGet);
            }
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.IndexOf("is already taken") > -1)
                {
                    if (Lang == Language.Arabic)
                    {
                        ModelState.AddModelError("", "رقم الجوال مستخدم من قبل");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mobile is already taken before.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            //var prevUserId = User.Identity.GetUserId();
            //var userId = prevUserId ?? SignInManager.AuthenticationManager.AuthenticationResponseGrant.Identity.GetUserId();

            if (Url.IsLocalUrl(returnUrl) && string.IsNullOrEmpty(returnUrl) == false && returnUrl.ToLower() != "/")
            {
                return Redirect(returnUrl);
            }

            if(User != null && User.Identity != null && User.IsInRole(AppConstants.AdminRoleName))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return RedirectToAction("Index", "HourlyWorkers");
        }



        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }


        [AllowAnonymous]
        public async Task<JsonResult> GetHourlyContractStatus()
        {
            var data = await GetResourceAsync<dynamic>("api/Profile/Options/HourlyContractStatus");
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}