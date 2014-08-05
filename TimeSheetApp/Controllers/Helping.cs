using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;


public static class Helping
{

    public static void SendEmail(string toEmail, string toName, string subject, string body)
    {
        string fromEmail = "timesheets@tamkeencapital.com";
        string fromPassword = "t@mk33ncapital";
        string fromName = "Timesheets Management - Do not reply";

        var fromAddress = new MailAddress(fromEmail, fromName);
        var toAddress = new MailAddress(toEmail, toName);
        //const string fromPassword = "fromPassword";
        //const string subject = "Subject";
        //const string body = "Body";

        var smtp = new SmtpClient
                   {
                       Host = "smtp.gmail.com",
                       Port = 587,
                       EnableSsl = true,
                       DeliveryMethod = SmtpDeliveryMethod.Network,
                       UseDefaultCredentials = false,
                       Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                   };
        using (var message = new MailMessage(fromAddress, toAddress)
                             {
                                 Subject = subject,
                                 Body = body
                             })
        {
            smtp.Send(message);
        }
    }

}
