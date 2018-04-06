using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjektIP.DAO;

namespace ProjektIP.Common
{
	public class User
	{
		public long Id;
		private string login;

		public User(string login, string password)
		{
			this.login = login;
			Dictionary<string, object> filter = new Dictionary<string, object>();
			filter.Add("Login", login);
			filter.Add("Password", password);
			List<object[]> list = BaseDAO.Select("Users", new List<string> { "IdUser", "Login", "Password", "IdEmployee" }, filter);
			this.Id = Convert.ToInt64(list[0][3]);
		}
	}
}
