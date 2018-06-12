using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjektIP.Common;
using ProjektIP.DAO;
using ProjektIP.Models;

namespace ProjektIP.Controllers
{
    public class MeetingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddMeeting()
        {
            List<object[]> roomList = BaseDAO.Select("Rooms", null, null);
            ViewBag.listOfRooms = roomList;

            List<object[]> membersList = BaseDAO.Select("Members", new List<string>() { "IdEmployee", "IdMeeting", "ConfirmationOfPresence" }, null);
            string membersId = string.Empty;
            foreach (object[] member in membersList)
                membersId += (int)member[0] + ",";
            ViewBag.membersId = membersId;

            String format ="dd.MM.yyyy";
            DateTime today = DateTime.Today;
            ViewBag.Today = today.ToString(format);

            return PartialView();
        }

        [HttpPost]
        public IActionResult PushAddMeetingToDB(long meetingTypeId, string title, DateTime datestart, DateTime? dateend, TimeSpan hourStart, TimeSpan? hourEnd, long? roomId, string location, string note, long priorityId, string newIdMembers)
        {
            // TODO
            string[] newMembers = new string[0];
            if (newIdMembers != null)
            {
                newMembers = newIdMembers.Split(",");
            }


            List<EmployeeModel> all = EmployeeController.GetAllEmployees();

            List<EmployeeModel> members = new List<EmployeeModel>();

            foreach (EmployeeModel employee in all)
            {
                foreach (string newM in newMembers)
                {
                    if (!string.IsNullOrEmpty(newM))
                    {
                        if (employee.Id == long.Parse(newM))
                            members.Add(employee);
                    }
                }
            }

            MeetingModel meetingModel = new MeetingModel(0, meetingTypeId, datestart, dateend, hourStart, hourEnd, HomeController.ActualUser.Id, roomId, location, note, priorityId, title, members);
            MeetingDAO.Insert(meetingModel);

            MeetingModel fullMeetingModel = MeetingDAO.Select(MeetingDAO.Columns.Fill(meetingModel, true))[0];

            foreach (EmployeeModel employee in fullMeetingModel.Members)
            {
                MailMessageSender.SendMessage(employee.Email, employee.Name + " " + employee.SurName, "Zaproszono Cię na spotkanie", fullMeetingModel, MailTypes.addMeeting);
            }
            return RedirectToAction("MainPage", "Home");
        }

        [HttpPost]
        public IActionResult PushChangeConfirmation(long meetingId, bool confirmationofPresence)
        {
            BaseDAO.Update("Members", new Dictionary<string, object> { { "IdMeeting", meetingId }, { "IdEmployee", HomeController.ActualUser.Id } }, new Dictionary<string, object>()
            {
                {"IdEmployee",HomeController.ActualUser.Id },
                { "IdMeeting", meetingId},
                { "ConfirmationOfPresence", confirmationofPresence}
            });

			return Json(true);
        }

