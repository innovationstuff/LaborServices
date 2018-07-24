using System.Web.Mail;
using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using LaborServices.Managers;
using LaborServices.Entity;

namespace LaborServices.Web.Helpers
{

    public class MailSender
    {
        public static string Mail
        {
            get
            {
                ///return ConfigurationManager.AppSettings["CSEmail"].ToString();
                ///

        SettingStoreBase _storeBase;
        _storeBase = new SettingStoreBase(new LaborServicesDbContext());

               return _storeBase.GetSettingValueByName("CSEmail");
            }
        }
        public static string MailPass
        {
            get
            {
                //return GlobalCode.DecryptText(ConfigurationManager.AppSettings["CSPassword"].ToString(), "Ahmed");
               SettingStoreBase _storeBase;
                _storeBase = new SettingStoreBase(new LaborServicesDbContext());
                return GlobalCode.DecryptText(_storeBase.GetSettingValueByName("CSPassword"), "Ahmed");
                
            }
        }
        public static string ExchangeDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["ExchangeDomain"].ToString();
            }
        }
        public static string SMTPServer
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPServer"].ToString();
            }
        }

        public static bool SendEmail(
            string pTo,
            string ccEmails,
            string pSubject,
            string pBody,
            System.Web.Mail.MailFormat pFormat,
            string pAttachmentPath)
        {
            if (pTo.Contains("mawarid") || ccEmails.Contains("mawarid"))
            {
                return false;
            }

            //string   Mail = System.Configuration.ConfigurationManager.AppSettings["CSEmail"];
            //string MailPass = System.Configuration.ConfigurationManager.AppSettings["CSPassword"];
            string exchangeDomain = System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"];
            // pTo = "crm@naas.com.sa";
            //ccEmails = "crm@naas.com.sa";
            try
            {
                System.Web.Mail.MailMessage myMail = new System.Web.Mail.MailMessage();

                NetworkCredential credential = new NetworkCredential(Mail, MailPass, SMTPServer);

                myMail.Fields.Add
                    ("http://schemas.microsoft.com/cdo/configuration/smtpserver",
                                  SMTPServer);
                myMail.Fields.Add
                    ("http://schemas.microsoft.com/cdo/configuration/smtpserverport",
                                  "465");
                myMail.Fields.Add
                    ("http://schemas.microsoft.com/cdo/configuration/sendusing",
                                  "2");
                //sendusing: cdoSendUsingPort, value 2, for sending the message using 
                //the network.

                //smtpauthenticate: Specifies the mechanism used when authenticating 
                //to an SMTP 
                //service over the network. Possible values are:
                //- cdoAnonymous, value 0. Do not authenticate.
                //- cdoBasic, value 1. Use basic clear-text authentication. 
                //When using this option you have to provide the user name and password 
                //through the sendusername and sendpassword fields.
                //- cdoNTLM, value 2. The current process security context is used to 
                // authenticate with the service.
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                //Use 0 for anonymous
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/sendusername",
                    Mail);
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/sendpassword",
                     MailPass);
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/smtpusessl",
                     "true");

                myMail.From = Mail;
                pTo = pTo.Replace(",", ";");
                pTo = pTo.Replace(",", ";");
                myMail.To = pTo;
                myMail.Cc = ccEmails;
                myMail.Subject = pSubject;
                myMail.BodyFormat = pFormat;
                myMail.Body = pBody;
                if (pAttachmentPath.Trim() != "")
                {
                    MailAttachment MyAttachment =
                            new MailAttachment(pAttachmentPath);
                    myMail.Attachments.Add(MyAttachment);
                    myMail.Priority = System.Web.Mail.MailPriority.High;
                }

                System.Web.Mail.SmtpMail.SmtpServer = SMTPServer + ":587";
                System.Web.Mail.SmtpMail.Send(myMail);
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static bool SendEmail02(string pTo, string ccEmails, string pSubject, string pBody, bool isBodyHTML, string pAttachmentPath)
        {
            Attachment attachment = !string.IsNullOrEmpty(pAttachmentPath) ? new Attachment(pAttachmentPath) : null;
            return SendEmail02(pTo, ccEmails, pSubject, pBody, isBodyHTML, attachment);
        }

        public static bool SendEmail02(string pTo, string ccEmails, string pSubject, string pBody, bool isBodyHTML, Stream attachmentStream, string attachmentName)
        {
            Attachment attachment = attachmentStream != null ? new Attachment(attachmentStream, attachmentName) : null;
            return SendEmail02(pTo, ccEmails, pSubject, pBody, isBodyHTML, attachment);
        }

        private static bool SendEmail02(string pTo, string ccEmails, string pSubject, string pBody, bool isBodyHTML, Attachment attachment)
        {
            //string Mail = System.Configuration.ConfigurationManager.AppSettings["CSEmail"];
            SettingStoreBase _storeBase;
            _storeBase = new SettingStoreBase(new LaborServicesDbContext());

            string Mail = _storeBase.GetSettingValueByName("CSEmail");

            string MailPass = GlobalCode.DecryptText(_storeBase.GetSettingValueByName("CSPassword"), "Ahmed");
            string exchangeDomain = System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"];
            // pTo = "crm@mawarid.com.sa";
            //ccEmails = "crm@mawarid.com.sa";
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                NetworkCredential basicCredential = new NetworkCredential(Mail, MailPass, ExchangeDomain);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.IsBodyHtml = isBodyHTML;
                MailAddress fromAddress = new MailAddress(Mail);

                // setup up the host, increase the timeout to 5 minutes
                smtpClient.Host = SMTPServer;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = basicCredential;
                smtpClient.Timeout = (60 * 5 * 1000);
                // smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                message.From = fromAddress;
                message.Subject = pSubject + " - " + DateTime.Now.Date.ToString().Split(' ')[0];
                message.IsBodyHtml = true;
                // message.Body = pBody.Replace("\r\n", "<br>");
                message.Body = pBody;
                string[] to = pTo.Split(';', ',');

                foreach (string mailto in to)
                {
                    if (mailto == "") continue;
                    string mailto02 = mailto.Trim(';').Trim(',');
                    if (!string.IsNullOrEmpty(mailto02) && Validator.EmailIsValid(mailto02))
                        message.To.Add(mailto02);

                }
                string[] ccs = ccEmails.Split(';', ',');
                for (int i = 0; i < ccs.Length; i++)
                {
                    if (!string.IsNullOrEmpty(ccs[i]) && Validator.EmailIsValid(ccs[i]))
                        message.CC.Add(ccs[i]);
                }
                //pTo = pTo.Replace(",", ";");
                //pTo = pTo.Replace(",", ";");
                if (attachment != null)
                {
                    message.Attachments.Add(attachment);
                }
                smtpClient.Send(message);


            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger("FileLogger");
                logger.Error("error", ex);
                throw new ArgumentException(ex.ToString());
            }
            return true;
        }

    }

    public static class Validator
    {

        static Regex ValidEmailRegex = CreateValidEmailRegex();

        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        internal static bool EmailIsValid(string emailAddress)
        {
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);

            return isValid;
        }
    }
}