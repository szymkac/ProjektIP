﻿using System;
using System.Collections.Generic;
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

		public DateTime DateEnd { get; set; }

		public long EmployeeAuthorId { get; set; }

		public long RoomId { get; set; }

		public string RoomName { get; set; }

		public string Location { get; set; }

		public string Note { get; set; }

		public long PriorityId { get; set; }

		public string PriorityName { get; set; }

		public string Title { get; set; }


		public MeetingModel(long id, long meetingTypeId, string meetingTypeName, DateTime dateStart, DateTime dateEnd, long employeeAuthorId, long roomId, string roomName, string location, string note, long priorityId, string priorityName, string title)
		{
			Id = id;
			MeetingTypeId = meetingTypeId;
			MeetingTypeName = meetingTypeName;
			DateStart = dateStart;
			DateEnd = dateEnd;
			EmployeeAuthorId = employeeAuthorId;
			RoomId = roomId;
			RoomName = roomName;
			Location = location;
			Note = note;
			PriorityId = priorityId;
			PriorityName = priorityName;
			Title = title;
		}

	}
}