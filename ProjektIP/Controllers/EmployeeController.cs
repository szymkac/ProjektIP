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

        [HttpGet]
        public IActionResult MemberList()
        {
            return PartialView(GetAllEmployees());
        }


        [HttpGet]
        public IActionResult EmployeeList()
        {
            return PartialView(GetAllEmployees());
        }

        public IActionResult PushAddEmployeeToDB()
        {

            //bool losuj = true;
            //while (losuj)
            //{
            //    Random r = new Random();
            //    string wylosowany_login = employeeModel.Name.Substring(0, 3) + employeeModel.SurName.Substring(0, 3) + r.Next();
            //    string wylosowany_password = r.Next().ToString() + r.Next().ToString() + r.Next().ToString() + r.Next().ToString() + r.Next().ToString();
            //    List<object[]> lista = BaseDAO.Select("Users", null, new Dictionary<string, object>() { { "Login", wylosowany_login } });
            //    if (lista.Count == 0)
            //    {
            //        EmployeeDAO.Insert(employeeModel);
            //        BaseDAO.Insert("Users", new Dictionary<string, object>
            //        {
            //            { "Login",wylosowany_login},
            //            { "Password", wylosowany_password}
            //        });
            //        EmployeeModel addedEmployee = EmployeeDAO.SelectFirst(EmployeeDAO.Columns.Fill(EmployeeModel));
            //        if (addingUserPermission)
            //            BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
            //        {
            //            { "IdEmployees",addedEmployee.Id},
            //            { "IdPermission", 1}
            //        });
            //        if (addingTaskPermission)
            //            BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
            //        {
            //            { "IdEmployees",addedEmployee.Id},
            //            { "IdPermission", 2}
            //        });
            //        if (addingMeetingPermission)
            //            BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
            //        {
            //            { "IdEmployees",addedEmployee.Id},
            //            { "IdPermission", 3}
            //        });
            //        if (previewUserPermission)
            //            BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
            //        {
            //            { "IdEmployees",addedEmployee.Id},
            //            { "IdPermission", 4}
            //        });
            //        losuj = false;
            //        //TO DO: WYSYŁANIE MAILA
            //    }
            //}
            return RedirectToAction("MainPage", "Home");
        }

        public static List<EmployeeModel> GetAllEmployees()
        {
            return EmployeeDAO.Select(null);
        }

        public static List<EmployeeModel> GetEmployeesOnPosition(long positionId)
        {
            return EmployeeDAO.Select(new Dictionary<string, object>
            {
                {EmployeeDAO.Columns.IdPosition, positionId}
            });
        }

        public static List<EmployeeModel> GetEmployeeWithId(long employeeId)
        {
            return EmployeeDAO.Select(new Dictionary<string, object>
            {
                {EmployeeDAO.Columns.IdEmployee, employeeId}
            });
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
                    result[0][5].ToString() == "1" ? true : false,
                    result[0][6] != null && result[0][6] != DBNull.Value ? Convert.ToInt64(result[0][6]) : new long?());
            }

            public static List<EmployeeModel> Select(Dictionary<string, object> filters)
            {
                List<EmployeeModel> list = new List<EmployeeModel>();

                List<object[]> result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Employees E LEFT JOIN Positions P ON E.IdPosition = P.IdPosition {0}", BaseDAO.GetWhereClause(filters)));
                foreach (object[] res in result)
                    list.Add(new EmployeeModel(
                        Convert.ToInt64(res[0]),
                        res[1].ToString(),
                        res[2].ToString(),
                        res[3].ToString(),
                        res[4].ToString(),
                        res[5].ToString() == "1" ? true : false,
                        res[6] != null && res[6] != DBNull.Value ? Convert.ToInt64(res[6]) : new long?(),
                        res[8].ToString()
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