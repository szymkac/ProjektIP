﻿using ProjektIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ProjektIP.Common
{
	public static class MailMessageSender
	{
		public static void SendMessage(string addressTo, string addressee, string subject, Model model, MailTypes mailTypes)
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

			MailMessage mailWithImg = getMailWithImg(fromAddress, toAddress, subject, filePath, model, mailTypes);
			smtp.Send(mailWithImg);

		}

		static private MailMessage getMailWithImg(MailAddress fromAddress, MailAddress toAddress, string subject, string filePath, Model model, MailTypes mailTypes)
		{
			string bodyAdd = "";
			switch (mailTypes)
			{
				case MailTypes.addTask:
					bodyAdd = EmailBodyToAddTask((TaskModel)model);
					break;
				case MailTypes.addMeeting:
					bodyAdd = EmailBodyToAddMeeting((MeetingModel)model);
					break;
				case MailTypes.editMeeting:
					bodyAdd = EmailBodyToEditMeeting((MeetingModel)model);
					break;
				case MailTypes.addEmployee:
					bodyAdd = EmailBodyToAddEmployee((User)model);
					break;
				case MailTypes.editTask:
					bodyAdd = EmailBodyToEditTask((TaskModel)model);
					break;
			}
			MailMessage mail = new MailMessage(fromAddress, toAddress);
			mail.IsBodyHtml = true;
			mail.AlternateViews.Add(getEmbeddedImage(filePath, bodyAdd));
			mail.Subject = subject;
			return mail;
		}
		static private AlternateView getEmbeddedImage(String filePath, string bodyAdd)
		{
			LinkedResource res = new LinkedResource(filePath);
			res.ContentId = Guid.NewGuid().ToString();
			string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
			htmlBody += " " + "<br />" + " " + bodyAdd;
			AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
			alternateView.LinkedResources.Add(res);
			return alternateView;
		}

		static private string EmailBodyToAddTask(TaskModel model)
		{
			string htmlBody = "<h2>Zlecono Ci zadanie.</h2><br /><hr/>" +
				"<h3>Szczegóły zadania:</h3><br/>" +
				"<b>Nazwa: </b>" + model.Title + "<br/>" +
				"<b>Autor: </b>" + model.AuthorName + "<br/>" +
				"<b>Priorytet: </b>" + model.PriorityName + "<br/>" +
				"<b>Opis: </b>" + model.Comment + "<br/><br/>" +
				"<hr/>Wiadomość została wygenerowana automatycznie. Prosimy na nią nie odpowiadać. Aby uzyskać więcej informacji należy zalogować się na stronę internetową.";
			return htmlBody;
		}

		static private string EmailBodyToAddMeeting(MeetingModel model)
		{
			string htmlBody = "<h2>Zostałeś zaproszony na spotkanie.</h2><br /><hr/>" +
				"<h3>Szczegóły wydarzenia:</h3><br/>" +
				"<b>Nazwa: </b>" + model.Title + "<br/>" +
				"<b>Data: </b>" + model.DateStart + (model.DateEnd.HasValue ? " - " + model.DateEnd : "") + "<br/>" +
				"<b>Godzina: </b>" + model.HourStart + (model.HourEnd.HasValue ? " - " + model.HourEnd : "") + "<br/>" +
				"<b>Autor: </b>" + model.EmployeeAuthorName + "<br/>" +
				"<b>Miejsce: </b>" + (model.Location != null && model.Location != "" ? model.Location : model.RoomName) + "<br/>" +
				"<b>Priorytet: </b>" + model.PriorityName + "<br/>" +
				"<b>Opis: </b>" + model.Note + "<br/><br/>" +
				"<h3>Potwierdź swoją obecność na stronie internetowej.</h3>" + "<br/>" +
				"<hr/>Wiadomość została wygenerowana automatycznie. Prosimy na nią nie odpowiadać. Aby uzyskać więcej informacji należy zalogować się na stronę internetową.";
			return htmlBody;
		}
		static private string EmailBodyToEditMeeting(MeetingModel model)
		{
			string htmlBody = "<h2>Spotkanie uległo zmianie.</h2><br /><hr/>" +
				"<h3>Szczegóły wydarzenia:</h3><br/>" +
				"<b>Nazwa: </b>" + model.Title + "<br/>" +
				"<b>Data: </b>" + model.DateStart + (model.DateEnd.HasValue ? " - " + model.DateEnd : "") + "<br/>" +
				"<b>Godzina: </b>" + model.HourStart + (model.HourEnd.HasValue ? " - " + model.HourEnd : "") + "<br/>" +
				"<b>Autor: </b>" + model.EmployeeAuthorName + "<br/>" +
				"<b>Miejsce: </b>" + (model.Location != null && model.Location != "" ? model.Location : model.RoomName) + "<br/>" +
				"<b>Priorytet: </b>" + model.PriorityName + "<br/>" +
				"<b>Opis: </b>" + model.Note + "<br/> <br/> " +
				"<h3>Ponownie potwierdź swoją obecność na stronie internetowej.</h3>" + "<br/>" +
				"<hr/>Wiadomość została wygenerowana automatycznie. Prosimy na nią nie odpowiadać. Aby uzyskać więcej informacji należy zalogować się na stronę internetową.";
			return htmlBody;
		}
		static private string EmailBodyToAddEmployee(User user)
		{
			string htmlBody = "<h2>Witamy w naszej firmie.</h2><br /><hr/>" +
				"</h4>Twoje dane logowania:</h3><br/>" +
				"<b>Login: </b>" + user.Login + "<br/>" +
				"<b>Hasło: </b>" + user.Password + "<br/><br/>" +
				"<hr/>Wiadomość została wygenerowana automatycznie. Prosimy na nią nie odpowiadać. Aby uzyskać więcej informacji należy zalogować się na stronę internetową.";
			return htmlBody;
		}
		static private string EmailBodyToEditTask(TaskModel model)
		{
			string htmlBody = "<h2>Zadanie uległo zmianie.</h2><br /><hr/>" +
			"<h3>Szczegóły zadania:</h3><br/>" +
			"<b>Nazwa: </b>" + model.Title + "<br/>" +
			"<b>Autor: </b>" + model.AuthorName + "<br/>" +
			"<b>Priorytet: </b>" + model.PriorityName + "<br/>" +
			"<b>Opis: </b>" + model.Comment + "<br/><br/>" +
			"<hr/>Wiadomość została wygenerowana automatycznie. Prosimy na nią nie odpowiadać. Aby uzyskać więcej informacji należy zalogować się na stronę internetową.";
			return htmlBody;
		}
	}
	public enum MailTypes
	{
		addTask,
		addMeeting,
		editMeeting,
		addEmployee,
		editTask
	}
}
