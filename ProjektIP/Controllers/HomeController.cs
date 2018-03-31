using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjektIP.Database;
using ProjektIP.Models;

namespace ProjektIP.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult AddUser()
		{
			return View();
		}
		public IActionResult Info()
		{
			ViewData["Message"] = "Informacje";

			return View();
		}
		[HttpPost]
		public IActionResult Login(string login, string password)
		{
			if (AccountController.AccountDAO.CheckUser(login, password))
				ViewData["Message"] = "dobre hasło";
			else
				ViewData["Message"] = "złe hasło";
			return View("Login");

		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


	}
}
