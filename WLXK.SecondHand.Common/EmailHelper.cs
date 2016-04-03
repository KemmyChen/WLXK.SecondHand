using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;

namespace WLXK.SecondHand.Common
{
    public static class EmailHelper
    {
        public static string Email = ConfigurationManager.AppSettings["Email"];
        public static string EmailPwd = ConfigurationManager.AppSettings["EmailPwd"];
        public static string EmailSmtp = ConfigurationManager.AppSettings["EmailSmtp"];
        

        public static void SendMail(MyEmail userEmail)
        {

            //发送验证邮箱邮件。
            //1.创建邮件
            MailMessage mail = new MailMessage();
            mail.Subject =  userEmail.Title;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;


            mail.Body = userEmail.Content;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;

            //发件人
            mail.From = new MailAddress(Email, userEmail.Subject);

            //收件人
            mail.To.Add(new MailAddress(userEmail.SendEmail));

            //创建一个发送邮件的类
            SmtpClient client = new SmtpClient(EmailSmtp, 25);
            client.Credentials = new NetworkCredential(Email, EmailPwd);
            client.Send(mail);
        }
    }
}