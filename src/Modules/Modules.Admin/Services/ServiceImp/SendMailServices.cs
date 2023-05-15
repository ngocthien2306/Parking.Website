using InfrastructureCore;
using InfrastructureCore.Utils;
using Microsoft.Extensions.Configuration;
using Modules.Admin.Services.IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Admin.Services.ServiceImp
{
    public class SendMailServices : ISendMailServices
    {
        private readonly IConfiguration _config;
        private readonly string SendMailResult001 = "641"; //Send Email Failed
        private readonly string SendMailResult002 = "642"; //Send Email Successfully
        private readonly string SendMailResult003 = "639"; //Don't have email to send

        public bool CheckEmail(string email)
        {
            bool flag = true;
            try
            {
                TcpClient tClient = new TcpClient("gmail-smtp-in.l.google.com", 25);
                string CRLF = "\r\n";
                byte[] dataBuffer;
                string ResponseString;
                NetworkStream netStream = tClient.GetStream();
                StreamReader reader = new StreamReader(netStream);
                ResponseString = reader.ReadLine();
                /* Perform HELO to SMTP Server and get Response */
                dataBuffer = BytesFromString("HELO KirtanHere" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                dataBuffer = BytesFromString("MAIL FROM:<hoangquandang94@gmail.com>" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                /* Read Response of the RCPT TO Message to know from google if it exist or not */
                dataBuffer = BytesFromString("RCPT TO:<" + email + ">" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();

                //550 – The message has failed because the other user’s mailbox is unavailable or because the recipient server rejected your message.
                if (GetResponseCode(ResponseString) == 550)
                {
                    flag = false;
                }
                /* QUITE CONNECTION */
                dataBuffer = BytesFromString("QUITE" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                tClient.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return flag;
        }
        private byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private int GetResponseCode(string ResponseString)
        {
            return int.Parse(ResponseString.Substring(0, 3));
        }
        public SendMailServices(IConfiguration config)
        {
            _config = config;
        }

        public SendMailServices()
        {
        }


        public async Task<Result> SendEmailTo(IEnumerable<string> listEmail, string bodyContent, string subject, bool isBodyHtml)
        {
            var listEmailError = new StringBuilder();
            listEmailError.Append("<br>");
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
                        //if (!CheckEmail(toEmail))
                        //{
                        //    listEmailError.Append(toEmail).Append(" ");                           
                        //}
                        //else
                        //{
                            if (!string.IsNullOrEmpty(toEmail))
                            {
                                message.To.Add(toEmail.Trim());
                            }
                        //}
                    }
                    //message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    var sendResult = await SendMailSmtp(message);

                    if (sendResult)
                    {
                        result.Success = true;
                        result.Message = SendMailResult002;
                        result.Error = listEmailError.ToString();
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = SendMailResult001;
                        result.Error = listEmailError.ToString();
                    }
                    return result;
                }
            }
            result.Message = SendMailResult003;
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
                var emailPassword = _config.GetValue<string>("MailSettings:EmailPassword");

                var host = _config.GetValue<string>("MailSettings:Host");
                var port = _config.GetValue<int>("MailSettings:Port");
                var enableSsl = _config.GetValue<bool>("MailSettings:EnableSsl");

                //Switch off certificate validation// Do not use
                ServicePointManager.ServerCertificateValidationCallback =
                                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                                    { return true; };

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
                    message.IsBodyHtml = true;
                    await smtp.SendMailAsync(message);
                    return true;
                };

            }
            catch (Exception e)
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
        public async Task<Result> SendEmailWithFileTo(List<string> filesPath, IEnumerable<string> listEmail, string bodyContent, string subject, bool isBodyHtml, string SenderMailAddress, string SenderEmailPassword, string SenderEmailSmtpServer)
        {
            LogWriter log = new LogWriter("SendEmailWithFileTo");
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
                        result.Message = SendMailResult002;
                        log.LogWrite(SendMailResult002);
                    }
                    else
                    {
                        result.Message = SendMailResult001;
                        log.LogWrite(SendMailResult001);

                    }
                    return result;
                }
            }
            result.Message = SendMailResult003;
            return result;
        }

    }
}
