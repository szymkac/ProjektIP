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

        public static class MeetingDAO
        {
            public static class Columns
            {
                public static string IdMeeting = "IdMeeting";
                public static string IdMeetingType = "IdMeetingType";
                public static string DateFrom = "DateFrom";
                public static string DateTo = "DateTo";
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
                List<object[]> result = BaseDAO.Select("Meeting", null, null);
                return new MeetingModel(Convert.ToInt64(result[0][0]), Convert.ToInt64(result[0][1]), Convert.ToDateTime(result[0][2]), Convert.ToDateTime(result[0][3]), Convert.ToInt64(result[0][4]), Convert.ToInt64(result[0][5]), result[0][6].ToString(), result[0][7].ToString(), Convert.ToInt64(result[0][8]), result[0][9].ToString());
            }

            public static List<MeetingModel> Select(Dictionary<string, object> filters)
            {
                List<MeetingModel> list = new List<MeetingModel>();

                List<object[]> result = BaseDAO.Select("Task", null, null);
                foreach (object[] res in result)
                    list.Add(new MeetingModel(Convert.ToInt64(res[0]), Convert.ToInt64(res[1]), Convert.ToDateTime(res[2]), Convert.ToDateTime(res[3]), Convert.ToInt64(res[4]), Convert.ToInt64(res[5]), res[6].ToString(), res[7].ToString(), Convert.ToInt64(res[8]), res[9].ToString()));

                return list;
            }

            public static void Insert(MeetingModel Meeting)
            {
                BaseDAO.Insert("Meeting", Columns.Fill(Meeting));
            }

            public static void Update(int id, MeetingModel Meeting)
            {
                BaseDAO.Update("Meeting", new KeyValuePair<string, object>(Columns.IdMeeting, id), Columns.Fill(Meeting));
            }
        }
    }
}