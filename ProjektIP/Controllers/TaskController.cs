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
        [HttpGet]
        public IActionResult TaskDetails(int mode)
        {
            ViewBag.ActualUserId = HomeController.ActualUser.Id;
            List<TaskModel> taskList =  GetTasksForUser(HomeController.ActualUser.Id, mode);

            return PartialView(taskList);
        }
        [HttpPost]
        public IActionResult ChangeStatus(long id, int status, DateTime date)
        {
            TaskModel taskToChange = TaskDAO.SelectFirst(new Dictionary<string, object>(){
                { TaskDAO.Columns.IdTask, id }
            });
            taskToChange.StatusId = status;
            if (status == 2)
                taskToChange.DateStart = date;
            else
                taskToChange.DateEnd = date;
            TaskDAO.Update(id, taskToChange);
            return PartialView();
        }
        [HttpGet]
        public IActionResult GetEmployees()
        {
            List<EmployeeModel> listEmployee = EmployeeController.GetAllEmployees();
            return PartialView(listEmployee);
        }
        [HttpPost]
		public IActionResult PushAddTaskToDB(string name, long typeId, long employeeId, long priorityId, string comment)
		{
			TaskController.TaskDAO.Insert(new TaskModel(0, typeId, HomeController.ActualUser.Id, employeeId, name, null, null, comment, 1, priorityId));
			return RedirectToAction("MainPage", "Home");
		}

        private List<TaskModel> GetTasksForUser(long id, int mode)
        {
            return TaskDAO.Select(new Dictionary<string, object>()
            {
                { "T."+TaskDAO.Columns.IdEmployee, id},           
                { "Mode", mode}
            });
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
				return new TaskModel(
                        Convert.ToInt64(result[0][0]),
                        Convert.ToInt64(result[0][1]),
                        Convert.ToInt64(result[0][2]),
                        Convert.ToInt64(result[0][3]),
                        result[0][4].ToString(),
                        result[0][5] != DBNull.Value ? Convert.ToDateTime(result[0][5]) : new DateTime(),
                        result[0][6] != DBNull.Value ? Convert.ToDateTime(result[0][6]) : new DateTime(),
                        result[0][7].ToString(),
                        Convert.ToInt64(result[0][8]),
                        Convert.ToInt64(result[0][9])
                    );
            }

            public static List<TaskModel> Select(Dictionary<string, object> filters)
            {
                List<TaskModel> list = new List<TaskModel>();
                List<object[]> result;
                if (filters.ContainsKey("Mode"))
                {
                    int mode = (int)filters["Mode"];
                    filters.Remove("Mode");
                    switch (mode)
                    {
                        case 1:
                            result = BaseDAO.SelectWithOutWhereClause("SELECT * FROM Tasks T JOIN Employees E ON E.IdEmployee = T.IdAuthor JOIN Priorities P ON T.IdPriority = P.IdPriority JOIN TaskTypes Tt ON T.IdTaskType = Tt.IdTaskType JOIN Status S ON T.IdStatus = S.IdStatus " + BaseDAO.GetWhereClause(filters) + "AND (T.IdStatus = 1 OR T.IdStatus = 5) ORDER BY case when T.IdStatus = 1 then 1 when T.IdStatus = 5 then 2 when T.IdStatus = 2 then 3 else 5 end asc");
                            break;
                        case 2:
                            result = BaseDAO.SelectWithOutWhereClause("SELECT * FROM Tasks T JOIN Employees E ON E.IdEmployee = T.IdAuthor JOIN Priorities P ON T.IdPriority = P.IdPriority JOIN TaskTypes Tt ON T.IdTaskType = Tt.IdTaskType JOIN Status S ON T.IdStatus = S.IdStatus " + BaseDAO.GetWhereClause(filters) + "AND T.IdStatus = 2 ORDER BY case when T.IdStatus = 1 then 1 when T.IdStatus = 5 then 2 when T.IdStatus = 2 then 3 else 5 end asc");
                            break;
                        case 3:
                            result = BaseDAO.SelectWithOutWhereClause("SELECT TOP 5 * FROM Tasks T JOIN Employees E ON E.IdEmployee = T.IdAuthor JOIN Priorities P ON T.IdPriority = P.IdPriority JOIN TaskTypes Tt ON T.IdTaskType = Tt.IdTaskType JOIN Status S ON T.IdStatus = S.IdStatus " + BaseDAO.GetWhereClause(filters) + "AND T.IdStatus = 3 ORDER BY T.DateEnd");
                            break;
                        default:
                            result = BaseDAO.SelectWithOutWhereClause("SELECT * FROM Tasks T JOIN Employees E ON E.IdEmployee = T.IdAuthor JOIN Priorities P ON T.IdPriority = P.IdPriority JOIN TaskTypes Tt ON T.IdTaskType = Tt.IdTaskType JOIN Status S ON T.IdStatus = S.IdStatus " + BaseDAO.GetWhereClause(filters) + "AND (T.IdStatus = 1 OR T.IdStatus = 5 T.IdStatus = 2 OR T.IdStatus = 3) ORDER BY case when T.IdStatus = 1 then 1 when T.IdStatus = 5 then 2 when T.IdStatus = 2 then 3 else 5 end asc");
                            break;
                    }
            }
                else
                {
                    result = BaseDAO.SelectWithOutWhereClause("SELECT * FROM Tasks T JOIN Employees E ON E.IdEmployee = T.IdAuthor JOIN Priorities P ON T.IdPriority = P.IdPriority JOIN TaskTypes Tt ON T.IdTaskType = Tt.IdTaskType JOIN Status S ON T.IdStatus = S.IdStatus " + BaseDAO.GetWhereClause(filters) + "AND (T.IdStatus = 1 OR T.IdStatus = 5 T.IdStatus = 2 OR T.IdStatus = 3) ORDER BY case when T.IdStatus = 1 then 1 when T.IdStatus = 5 then 2 when T.IdStatus = 2 then 3 else 5 end asc");
                }
                foreach (object[] res in result)
                    list.Add(new TaskModel(
                        Convert.ToInt64(res[0]),
                        Convert.ToInt64(res[1]),
                        res[20].ToString(),
                        Convert.ToInt64(res[2]), 
                        res[11].ToString()+" "+ res[12].ToString(),
                        Convert.ToInt64(res[3]), 
                        res[4].ToString(), 
                        res[5]!=DBNull.Value ? Convert.ToDateTime(res[5]): new DateTime(),
                        res[6] != DBNull.Value ? Convert.ToDateTime(res[6]) : new DateTime(),
                        res[7].ToString(), 
                        Convert.ToInt64(res[8]),
                        res[22].ToString(),
                        Convert.ToInt64(res[9]),
                        res[18].ToString()
                        ));

				return list;
			}
			public static void Insert(TaskModel Task)
			{
				BaseDAO.Insert("Tasks", Columns.Fill(Task));
			}

			public static void Update(long id, TaskModel Task)
			{
				BaseDAO.Update("Tasks", new KeyValuePair<string, object>(Columns.IdTask, id), Columns.Fill(Task));
			}
		}
	}
}