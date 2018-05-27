using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ProjektIP.Common
{
    static public class MailMessageSender
    {
		static public void SendMessage(string addressTo, string addressee, string subject)
		{
			MailAddress fromAddress = new MailAddress("pingwinyib@gmail.com", "What's Today");
			MailAddress toAddress = new MailAddress(addressTo, addressee);
			const string fromPassword = "fourier<3";
			string filePath = @"Content\Image\logo_mini.png";

			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
				Timeout = 20000
			};

			MailMessage mailWithImg = getMailWithImg(fromAddress, toAddress, subject, filePath);
			smtp.Send(mailWithImg);

		}

		static private MailMessage getMailWithImg(MailAddress fromAddress, MailAddress toAddress, string subject, string filePath)
		{
			MailMessage mail = new MailMessage(fromAddress, toAddress);
			mail.IsBodyHtml = true;
			mail.AlternateViews.Add(getEmbeddedImage(filePath));
			mail.Subject = "yourSubject";
			return mail;
		}
		static private AlternateView getEmbeddedImage(String filePath)
		{
			LinkedResource res = new LinkedResource(filePath);
			res.ContentId = Guid.NewGuid().ToString();
			string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
			htmlBody += " " + "<br />" + " " + "Kupa";
			AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
			alternateView.LinkedResources.Add(res);
			return alternateView;
		}
	}
}
