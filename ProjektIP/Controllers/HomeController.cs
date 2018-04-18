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
		public IActionResult Info()
		{
			return View();
		}
		public IActionResult Index(bool unCorrectData)
		{
			if (unCorrectData)
			{
				ViewBag.notLogIn = "Niepoprawny login lub hasło";
			}
			else
				ViewBag.notLogIn = "OK";

			if (ActualUser == null)
				return View();

			return RedirectToAction("MainPage", "Home");
		}
		[HttpPost]
		public IActionResult Login(string login, string password)
		{
			if (AccountController.AccountDAO.CheckUser(login, password))
			{
				ActualUser = new User(login, password);
				return RedirectToAction("MainPage", "Home");
			}
			ActualUser = null;
			return RedirectToAction("Index", "Home", new { unCorrectData = true });
		}
		public IActionResult UserInfo()
		{
			ViewBag.loggedIn = ActualUser;
			return PartialView();
		}
		public IActionResult Logout()
		{
			ActualUser = null;
			return RedirectToAction("Index", "Home");
		}
		public IActionResult MainPage()
		{
			if (ActualUser != null)
				return View("MainPage", ActualUser.UserPermission);
			return RedirectToAction("Index", "Home");
		}
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


	}
}
