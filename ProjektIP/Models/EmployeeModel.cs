using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIP.Models
{
	public class EmployeeModel
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string SurName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

		public string Phone { get; set; }

		public bool Active { get; set; }

		public long? PositionId { get; set; }

		public string PositionName { get; set; }

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
    }
}
