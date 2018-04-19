using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIP.Models
{
	public class MeetingModel
	{
		public long Id { get; set; }

		public long MeetingTypeId { get; set; }
		public string MeetingTypeName { get; set; }

		public DateTime DateStart { get; set; }

		public DateTime? DateEnd { get; set; }

		public TimeSpan HourStart { get; set; }

		public TimeSpan? HourEnd { get; set; }

		public long EmployeeAuthorId { get; set; }

		public string EmployeeAuthorName { get; set; }

		public long? RoomId { get; set; }

		public string RoomName { get; set; }

		public string Location { get; set; }

        public string Note { get; set; }

		public long PriorityId { get; set; }

		public string PriorityName { get; set; }
		[Required]
		public string Title { get; set; }

		public List<EmployeeModel> Members { get; set; }

		public MeetingModel(long id, long meetingTypeId, string meetingTypeName, DateTime dateStart, DateTime? dateEnd, TimeSpan hourStart, TimeSpan? hourEnd, long employeeAuthorId, string employeeAuthorName, long? roomId, string roomName, string location, string note, long priorityId, string priorityName, string title, List<EmployeeModel> members)
		{
			Id = id;
			MeetingTypeId = meetingTypeId;
			MeetingTypeName = meetingTypeName;
			DateStart = dateStart;
			DateEnd = dateEnd;
			HourStart = hourStart;
			HourEnd = hourEnd;
			EmployeeAuthorId = employeeAuthorId;
            EmployeeAuthorName = employeeAuthorName;
            RoomId = roomId;
			RoomName = roomName;
			Location = location;
			Note = note;
			PriorityId = priorityId;
			PriorityName = priorityName;
			Title = title;
            Members = members;
        }

		/// <summary>
		/// Utworzone na potrzeby kontrolera, bez dodatkowych pól
		/// </summary>
		public MeetingModel(long id, long meetingTypeId, DateTime dateStart, DateTime? dateEnd, TimeSpan hourStart, TimeSpan? hourEnd, long employeeAuthorId, long? roomId, string location, string note, long priorityId, string title, List<EmployeeModel> members)
		{
			Id = id;
			MeetingTypeId = meetingTypeId;
			DateStart = dateStart;
			DateEnd = dateEnd;
			HourStart = hourStart;
			HourEnd = hourEnd;
			EmployeeAuthorId = employeeAuthorId;
            RoomId = roomId;
			Location = location;
			Note = note;
			PriorityId = priorityId;
			Title = title;
			Members = members;
		}

	}
}
