using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Pobiera listę spotkań danego dnia dla danego użytkownika.
        /// </summary>
        /// <param name="id">Identyfikator użytkownika.</param>
        /// <param name="dateFrom">Data szukanych spotkań</param>
        /// <returns></returns>
        public IActionResult GetMeetingsForUser(long id, DateTime dateFrom)
        {
            List<MeetingModel> lista = MeetingDAO.Select(new Dictionary<string, object>()
            {
                {"IdEmployee", String.Format("'{0}'",id) },
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
                    result[0][11].ToString());
            }

            public static List<MeetingModel> Select(Dictionary<string, object> filters)
            {
                List<MeetingModel> list = new List<MeetingModel>();

                List<object[]> result = BaseDAO.SelectWithOutWhereClause(String.Format("Meetings Mt JOIN Members Mb ON Mb.IdMeeting = Mt.IdMeeting WHERE (Mt.IdAuthor LIKE {0} OR Mb.IdEmployee LIKE {0}) AND Mt.DateFrom = '{1}'",filters["IdEmployee"],filters["DateFrom"]), null);
                foreach (object[] res in result)
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
                        res[11].ToString()));

                return list;
            }

            public static void Insert(MeetingModel Meeting)
            {
                BaseDAO.Insert("Meetings", Columns.Fill(Meeting));
            }

            public static void Update(int id, MeetingModel Meeting)
            {
                BaseDAO.Update("Meetings", new KeyValuePair<string, object>(Columns.IdMeeting, id), Columns.Fill(Meeting));
            }
        }
    }
}