using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjektIP.DAO;
using ProjektIP.Models;

namespace ProjektIP.Controllers
{
	public class TaskController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public IActionResult AddTask()
		{
			return PartialView();
		}
		[HttpPost]
		public IActionResult PushAddTaskToDB(string name, long typeId, long employeeId, long priorityId, string comment)
		{
			TaskController.TaskDAO.Insert(new TaskModel(0, typeId, HomeController.ActualUser.Id, 1, name, null, null, comment, 1, priorityId));
			return RedirectToAction("MainPage", "Home");
		}
		public static class TaskDAO
		{
			public static class Columns
			{
				public static string IdTask = "IdTask";
				public static string IdTaskType = "IdTaskType";
				public static string IdAuthor = "IdAuthor";
				public static string IdEmployee = "IdEmployee";
				public static string Title = "Title";
				public static string DateFrom = "DateStart";
				public static string DateTo = "DateEnd";
				public static string Comment = "Comment";
				public static string IdStatus = "IdStatus";
				public static string IdPriority = "IdPriority";

				public static Dictionary<string, object> Fill(TaskModel Task)
				{
					Dictionary<string, object> filler = new Dictionary<string, object>();
					filler.Add(IdTaskType, Task.TaskTypeId);
					filler.Add(DateFrom, Task.DateStart);
					filler.Add(DateTo, Task.DateEnd);
					filler.Add(IdAuthor, Task.AuthorId);
					filler.Add(IdEmployee, Task.EmployeeId);
					filler.Add(Comment, Task.Comment);
					filler.Add(IdStatus, Task.StatusId);
					filler.Add(IdPriority, Task.PriorityId);
					filler.Add(Title, Task.Title);
					return filler;
				}
			}

			public static TaskModel SelectFirst(Dictionary<string, object> filters)
			{
				List<object[]> result = BaseDAO.Select("Tasks", null, filters);
				return new TaskModel(Convert.ToInt64(result[0][0]), Convert.ToInt64(result[0][1]), Convert.ToInt64(result[0][2]), Convert.ToInt64(result[0][3]), result[0][4].ToString(), Convert.ToDateTime(result[0][5]), Convert.ToDateTime(result[0][6]), result[0][7].ToString(), Convert.ToInt64(result[0][8]), Convert.ToInt64(result[0][9]));
			}

			public static List<TaskModel> Select(Dictionary<string, object> filters)
			{
				List<TaskModel> list = new List<TaskModel>();

				List<object[]> result = BaseDAO.Select("Tasks", null, null);
				foreach (object[] res in result)
					list.Add(new TaskModel(Convert.ToInt64(res[0]), Convert.ToInt64(res[1]), Convert.ToInt64(res[2]), Convert.ToInt64(res[3]), res[4].ToString(), Convert.ToDateTime(res[5]), Convert.ToDateTime(res[6]), res[7].ToString(), Convert.ToInt64(res[8]), Convert.ToInt64(res[9])));

				return list;
			}
			private static DateTime? GetDateTime(object value)
			{
				if (value != null)
					return Convert.ToDateTime(value);
				else
					return null;
			}

			public static void Insert(TaskModel Task)
			{
				BaseDAO.Insert("Tasks", Columns.Fill(Task));
			}

			public static void Update(int id, TaskModel Task)
			{
				BaseDAO.Update("Tasks", new KeyValuePair<string, object>(Columns.IdTask, id), Columns.Fill(Task));
			}
		}
	}
}