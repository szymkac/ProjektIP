using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIP.Models
{
	public class EmployeeModel
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string SurName { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public bool Active { get; set; }

		public long PositionId { get; set; }

		public string PositionName { get; set; }

		public EmployeeModel(long id, string name, string surName, string email, string phone, bool active, long positionId, string positionName)
		{
			Id = id;
			Name = name;
			SurName = surName;
			Email = email;
			Phone = phone;
			Active = active;
			PositionId = positionId;
			PositionName = positionName;
		}
	}
}
