using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjektIP.DAO;
using ProjektIP.Models;

namespace ProjektIP.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
		public IActionResult AddEmployee()
		{
			return PartialView();
		}
		
		public IActionResult PushAddEmployeeToDB()
		{
			return RedirectToAction("MainPage", "Home");
		}
        public static class EmployeeDAO
        {

            public static class Columns
            {
                public static string IdEmployee = "IdEmployee";
                public static string Name = "Name";
                public static string Surname = "Surname";
                public static string Email = "Email";
                public static string Phone = "Phone";
                public static string Active = "Active";
                public static string IdPosition = "IdPostition";

                public static Dictionary<string, object> Fill(EmployeeModel employee)
                {
                    Dictionary<string, object> filler = new Dictionary<string, object>();
                    filler.Add(Name, employee.Name);
                    filler.Add(Surname, employee.SurName);
                    filler.Add(Email, employee.Email);
                    filler.Add(Phone, employee.Phone);
                    filler.Add(Active, employee.Active);
                    filler.Add(IdPosition, employee.PositionId);
                    return filler;
                }
            }
            
            public static EmployeeModel SelectFirst(Dictionary<string, object> filters)
            {
                List<object[]> result = BaseDAO.Select("Employees", null, null);
                return new EmployeeModel(
                    Convert.ToInt64(result[0][0]),
                    result[0][1].ToString(), 
                    result[0][2].ToString(), 
                    result[0][3].ToString(), 
                    result[0][4].ToString(), 
                    Convert.ToBoolean(result[0][5]),
                    result[0][6] != null ? Convert.ToInt64(result[0][6]) : new long?());
            }

            public static List<EmployeeModel> Select(Dictionary<string, object> filters)
            {
                List<EmployeeModel> list = new List<EmployeeModel>();

                List<object[]> result = BaseDAO.Select("Employees", null, null);
                foreach (object[] res in result)
                    list.Add(new EmployeeModel(
                        Convert.ToInt64(res[0]),
                        res[1].ToString(),
                        res[2].ToString(),
                        res[3].ToString(),
                        res[4].ToString(),
                        Convert.ToBoolean(res[5]),
                        res[6] != null ? Convert.ToInt64(res[6]) : new long?()
                        ));

                return list;
            }
            public static void Insert(EmployeeModel employee)
            {
                BaseDAO.Insert("Employees", Columns.Fill(employee));
            }

            public static void Update(int id, EmployeeModel employee)
            {
                BaseDAO.Update("Employees", new KeyValuePair<string, object>(Columns.IdEmployee, id), Columns.Fill(employee));
            }
        }
    }
}