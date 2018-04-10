using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
		public IActionResult MeetingDetailsForDay(string date, int column)
		{
			DateTime day = Convert.ToDateTime(date);
			ViewBag.ActualUserId = HomeController.ActualUser.Id;
			ViewBag.Column = column;

			DateTime dt = new DateTime(2018, 4, 9, 10, 20, 1);
			string hour = "23:12:10";
			DateTime dateTime = DateTime.ParseExact(hour, "HH:mm:ss",
										CultureInfo.InvariantCulture);

			List<MeetingModel> meetingsList = new List<MeetingModel>();
			meetingsList.Add(new MeetingModel(0, 1, "Spotkanie", new DateTime(2018, 4, 9), null, new DateTime(2018, 4, 9, 10, 20, 0), new DateTime(2018, 4, 9, 20, 20, 0), 4, 1, "sala 123", "", "Notatka", 1, "Pilny", "SPOTKANIE"));
			meetingsList.Add(new MeetingModel(1, 2, "Wyjazd", new DateTime(2018, 4, 9), null, new DateTime(2018, 4, 9, 10, 20, 0), null, 3, 1, "", "Ustrzyki", "Notatka", 2, "Normalny", "WYJAZD"));
			meetingsList.Add(new MeetingModel(2, 3, "Telekonferencja", new DateTime(2018, 4, 9), null, new DateTime(2018, 4, 9, 10, 20, 0), null, 2, 1, "sala 123", "", "Notatka", 3, "Niski", "TELE"));
			meetingsList.Add(new MeetingModel(3, 4, "Szkolenie", new DateTime(2018, 4, 9), null, new DateTime(2018, 4, 9, 10, 20, 0), null, 1, 1, "sala 123", "", "Notatka", 4, "Opjonalny", "SZKOLENIE"));
			meetingsList.Add(new MeetingModel(4, 1, "Spotkanie", new DateTime(2018, 4, 9), null, new DateTime(2018, 4, 9, 10, 20, 0), null, 4, 1, "sala 123", "", "Notatka", 1, "Pilny", "SPOTKANIE"));

			return PartialView(meetingsList);
		}
		[HttpGet]
		public IActionResult GetMeeting(DateTime dt)
		{
			dt = new DateTime(2018, 4, 9);
			//List<MeetingModel> mm = MeetingDAO.Select(new Dictionary<string, object>
			//{
			//	{ MeetingDAO.Columns.IdAuthor, HomeController.ActualUser.Id },
			//	{ MeetingDAO.Columns.DateFrom, dt },
			//});
			List<MeetingModel> mm = new List<MeetingModel>();
			mm.Add(new MeetingModel(0, 1, "Spotkanie", new DateTime(2018, 4, 9), null, new DateTime(2018, 4, 9, 10, 20, 0), null, 4, 1, "sala 123", "", "Notatka", 1, "Pilny", "SPOTKANIE"));

			return Json(mm);
		}
            List<MeetingModel> lista = MeetingDAO.Select(new Dictionary<string, object>()
            {
                {"DateFrom", dateFrom}
            });
            return View();
        }

        /// <summary>
        /// Pobiera listę spotkań danego dnia dla danych użytkowników.
        /// </summary>
        /// <param name="id">Lista identyfikatorów użytkowników. </param>
        /// <param name="dateFrom">Data szukanych spotkań</param>
        /// <returns></returns>
        public IActionResult GetMeetingsForUsers(List<long> id, DateTime dateFrom)
        {
            string likeId = "";
            foreach (long l in id)
                likeId += String.Format("'{0}' ", l);

            List<MeetingModel> lista = MeetingDAO.Select(new Dictionary<string, object>()
            {
                {"IdEmployee", likeId },
                {"DateFrom", dateFrom}
            });
            return View();
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
                public static string Note  = "Note";
                public static string IdPriority = "IdPriority";
                public static string Title = "Title";

                public static Dictionary<string, object> Fill(MeetingModel Meeting)
                {
                    Dictionary<string, object> filler = new Dictionary<string, object>();
                    filler.Add(IdMeetingType, Meeting.MeetingTypeId);
                    filler.Add(DateFrom, Meeting.DateStart);
                    filler.Add(DateTo, Meeting.DateEnd);
                    filler.Add(HourFrom, Meeting.HourStart);
                    filler.Add(HourTo, Meeting.HourEnd);
					filler.Add(IdAuthor, Meeting.EmployeeAuthorId);
                    filler.Add(IdRoom, Meeting.RoomId);
                    filler.Add(Location, Meeting.Location);
                    filler.Add(Note, Meeting.Note);
                    filler.Add(IdPriority, Meeting.PriorityId);
                    filler.Add(Title, Meeting.Title);
                    return filler;
                }
            }

            public static MeetingModel SelectFirst(Dictionary<string, object> filters)
            {
                List<object[]> result = BaseDAO.Select("Meetings", null, filters);

                List<object[]> members = BaseDAO.Select("Members", null, new Dictionary<string, object>()
                    {
                        { "IdMeeting",  Convert.ToInt64(result[0][0])}
                    });

                List<EmployeeModel> memberList = new List<EmployeeModel>();
                foreach (object[] mem in members)
                {
                    if (mem[3] == null || (mem[3] != null && Convert.ToBoolean(mem[3]) == true))
                    {
                        List<object[]> employee = BaseDAO.Select("Employees", null, new Dictionary<string, object>()
                            {
                                {"IdEmployee",  Convert.ToInt64(mem[2]) }
                            });
                        memberList.Add(new EmployeeModel(
                            Convert.ToInt64(employee[0][0]),
                            employee[0][1].ToString(),
                             employee[0][2].ToString(),
                             employee[0][3].ToString(),
                             employee[0][4].ToString(),
                             Convert.ToBoolean(employee[0][5]),
                             employee[0][6] != null ? Convert.ToInt64(employee[0][6]) : new long?()
                            ));
                    }
                }

                return new MeetingModel(
                    Convert.ToInt64(result[0][0]), 
                    Convert.ToInt64(result[0][1]), 
                    Convert.ToDateTime(result[0][2]),
                    result[0][3] != null ? Convert.ToDateTime(result[0][3]) : new DateTime?(),
                    Convert.ToDateTime(result[0][4]),
                    result[0][5] != null ? Convert.ToDateTime(result[0][5]) : new DateTime?(),
                    Convert.ToInt64(result[0][6]),
                    result[0][7] != null ? Convert.ToInt64(result[0][7]) : new long?(),
                    result[0][8].ToString(),
                    result[0][9].ToString(), 
                    Convert.ToInt64(result[0][10]),
                    result[0][11].ToString(),
                    memberList
                    );
            }

            public static List<MeetingModel> Select(Dictionary<string, object> filters)
            {
                List<MeetingModel> list = new List<MeetingModel>();


                List<object[]> result;
                if (filters.ContainsKey("IdEmployee"))
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("Meetings Mt JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting WHERE (Mt.IdAuthor LIKE {0} OR Mb.IdEmployee LIKE {0}) AND Mt.DateFrom = '{1}'",filters["IdEmployee"],filters["DateFrom"]), null);
                else
                    result = BaseDAO.SelectWithOutWhereClause(String.Format("Meetings Mt JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting WHERE Mt.DateFrom = '{0}'", filters["DateFrom"]), null);
                
                foreach (object[] res in result)
                {
                    List<object[]> members = BaseDAO.Select("Members", null, new Dictionary<string, object>()
                    {
                        { "IdMeeting",  Convert.ToInt64(res[0])}
                    });

                    List<EmployeeModel> memberList = new List<EmployeeModel>();
                    foreach(object[] mem in members)
                    {
                        if(mem[3]==null|| (mem[3]!=null && Convert.ToBoolean(mem[3])==true))
                        {
                            List<object[]> employee = BaseDAO.Select("Employees", null, new Dictionary<string, object>()
                            {
                                {"IdEmployee",  Convert.ToInt64(mem[2]) }
                            });
                            memberList.Add(new EmployeeModel(
                                Convert.ToInt64(employee[0][0]),
                                employee[0][1].ToString(),
                                 employee[0][2].ToString(),
                                 employee[0][3].ToString(),
                                 employee[0][4].ToString(),
                                 Convert.ToBoolean(employee[0][5]),
                                 employee[0][6]!=null ? Convert.ToInt64(employee[0][6]) : new long?()
                                ));
                        }
                    }

                    list.Add(new MeetingModel(
                        Convert.ToInt64(res[0]),
                        Convert.ToInt64(res[1]),
                        Convert.ToDateTime(res[2]),
                        result[3] != null ? Convert.ToDateTime(result[3]) : new DateTime?(),
                        Convert.ToDateTime(res[4]),
                        result[5] != null ? Convert.ToDateTime(result[5]) : new DateTime?(),
                        Convert.ToInt64(res[6]),
                        result[7] != null ? Convert.ToInt64(result[7]) : new long?(),
                        res[8].ToString(),
                        res[9].ToString(),
                        Convert.ToInt64(res[10]),
                        res[11].ToString(),
                        memberList
                        ));                   
                }
                return list;
            }

            public static void Insert(MeetingModel Meeting, List<EmployeeModel> MemberList)
            {              
                BaseDAO.Insert("Meetings", Columns.Fill(Meeting));
                MeetingModel meeting = SelectFirst(Columns.Fill(Meeting));
                long idMeeting = meeting.Id;
                foreach(EmployeeModel employee in MemberList)
                    BaseDAO.Insert("Members", MembersDAO.Columns.Fill(idMeeting,employee.Id));
            }

            public static void Update(int id, MeetingModel Meeting, List<EmployeeModel> MemberList)
            {
                BaseDAO.Update("Meetings", new KeyValuePair<string, object>(Columns.IdMeeting, id), Columns.Fill(Meeting));
                BaseDAO.Delete("Members", new Dictionary<string, object>()
                {
                    {"IdMeeting",id }
                });
                foreach (EmployeeModel employee in MemberList)
                {
                    BaseDAO.Insert("Members", MembersDAO.Columns.Fill(employee.Id, id));
                }
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
                        filler.Add(Confirmation, false);
                        return filler;
                    }
                }
            }
            }
        }
}