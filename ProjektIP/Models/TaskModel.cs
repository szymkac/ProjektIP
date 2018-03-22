﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIP.Models
{
    public class TaskModel
    {
		public long Id { get; set; }

		public long TaskTypeId { get; set; }

		public string TaskTypeName { get; set; }

		public long EmployeeId { get; set; }

		public string Title { get; set; }

		public DateTime DateStart { get; set; }

		public DateTime DateEnd { get; set; }

		public string Comment { get; set; }

		public long StatusId { get; set; }

		public string StatusName { get; set; }

		public long PriorityId { get; set; }

		public string PriorityName { get; set; }

		public TaskModel(long id, long taskTypeId, string taskTypeName, long employeeId, string title, DateTime dateStart, DateTime dateEnd, string comment, long statusId, string statusName, long priorityId, string priorityName)
		{
			Id = id;
			TaskTypeId = taskTypeId;
			TaskTypeName = taskTypeName;
			EmployeeId = employeeId;
			Title = title;
			DateStart = dateStart;
			DateEnd = dateEnd;
			Comment = comment;
			StatusId = statusId;
			StatusName = statusName;
			PriorityId = priorityId;
			PriorityName = priorityName;
		}
	}
}