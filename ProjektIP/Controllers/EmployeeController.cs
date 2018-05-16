using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        public IActionResult ChooseEmployee()
        {
            return PartialView();
        }

        [HttpGet]
        public IActionResult MemberList(bool? showPresent, long? meetingId)
        {
            if (showPresent.HasValue && showPresent.Value)
                ViewBag.ShowPresent = true;
            else
                ViewBag.ShowPresent = false;
            List<EmployeeModel> list = GetAllEmployees();
            if (showPresent != null && showPresent == true && meetingId!=null)
            {
                List<object[]> result = BaseDAO.Select("Members", null, new Dictionary<string, object>() {
                    { "IdMeeting",meetingId}
                });
                foreach (EmployeeModel e in list)
                {
                    foreach(object[] res in result)
                    {
                        if (e.Id == Convert.ToInt32(res[0]))
                        {
                            bool? conf = res[2] != null && res[2] != DBNull.Value ? Convert.ToBoolean(res[2]) : new bool?();
                            e.Confirmation = conf;
                        }
                    }
                }
            }
            return PartialView(list);
        }


        [HttpGet]
        public IActionResult EmployeeList()
        {
            return PartialView(GetAllEmployees());
        }

        public IActionResult PushAddEmployeeToDB(string name, string surname, string email, string phone, long positionId, bool addNewEmpleyeePermission, bool addNewTaskPermission, bool addNewEventPermission, bool previewEmployeesPermission)
        {
            Random random = new Random();
            EmployeeModel employeeModel = new EmployeeModel(0, name, surname, email, phone, true, positionId);

            bool randomLoginAndPassword = true;
            string login = string.Empty;
            string password = string.Empty;
         
            do
            {
                login = name.Substring(0, 3).ToLower() + surname.Substring(0, 3).ToLower() + random.Next(0, 9999).ToString("D4");
                password = Extensions.GeneratePasswordExtensions.GenerateRandomPassword();

                List<object[]> usersList = BaseDAO.Select("Users", null, new Dictionary<string, object>() { { "Login", login } });
                if(!usersList.Any())
                {
                    EmployeeDAO.Insert(employeeModel);
                    employeeModel = EmployeeDAO.SelectFirst(EmployeeDAO.Columns.Fill(employeeModel));
                    BaseDAO.Insert("Users", new Dictionary<string, object>
                    {
                        {"Login", login },
                        {"Password", password },
                        { "IdEmployee",employeeModel.Id} // Zła wartość
                    });

                    if (addNewEmpleyeePermission)
                        BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
                            {
                                { "IdEmployee",employeeModel.Id},
                                { "IdPermission", 1}
                            });
                    if (addNewTaskPermission)
                        BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
                            {
                                { "IdEmployee",employeeModel.Id},
                                { "IdPermission", 2}
                            });
                    if (addNewEventPermission)
                        BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
                            {
                                { "IdEmployee",employeeModel.Id},
                                { "IdPermission", 3}
                            });
                    if (previewEmployeesPermission)
                        BaseDAO.Insert("EmployeePermissions", new Dictionary<string, object>()
                            {
                                { "IdEmployee",employeeModel.Id},
                                { "IdPermission", 4}
                            });

                    randomLoginAndPassword = false;
                }
            }
            while (randomLoginAndPassword);
            
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

        public static EmployeeModel GetEmployeeWithId(long employeeId)
        {
            return EmployeeDAO.SelectFirst(new Dictionary<string, object>
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
                public static string IdPosition = "IdPosition";

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
                List<EmployeeModel> list = new List<EmployeeModel>();

                List<object[]> result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Employees E LEFT JOIN Positions P ON E.IdPosition = P.IdPosition {0}", BaseDAO.GetWhereClause(filters)));
                foreach (object[] res in result)
                {
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
                }
                return list[0];
            }

            public static List<EmployeeModel> Select(Dictionary<string, object> filters)
            {
                List<EmployeeModel> list = new List<EmployeeModel>();

                List<object[]> result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Employees E LEFT JOIN Positions P ON E.IdPosition = P.IdPosition {0}", BaseDAO.GetWhereClause(filters)));
                foreach (object[] res in result)
                {
                    if (HomeController.ActualUser.Id != 0 && Convert.ToInt64(res[0])!=HomeController.ActualUser.Id)
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
                }
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