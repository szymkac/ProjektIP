using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjektIP.DAO;
using ProjektIP.Models;

namespace ProjektIP.Common
{
	public class User:Model
	{
		public long Id;
		public string Login;
		public List<long> UserPermission;

		public User()
		{
			Id = 0;
		}

		public User(string login, string password)
		{
			this.Login = login;
			Dictionary<string, object> filter = new Dictionary<string, object>();
			filter.Add("Login", login);
			filter.Add("Password", password);
			List<object[]> list = BaseDAO.Select("Users", new List<string> { "IdUser", "Login", "Password", "IdEmployee" }, filter);
			this.Id = Convert.ToInt64(list[0][3]);
			List<object[]> permissions = BaseDAO.Select("EmployeePermissions", new List<string> { "IdEmployee", "IdPermission" }, new Dictionary<string, object>() { { "IdEmployee", Id } });
			List<long> userPermissions = permissions.Select(x => Convert.ToInt64(x[1])).ToList();
			UserPermission = userPermissions;
		}
	}
}
