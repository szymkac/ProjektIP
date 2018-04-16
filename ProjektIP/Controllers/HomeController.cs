using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjektIP.Common;
using ProjektIP.DAO;
using ProjektIP.Models;

namespace ProjektIP.Controllers
{
	public class HomeController : Controller
	{
		public static User ActualUser;
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Info()
		{
			return View();
		}
		public IActionResult MainPage()
		{
			return View("MainPage");
		}
		[HttpPost]
		public IActionResult MainPage(string login, string password)
		{
			if (AccountController.AccountDAO.CheckUser(login, password))
			{
				ViewData["Message"] = "dobre hasło";
				ActualUser = new User(login, password);
			}
			else
			{
				ViewData["Message"] = "złe hasło";
				ActualUser = null;
			}
			return View("MainPage");

		}
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


	}
}