        [HttpGet]
        public IActionResult EditMeeting(long Id, bool Details)
        {
            MeetingModel model = MeetingDAO.SelectFirst(new Dictionary<string, object>()
            {
                {"IdMeeting", Id }
            });

            List<object[]> roomList=BaseDAO.Select("Rooms", null,null);
            ViewBag.listOfRooms = roomList;
			ViewBag.details = Details;

            List<object[]> membersList = BaseDAO.Select("Members", new List<string>() { "IdEmployee", "IdMeeting", "ConfirmationOfPresence" },
                new Dictionary<string, object>()
            {
                {"IdMeeting", Id }
            });
            List<int> listMembersId = new List<int>();

            string membersId = "";

            foreach (object[] i in membersList)
            {
                membersId += (int)i[0] + ",";
            }

            ViewBag.membersId = membersId;
            ViewBag.meetingId = Id;

            String format = "dd.MM.yyyy";
            DateTime today = DateTime.Today;
            ViewBag.Today = today.ToString(format);

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult PushEditMeetingToDB(long id, long meetingTypeId, DateTime dateStart, DateTime? dateEnd, TimeSpan hourStart, TimeSpan? hourEnd, long? roomId, string location, string note, long priorityId, string title, string oldIdMembers, string newIdMembers)
        {

            string[] newMembers = new string[0];
            if (newIdMembers != null)
            {
                newMembers = newIdMembers.Split(",");
            }


            List<EmployeeModel> all = EmployeeController.GetAllEmployees();

            List<EmployeeModel> members = new List<EmployeeModel>();

            foreach(EmployeeModel employee in all)
            {
                bool newMemberCheck = false;

                foreach (string newM in newMembers)
                {
                    if (newM != "") {
                        if (employee.Id == long.Parse(newM))
                            newMemberCheck = true;
                    }
                }

                if (newMemberCheck == true)
                { //zostawia
                    members.Add(employee);
                }

            }

            MeetingModel model = new MeetingModel(id, meetingTypeId, dateStart, dateEnd, hourStart, hourEnd, HomeController.ActualUser.Id, roomId, location, note, priorityId, title, members);
            if (model.Location == null)
                model.Location = "";
            if (model.RoomId == null) {
                model.RoomId = null;
                model.RoomName = "";
            }

            MeetingDAO.Update((int)id,model);

            MeetingModel fullMeetingModel = MeetingDAO.Select(MeetingDAO.Columns.Fill(model, true))[0];

            foreach (EmployeeModel employee in fullMeetingModel.Members)
            {
               // if (employee.Confirmation != false)
                    MailMessageSender.SendMessage(employee.Email, employee.Name + " " + employee.SurName, "Edytowano Twoje spotkanie", fullMeetingModel, MailTypes.editMeeting);
            }

            return RedirectToAction("MainPage", "Home");
        }

        public static int actualMode;
        public static string Members;
        [HttpPost]
        public IActionResult Back(string newMembers)
        {
            Members = newMembers;
            actualMode = 3;
            return RedirectToAction("MainPage", "Home");
        }

        [HttpGet]
        public IActionResult DeleteMeetingFromDB(long id)
        {
            MeetingDAO.Delete(id);
            return RedirectToAction("MainPage", "Home");
        }

        [HttpGet]
        public IActionResult MeetingDetailsForDay(string date, int column, int mode)
        {
            DateTime day = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            ViewBag.ActualUserId = HomeController.ActualUser.Id;
            ViewBag.Column = column;
			ViewBag.Mode = mode;
			List<MeetingModel> meetingsList = new List<MeetingModel>();
            switch (mode)
            {
                case 1:
                    meetingsList = GetMeetingsForUsers(new List<long> { HomeController.ActualUser.Id }, day);
                    actualMode = 1;
                    break;
                case 2:
                    meetingsList = GetMeetingsForAllUsers(day);
                    actualMode = 2;
                    break;
                default:
                    if (actualMode == 3)
                    {
                        List<long> employees = new List<long>();
                        string[] id = Members.Split(',');
                        foreach (string s in id)
                        {
                            if (s != string.Empty)
                                employees.Add(Convert.ToInt64(s));
                        }
                        meetingsList = GetMeetingsForUsers(employees, day);
                    }
                    else if (actualMode == 2)
                    {
                        meetingsList = GetMeetingsForAllUsers(day);
                    }
                    else
                        meetingsList = GetMeetingsForUsers(new List<long> { HomeController.ActualUser.Id }, day);
                    break;
            }
            return PartialView(meetingsList);
        }

        /// <summary>
        /// Pobiera listę spotkań danego dnia dla danych użytkowników.
        /// </summary>
        /// <param name="id">Lista identyfikatorów użytkowników. </param>
        /// <param name="dateFrom">Data szukanych spotkań</param>
        /// <returns></returns>
        private List<MeetingModel> GetMeetingsForUsers(List<long> id, DateTime dateFrom)
        {
            string likeId = "";
            for (int i = 0; i < id.Count; i++)
            {
                if (i == id.Count - 1)
                    likeId += String.Format("(Mt.IdAuthor LIKE {0} OR (Mb.IdEmployee LIKE {0} AND (Mb.ConfirmationOfPresence = 1 OR Mb.ConfirmationOfPresence  IS NULL))) ", id[i]);
                else
                    likeId += String.Format("(Mt.IdAuthor LIKE {0} OR (Mb.IdEmployee LIKE {0} AND (Mb.ConfirmationOfPresence = 1 OR Mb.ConfirmationOfPresence  IS NULL))) OR", id[i]);
            }

            List<MeetingModel> list = MeetingDAO.Select(new Dictionary<string, object>()
            {
                {"IdEmployee", likeId },
                {"DateFrom", dateFrom}
            });


            return list;
        }

        private List<MeetingModel> GetMeetingsForAllUsers(DateTime dateFrom)
        {

            List<MeetingModel> list = MeetingDAO.Select(new Dictionary<string, object>()
            {
                {"DateFrom", dateFrom}
            });
            return list;
        }

        public static class MeetingDAO
        {
            public static class Columns
            {
                public static string IdMeeting = "IdMeeting";
                public static string IdMeetingType = "IdMeetingType";
                public static string DateFrom = "DateFrom";
                public static string DateTo = "DateTo";
                public static string HourFrom = "HourFrom";
                public static string HourTo = "HourTo";
                public static string IdAuthor = "IdAuthor";
                public static string IdRoom = "IdRoom";
                public static string Location = "Location";
                public static string Note = "Note";
                public static string IdPriority = "IdPriority";
                public static string Title = "Title";

                public static Dictionary<string, object> Fill(MeetingModel Meeting, bool join)
                {
                    Dictionary<string, object> filler = new Dictionary<string, object>();
                    if(join)
                        filler.Add("Mt."+IdMeetingType, Meeting.MeetingTypeId);
                    else
                        filler.Add(IdMeetingType, Meeting.MeetingTypeId);
                    filler.Add(DateFrom, Meeting.DateStart);
                    filler.Add(DateTo, Meeting.DateEnd);
                    filler.Add(HourFrom, Meeting.HourStart);
                    filler.Add(HourTo, Meeting.HourEnd);
                    if (join)
                        filler.Add("Mt." + IdAuthor, Meeting.EmployeeAuthorId);
                    else
                        filler.Add(IdAuthor, Meeting.EmployeeAuthorId);
                    if (join)
                        filler.Add("Mt." + IdRoom, Meeting.RoomId);
                    else
                        filler.Add(IdRoom, Meeting.RoomId);
                    filler.Add(Location, Meeting.Location);
                    filler.Add(Note, Meeting.Note);
                    if (join)
                        filler.Add("Mt." + IdPriority, Meeting.PriorityId);
                    else
                        filler.Add(IdPriority, Meeting.PriorityId);
                    filler.Add(Title, Meeting.Title);
                    return filler;
                }
            }

            public static MeetingModel SelectFirst(Dictionary<string, object> filters)
            {
                List<object[]> result = BaseDAO.Select("Meetings", null, filters);

                List<object[]> members = new List<object[]>();
                if (result.Count > 0)
                {
                    members = BaseDAO.Select("Members", null, new Dictionary<string, object>()
                    {
                        { "IdMeeting",  Convert.ToInt64(result[0][0])}
                    });
                }

                List<EmployeeModel> memberList = new List<EmployeeModel>();
                foreach (object[] mem in members)
                {
                    if (mem[2] == null || mem[2] == DBNull.Value || (mem[2] != null && Convert.ToBoolean(mem[2]) == true))
                    {
                        List<object[]> employee = BaseDAO.Select("Employees", null, new Dictionary<string, object>()
                            {
                                {"IdEmployee",  Convert.ToInt64(mem[0]) }
                            });
                        memberList.Add(new EmployeeModel(
                            Convert.ToInt64(employee[0][0]),
                            employee[0][1].ToString(),
                             employee[0][2].ToString(),
                             employee[0][3].ToString(),
                             employee[0][4].ToString(),
                             employee[0][5].ToString() == "1" ? true : false,
                             employee[0][6] != null ? Convert.ToInt64(employee[0][6]) : new long?(),
                             mem[2] != null && mem[2] != DBNull.Value ? Convert.ToBoolean(mem[2]) : new bool?()
                            ));
                    }
                }
                if (result.Count > 0)
                {
                    string[] HourFrom = result[0][10].ToString().Split(':');
                    string[] HourTo = new string[3];
                    if (result[0][11] != null && result[0][11] != DBNull.Value)
                        HourTo = result[0][11].ToString().Split(':');

                    return new MeetingModel(
                    Convert.ToInt64(result[0][0]),
                    Convert.ToInt64(result[0][1]),
                    Convert.ToDateTime(result[0][2]),
                    result[0][3] != null && result[0][3] != DBNull.Value ? Convert.ToDateTime(result[0][3]) : new DateTime?(),
                    new TimeSpan(0, Convert.ToInt32(HourFrom[0]), Convert.ToInt32(HourFrom[1]), Convert.ToInt32(HourFrom[2])),
                    result[0][11] != null && result[0][11] != DBNull.Value ? new TimeSpan(0, Convert.ToInt32(HourTo[0]), Convert.ToInt32(HourTo[1]), Convert.ToInt32(HourTo[2])) : new TimeSpan?(),
                    Convert.ToInt64(result[0][4]),
                    result[0][5] != null && result[0][5] != DBNull.Value ? Convert.ToInt64(result[0][5]) : new long?(),
                    result[0][6].ToString(),
                    result[0][7].ToString(),
                    Convert.ToInt64(result[0][8]),
                    result[0][9] != null && result[0][9] != DBNull.Value ? result[0][9].ToString() : string.Empty,
                    memberList
                    );
                }
                else
                    return null;
            }

            public static MeetingModel SelectFirstForMembers(Dictionary<string, object> filters)
            {
                List<object[]> result = BaseDAO.Select("Meetings", null, filters);

                List<object[]> members = new List<object[]>();
                if (result.Count > 0)
                {
                    members = BaseDAO.Select("Members", null, new Dictionary<string, object>()
                    {
                        { "IdMeeting",  Convert.ToInt64(result[0][0])}
                    });
                }

                List<EmployeeModel> memberList = new List<EmployeeModel>();
                foreach (object[] mem in members)
                {
                    List<object[]> employee = BaseDAO.Select("Employees", null, new Dictionary<string, object>()
                            {
                                {"IdEmployee",  Convert.ToInt64(mem[0]) }
                            });
                    if (Convert.ToInt64(employee[0][0]) != Convert.ToInt64(result[4]))
                    {
                        memberList.Add(new EmployeeModel(
                            Convert.ToInt64(employee[0][0]),
                            employee[0][1].ToString(),
                             employee[0][2].ToString(),
                             employee[0][3].ToString(),
                             employee[0][4].ToString(),
                             employee[0][5].ToString() == "1" ? true : false,
                             employee[0][6] != null ? Convert.ToInt64(employee[0][6]) : new long?(),
                             mem[2] != null && mem[2] != DBNull.Value ? Convert.ToBoolean(mem[2]) : new bool?()
                            ));
                    }
                }
                if (result.Count > 0)
                {
                    string[] HourFrom = result[0][10].ToString().Split(':');
                    string[] HourTo = new string[3];
                    if (result[0][11] != null && result[0][11] != DBNull.Value)
                        HourTo = result[0][11].ToString().Split(':');

                    return new MeetingModel(
                    Convert.ToInt64(result[0][0]),
                    Convert.ToInt64(result[0][1]),
                    Convert.ToDateTime(result[0][2]),
                    result[0][3] != null && result[0][3] != DBNull.Value ? Convert.ToDateTime(result[0][3]) : new DateTime?(),
                    new TimeSpan(0, Convert.ToInt32(HourFrom[0]), Convert.ToInt32(HourFrom[1]), Convert.ToInt32(HourFrom[2])),
                    result[0][11] != null && result[0][11] != DBNull.Value ? new TimeSpan(0, Convert.ToInt32(HourTo[0]), Convert.ToInt32(HourTo[1]), Convert.ToInt32(HourTo[2])) : new TimeSpan?(),
                    Convert.ToInt64(result[0][4]),
                    result[0][5] != null && result[0][5] != DBNull.Value ? Convert.ToInt64(result[0][5]) : new long?(),
                    result[0][6].ToString(),
                    result[0][7].ToString(),
                    Convert.ToInt64(result[0][8]),
                    result[0][9] != null && result[0][9] != DBNull.Value ? result[0][9].ToString() : string.Empty,
                    memberList
                    );
                }
                else
                    return null;
            }


            public static List<MeetingModel> Select(Dictionary<string, object> filters)
            {
                List<MeetingModel> list = new List<MeetingModel>();

                if (filters != null && filters.ContainsKey("DateFrom"))
                {
                    DateTime date = (DateTime)filters["DateFrom"];
                    filters["DateFrom"] = String.Format(
                        "{1}-{0}-{2}",
                        date.Day.ToString().Length == 1 ? "0" + date.Day.ToString() : date.Day.ToString(),
                        date.Month.ToString().Length == 1 ? "0" + date.Month.ToString() : date.Month.ToString(),
                        date.Year);
                }
                List<object[]> result;
                if (filters != null && filters.ContainsKey("IdEmployee"))
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Meetings Mt LEFT JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting LEFT JOIN Priorities P ON Mt.IdPriority = P.IdPriority  LEFT JOIN Rooms R ON Mt.IdRoom = R.IdRoom LEFT JOIN Employees E ON Mt.IdAuthor = E.IdEmployee LEFT JOIN MeetingTypes Mtt ON Mt.IdMeetingType = Mtt.IdMeetingType WHERE ({0}) AND ((Mt.DateFrom <= '{1}' AND Mt.DateTo >= '{1}') OR Mt.DateFrom = '{1}')", filters["IdEmployee"], filters["DateFrom"]));
                else if (filters != null && filters.ContainsKey("DateFrom"))
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Meetings Mt LEFT JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting LEFT JOIN Priorities P ON Mt.IdPriority = P.IdPriority  LEFT JOIN Rooms R ON Mt.IdRoom = R.IdRoom LEFT JOIN Employees E ON Mt.IdAuthor = E.IdEmployee LEFT JOIN MeetingTypes Mtt ON Mt.IdMeetingType = Mtt.IdMeetingType WHERE (Mt.DateFrom <= '{0}' AND Mt.DateTo >= '{0}') OR Mt.DateFrom = '{0}'", filters["DateFrom"]));
                else
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Meetings Mt LEFT JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting LEFT JOIN Priorities P ON Mt.IdPriority = P.IdPriority  LEFT JOIN Rooms R ON Mt.IdRoom = R.IdRoom LEFT JOIN Employees E ON Mt.IdAuthor = E.IdEmployee LEFT JOIN MeetingTypes Mtt ON Mt.IdMeetingType = Mtt.IdMeetingType"));

                foreach (object[] res in result)
                {
                    List<object[]> members = BaseDAO.Select("Members", null, new Dictionary<string, object>()
                    {
                        { "IdMeeting",  Convert.ToInt64(res[0])}
                    });

                    List<EmployeeModel> memberList = new List<EmployeeModel>();
                    foreach (object[] mem in members)
                    {
                        if (mem[2] == null || mem[2] == DBNull.Value || (mem[2] != null && Convert.ToBoolean(mem[2]) == true))
                        {
                            List<object[]> employee = BaseDAO.Select("Employees", null, new Dictionary<string, object>()
                            {
                                {"IdEmployee",  Convert.ToInt64(mem[0]) }
                            });
                            memberList.Add(new EmployeeModel(
                                Convert.ToInt64(employee[0][0]),
                                employee[0][1].ToString(),
                                 employee[0][2].ToString(),
                                 employee[0][3].ToString(),
                                 employee[0][4].ToString(),
                                 employee[0][5].ToString() == "1" ? true : false,
                                 employee[0][6] != null ? Convert.ToInt64(employee[0][6]) : new long?(),
                                 mem[2] != null && mem[2] != DBNull.Value ? Convert.ToBoolean(mem[2]) : new bool?()
                                ));
                        }
                    }
                    string[] HourFrom = res[10].ToString().Split(':');
                    string[] HourTo = new string[3];
                    if (res[11] != null && res[11] != DBNull.Value)
                        HourTo = res[11].ToString().Split(':');

                    MeetingModel modelDoDodania = new MeetingModel(
                    Convert.ToInt64(res[0]),
                    Convert.ToInt64(res[1]),
                    res[27].ToString(),
                    Convert.ToDateTime(res[2]),
                    res[3] != null && res[3] != DBNull.Value ? Convert.ToDateTime(res[3]) : new DateTime?(),
                    new TimeSpan(0, Convert.ToInt32(HourFrom[0]), Convert.ToInt32(HourFrom[1]), Convert.ToInt32(HourFrom[2])),
                    res[11] != null && res[11] != DBNull.Value ? new TimeSpan(0, Convert.ToInt32(HourTo[0]), Convert.ToInt32(HourTo[1]), Convert.ToInt32(HourTo[2])) : new TimeSpan?(),
                    Convert.ToInt64(res[4]),
                     Convert.ToInt64(res[25]),
                    res[20].ToString() + " " + res[21].ToString(),
                    res[5] != null && res[5] != DBNull.Value ? Convert.ToInt64(res[5]) : new long?(),
                    res[18].ToString(),
                    res[6].ToString(),
                    res[7].ToString(),
                    Convert.ToInt64(res[8]),
                     res[16].ToString(),
                        res[9] != null && res[9] != DBNull.Value ? res[9].ToString() : string.Empty,
                        memberList
                        );
                    bool dodaj = true;
                    foreach (MeetingModel mm in list)
                    {
                        if (mm.Id == modelDoDodania.Id)
                            dodaj = false;
                    }
                    if (dodaj)
                        list.Add(modelDoDodania);
                }
                return list;
            }

            public static List<MeetingModel> SelectForMembers(Dictionary<string, object> filters)
            {
                List<MeetingModel> list = new List<MeetingModel>();

                if (filters != null && filters.ContainsKey("DateFrom"))
                {
                    DateTime date = (DateTime)filters["DateFrom"];
                    filters["DateFrom"] = String.Format(
                        "{1}-{0}-{2}",
                        date.Day.ToString().Length == 1 ? "0" + date.Day.ToString() : date.Day.ToString(),
                        date.Month.ToString().Length == 1 ? "0" + date.Month.ToString() : date.Month.ToString(),
                        date.Year);
                }
                List<object[]> result;
                if (filters != null && filters.ContainsKey("IdEmployee"))
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Meetings Mt LEFT JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting LEFT JOIN Priorities P ON Mt.IdPriority = P.IdPriority  LEFT JOIN Rooms R ON Mt.IdRoom = R.IdRoom LEFT JOIN Employees E ON Mt.IdAuthor = E.IdEmployee LEFT JOIN MeetingTypes Mtt ON Mt.IdMeetingType = Mtt.IdMeetingType WHERE ({0}) AND Mt.DateFrom = '{1}'", filters["IdEmployee"], filters["DateFrom"]));
                else if (filters != null && filters.ContainsKey("DateFrom"))
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Meetings Mt LEFT JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting LEFT JOIN Priorities P ON Mt.IdPriority = P.IdPriority  LEFT JOIN Rooms R ON Mt.IdRoom = R.IdRoom LEFT JOIN Employees E ON Mt.IdAuthor = E.IdEmployee LEFT JOIN MeetingTypes Mtt ON Mt.IdMeetingType = Mtt.IdMeetingType WHERE Mt.DateFrom = '{0}'", filters["DateFrom"]));
                else
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("SELECT * FROM Meetings Mt LEFT JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting LEFT JOIN Priorities P ON Mt.IdPriority = P.IdPriority  LEFT JOIN Rooms R ON Mt.IdRoom = R.IdRoom LEFT JOIN Employees E ON Mt.IdAuthor = E.IdEmployee LEFT JOIN MeetingTypes Mtt ON Mt.IdMeetingType = Mtt.IdMeetingType"));

                foreach (object[] res in result)
                {
                    List<object[]> members = BaseDAO.Select("Members", null, new Dictionary<string, object>()
                    {
                        { "IdMeeting",  Convert.ToInt64(res[0])}
                    });

                    List<EmployeeModel> memberList = new List<EmployeeModel>();
                    foreach (object[] mem in members)
                    {
                        List<object[]> employee = BaseDAO.Select("Employees", null, new Dictionary<string, object>()
                            {
                                {"IdEmployee",  Convert.ToInt64(mem[0]) }
                            });
                        if (Convert.ToInt64(employee[0][0]) != Convert.ToInt64(res[4]))
                        {
                            memberList.Add(new EmployeeModel(
                                Convert.ToInt64(employee[0][0]),
                                employee[0][1].ToString(),
                                 employee[0][2].ToString(),
                                 employee[0][3].ToString(),
                                 employee[0][4].ToString(),
                                 employee[0][5].ToString() == "1" ? true : false,
                                 employee[0][6] != null ? Convert.ToInt64(employee[0][6]) : new long?(),
                                 mem[2] != null && mem[2] != DBNull.Value ? Convert.ToBoolean(mem[2]) : new bool?()
                                ));
                        }
                    }
                    string[] HourFrom = res[10].ToString().Split(':');
                    string[] HourTo = new string[3];
                    if (res[11] != null && res[11] != DBNull.Value)
                        HourTo = res[11].ToString().Split(':');

                    MeetingModel modelDoDodania = new MeetingModel(
                    Convert.ToInt64(res[0]),
                    Convert.ToInt64(res[1]),
                    res[27].ToString(),
                    Convert.ToDateTime(res[2]),
                    res[3] != null && res[3] != DBNull.Value ? Convert.ToDateTime(res[3]) : new DateTime?(),
                    new TimeSpan(0, Convert.ToInt32(HourFrom[0]), Convert.ToInt32(HourFrom[1]), Convert.ToInt32(HourFrom[2])),
                    res[11] != null && res[11] != DBNull.Value ? new TimeSpan(0, Convert.ToInt32(HourTo[0]), Convert.ToInt32(HourTo[1]), Convert.ToInt32(HourTo[2])) : new TimeSpan?(),
                    Convert.ToInt64(res[4]),
                     Convert.ToInt64(res[25]),
                    res[20].ToString() + " " + res[21].ToString(),
                    res[5] != null && res[5] != DBNull.Value ? Convert.ToInt64(res[5]) : new long?(),
                    res[18].ToString(),
                    res[6].ToString(),
                    res[7].ToString(),
                    Convert.ToInt64(res[8]),
                     res[16].ToString(),
                        res[9] != null && res[9] != DBNull.Value ? res[9].ToString() : string.Empty,
                        memberList
                        );
                    bool dodaj = true;
                    foreach (MeetingModel mm in list)
                    {
                        if (mm.Id == modelDoDodania.Id)
                            dodaj = false;
                    }
                    if (dodaj)
                        list.Add(modelDoDodania);
                }
                return list;
            }


            public static void Insert(MeetingModel Meeting)
            {
                BaseDAO.Insert("Meetings", Columns.Fill(Meeting,false));
                if (Meeting.Members != null && Meeting.Members.Count>0)
                {
                    MeetingModel meeting = SelectFirst(Columns.Fill(new MeetingModel(Meeting.Id,Meeting.MeetingTypeId,Meeting.DateStart,Meeting.DateEnd,Meeting.HourStart,Meeting.HourEnd,Meeting.EmployeeAuthorId,Meeting.RoomId,Meeting.Location,Meeting.Note,Meeting.PriorityId,Meeting.Title,new List<EmployeeModel>()),false));
                    long idMeeting = meeting.Id;
                    foreach (EmployeeModel employee in Meeting.Members)
                        BaseDAO.Insert("Members", MembersDAO.Columns.Fill(employee.Id, idMeeting));
                }
            }

            public static void Update(int id, MeetingModel Meeting)
            {
                BaseDAO.Update("Meetings", new Dictionary<string, object> { { Columns.IdMeeting, id } }, Columns.Fill(Meeting,false));
                BaseDAO.Delete("Members", new Dictionary<string, object>()
                {
                    {"IdMeeting",id }
                });
                foreach (EmployeeModel employee in Meeting.Members)
                {
                    BaseDAO.Insert("Members", MembersDAO.Columns.Fill(employee.Id, id));
                }
            }

            public static void Delete(long id)
            {
                BaseDAO.Delete("Members", new Dictionary<string, object>()
                {
                    { MembersDAO.Columns.IdMeeting, id}
                });
                BaseDAO.Delete("Meetings", new Dictionary<string, object>()
                {
                    { Columns.IdMeeting, id}
                });
            }

            public static class MembersDAO
            {
                public static class Columns
                {
                    public static string IdMeeting = "IdMeeting";
                    public static string IdEmployee = "IdEmployee";
                    public static string Confirmation = "ConfirmationOfPresence";

                    public static Dictionary<string, object> Fill(long idEmployee, long idMeeting)
                    {
                        Dictionary<string, object> filler = new Dictionary<string, object>();
                        filler.Add(IdMeeting, idMeeting);
                        filler.Add(IdEmployee, idEmployee);
                        filler.Add(Confirmation, null);
                        return filler;
                    }
                }
            }

        }
    }
}