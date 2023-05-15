using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InfrastructureCore.Utils
{
    public class SendMail
    {
        private readonly IConfiguration _config;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        public SendMail(IConfiguration config, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _config = config;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }
        public async Task<Result> SendEmailTo(IEnumerable<string> listEmail, string bodyContent, string subject, bool isBodyHtml)
        {
            var result = new Result();
            if (listEmail.Any())
            {
                using (var message = new MailMessage()
                {
                    Subject = subject,
                    Body = bodyContent,
                    IsBodyHtml = isBodyHtml
                })
                {
                    foreach (var toEmail in listEmail)
                    {
                        if (!string.IsNullOrEmpty(toEmail))
                        {
                            message.To.Add(toEmail.Trim());
                        }
                    }
                    var sendResult = await SendMailSmtp(message);

                    if (sendResult)
                    {
                        result.Success = true;
                        result.Message = "Send Email Successfully";
                    }
                    else
                    {
                        result.Message = "Send Email Failed";
                    }
                    return result;
                }
            }
            result.Message = "Don't have email to send";
            return result;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> SendMailSmtp(MailMessage message)
        {
            try
            {
                var emailAccount = new MailAddress(_config.GetValue<string>("MailSettings:EmailAccount"));
                var displayName = _config.GetValue<string>("MailSettings:DisplayName");
                var emailPassword = _config.GetValue<string>("MailSettings:EmailPassword");

                var host = _config.GetValue<string>("MailSettings:Host");
                var port = _config.GetValue<int>("MailSettings:Port");
                var enableSsl = _config.GetValue<bool>("MailSettings:EnableSsl");

                using (var smtp = new SmtpClient
                {
                    Port = port,
                    Host = host,
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(emailAccount.Address, emailPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                })
                {
                    if (message.From == null)
                    {
                        message.From = emailAccount;
                    }
                    await smtp.SendMailAsync(message);
                    return true;
                };

            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Send Email with file
        /// </summary>
        /// <param name="filesPath"></param>
        /// <param name="listEmail"></param>
        /// <param name="bodyContent"></param>
        /// <param name="subject"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        public async Task<Result> SendEmailWithFileTo(List<string> filesPath, IEnumerable<string> listEmail, string bodyContent, string subject, bool isBodyHtml)
        {
            var result = new Result();
            if (listEmail.Any())
            {
                using (var message = new MailMessage()
                {
                    Subject = subject,
                    Body = bodyContent,
                    IsBodyHtml = isBodyHtml
                })
                {
                    if (filesPath.Any())
                    {
                        foreach (var path in filesPath)
                        {
                            message.Attachments.Add(new Attachment(path));
                        }
                    }
                    foreach (var toEmail in listEmail)
                    {
                        if (!string.IsNullOrEmpty(toEmail))
                        {
                            message.To.Add(toEmail.Trim());
                        }
                    }
                    var sendResult = await SendMailSmtp(message);

                    if (sendResult)
                    {
                        result.Success = true;
                        result.Message = "Send Email Successfully";
                    }
                    else
                    {
                        result.Message = "Send Email Failed";
                    }
                    return result;
                }
            }
            result.Message = "Don't have email to send";
            return result;
        }
    }
}