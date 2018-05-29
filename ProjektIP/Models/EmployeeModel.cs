using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIP.Models
{
	public class EmployeeModel: IModel
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string SurName { get; set; }

        public string Email { get; set; }

		public string Phone { get; set; }

		public bool Active { get; set; }

		public long? PositionId { get; set; }

		public string PositionName { get; set; }

        public bool? Confirmation { get; set; }

        public EmployeeModel(long id, string name, string surName, string email, string phone, bool active, long? positionId, string positionName)
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
        public EmployeeModel(long id, string name, string surName, string email, string phone, bool active, long? positionId, string positionName, bool? confirmation)
        {
            Id = id;
            Name = name;
            SurName = surName;
            Email = email;
            Phone = phone;
            Active = active;
            PositionId = positionId;
            PositionName = positionName;
            Confirmation = confirmation;
        }

        public EmployeeModel(long id, string name, string surName, string email, string phone, bool active, long? positionId)
        {
            Id = id;
            Name = name;
            SurName = surName;
            Email = email;
            Phone = phone;
            Active = active;
            PositionId = positionId;
        }

        public EmployeeModel(long id, string name, string surName, string email, string phone, bool active, long? positionId, bool? confirmation)
        {
            Id = id;
            Name = name;
            SurName = surName;
            Email = email;
            Phone = phone;
            Active = active;
            PositionId = positionId;
            Confirmation = confirmation;
        }
    }
}
