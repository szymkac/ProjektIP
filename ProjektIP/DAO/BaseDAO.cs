using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace ProjektIP.DAO
{
	public static class BaseDAO
	{

		/// <summary>
		/// Zapytanie SELECT
		/// </summary>
		/// <param name="table">Nazwa tabeli</param>
		/// <param name="columns">Lista kolumn, które mają być odczytane</param>
		/// <param name="filter">Słownik filtów, gdzie kluczem jest nazwa parametru</param>
		/// <returns>Metoda zwraca tablice obiektów [i,k] i-zwrócone wiersze, k-kolumny</returns>
		public static List<object[]> Select(string table, List<string> columns, Dictionary<string, object> filters)
		{
			List<object[]> result = null;
			using (SqlConnection conn = new SqlConnection(Startup.StaticConfiguration.GetConnectionString("DefaultConnection")))
			{
				using (SqlCommand command = new SqlCommand("", conn))
				{
					string cols = "";
					if (columns != null && columns.Count > 0)
					{
						for (int i = 0; i < columns.Count; i++)
						{
							if (i == columns.Count - 1)
								cols += columns[i];
							else
								cols += columns[i] + ", ";
						}
					}
					else
						cols = "*";


					string sqlQuery = String.Format("SELECT {0} FROM {1}", cols, table);

					sqlQuery += GetWhereClause(filters);

					conn.Open();
					command.CommandText = sqlQuery;
					SqlDataReader reader = command.ExecuteReader();
					result = new List<object[]>();

					while (reader.Read())
					{
						object[] res = new object[reader.FieldCount];
						for (int i = 0; i < reader.FieldCount; i++)
							res[i] = reader[i];
						result.Add(res);
					}

					conn.Close();
				}
			}
			return result;
		}

        public static List<object[]> SelectWithOutWhereClause(string sqlQuery)
        {
            List<object[]> result = null;
            using (SqlConnection conn = new SqlConnection(Startup.StaticConfiguration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    conn.Open();
                    command.CommandText = sqlQuery;
                    SqlDataReader reader = command.ExecuteReader();
                    result = new List<object[]>();

                    while (reader.Read())
                    {
                        object[] res = new object[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                            res[i] = reader[i];
                        result.Add(res);
                    }
                    conn.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Metoda INSERT
        /// </summary>
        /// <param name="table">Nazwa tabeli</param>
        /// <param name="values">Słownik wartości, gdzie kluczem jest nazwa kolumny</param>
        public static void Insert(string table, Dictionary<string, object> values)
		{
			using (SqlConnection conn = new SqlConnection(Startup.StaticConfiguration.GetConnectionString("DefaultConnection")))
			{
				using (SqlCommand command = new SqlCommand("", conn))
				{

					string columns = "";
					string value = "";

					if (values != null && values.Count > 0)
					{

						int i = 0;
						foreach (KeyValuePair<string, object> pair in values)
						{
							if (i != values.Count - 1)
								columns += pair.Key + ",";
							else
								columns += pair.Key;

							if (pair.Value != null)
							{
                                object value_obj = pair.Value;
                                if (pair.Key.Contains("Date"))
                                {
                                    DateTime date = (DateTime)pair.Value;
                                    value_obj = String.Format(
                                        "{1}-{0}-{2}",
                                        date.Day.ToString().Length == 1 ? "0" + date.Day.ToString() : date.Day.ToString(),
                                        date.Month.ToString().Length == 1 ? "0" + date.Month.ToString() : date.Month.ToString(),
                                        date.Year);
                                }
                                Type t = value_obj.GetType();
								if (value_obj.GetType() == Type.GetType("System.String") || value_obj.GetType() == Type.GetType("System.Char") || value_obj.GetType() == Type.GetType("System.DateTime") || value_obj.GetType() == Type.GetType("System.TimeSpan"))
                                {
									if (i != values.Count - 1)
										value += "'" + value_obj + "'" + ",";
									else
										value += "'" + value_obj + "'";

								}
								else
								{
                                    if (value_obj.GetType() == Type.GetType("System.Boolean"))
                                    {
                                        int? val = null;
                                        if ((Boolean)value_obj == false)
                                            val = 0;
                                        else if((Boolean)value_obj == true)
                                            val = 1;                            
                                        if (i != values.Count - 1)
                                            value += val.ToString() + ",";
                                        else
                                            value += val.ToString();
                                    }
                                    else
                                    {
                                        if (i != values.Count - 1)
                                            value += value_obj + ",";
                                        else
                                            value += value_obj;
                                    }
								}
							}
							else
							{
								if (i != values.Count - 1)
									value += "null,";
								else
									value += "null";
							}

							i++;
						}
					}

					string sqlQuery = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", table, columns, value);

					conn.Open();
					command.CommandText = sqlQuery;
					command.ExecuteNonQuery();

					conn.Close();
				}
			}
		}

		/// <summary>
		/// Metoda UPDATE
		/// </summary>
		/// <param name="table">Nazwa tabeli</param>
		/// <param name="id">Identyfikator modyfikowanego pola</param>
		/// <param name="values">Słownik wartości, gdzie kluczem jest nazwa kolumny. Jeśli wartość ma pozostać bez zmiany, podać null</param>
		public static void Update(string table, KeyValuePair<string, object> id, Dictionary<string, object> values)
		{
			using (SqlConnection conn = new SqlConnection(Startup.StaticConfiguration.GetConnectionString("DefaultConnection")))
			{
				using (SqlCommand command = new SqlCommand("", conn))
				{
					string columns = string.Empty;

                    int i = 0;
					foreach (KeyValuePair<string, object> pair in values)
					{

                        if (pair.Value != null)
						{
                            object value = pair.Value;
                            if (pair.Key.Contains("Date"))
                            {
                                DateTime date = (DateTime)pair.Value;
                                value = String.Format(
                                    "{1}-{0}-{2}",
                                    date.Day.ToString().Length == 1 ? "0" + date.Day.ToString() : date.Day.ToString(),
                                    date.Month.ToString().Length == 1 ? "0" + date.Month.ToString() : date.Month.ToString(),
                                    date.Year);
                            }
                            if (i == values.Count - 1)
                            {
                                if (value.GetType() == Type.GetType("System.String") || value.GetType() == Type.GetType("System.Char") || value.GetType() == Type.GetType("System.DateTime") || value.GetType() == Type.GetType("System.TimeSpan"))
                                {
                                    if(value.GetType() == Type.GetType("System.DateTime") && (DateTime)value == new DateTime())
                                        columns += pair.Key + " = null";
                                    else
                                        columns += pair.Key + " = '" + value + "'";
                                }
                                else
                                {
                                    if (value.GetType() == Type.GetType("System.Boolean"))
                                    {
                                        int? val = null;
                                        if ((Boolean)value == false)
                                            val = 0;
                                        else if ((Boolean)value == true)
                                            val = 1;
                                     
                                            columns += pair.Key + " = " + val.ToString();
                                    }
                                    else
                                    {
                                        columns += pair.Key + " = " + value;
                                    }
                                }
                            }
                            else
                            {
                                if (value.GetType() == Type.GetType("System.String") || value.GetType() == Type.GetType("System.Char") || value.GetType() == Type.GetType("System.DateTime") || value.GetType() == Type.GetType("System.TimeSpan"))
                                {
                                    if (value.GetType() == Type.GetType("System.DateTime") && (DateTime)value == new DateTime())
                                        columns += pair.Key + " = null,";
                                    else
                                        columns += pair.Key + " = '" + value + "',";
                                }
                                else
                                {
                                    if (value.GetType() == Type.GetType("System.Boolean"))
                                    {
                                        int? val = null;
                                        if ((Boolean)value == false)
                                            val = 0;
                                        else if ((Boolean)value == true)
                                            val = 1;

                                        columns += pair.Key + " = " + val + ",";
                                    }
                                    else
                                    columns += pair.Key + " = " + value + ",";
                                }
                            }
						}
                        i++;
					}
					if (columns.Length > 0)
						columns.Remove(columns.Length - 1);

					string sqlQuery = String.Format("UPDATE {0} SET {1} WHERE {2} = {3} ", table, columns, id.Key, id.Value);

					conn.Open();
					command.CommandText = sqlQuery;
					command.ExecuteNonQuery();

					conn.Close();
				}
			}
		}

        public static void Delete(string table, Dictionary<string, object> filters)
        {
            using (SqlConnection conn = new SqlConnection(Startup.StaticConfiguration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    string sqlQuery = String.Format("DELETE FROM {0} {1} ", table, GetWhereClause(filters));

                    conn.Open();
                    command.CommandText = sqlQuery;
                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public static string GetWhereClause(Dictionary<string, object> filters)
		{
			string clause = string.Empty;
			if (filters != null && filters.Count > 0)
			{
				string flt = "";
				int i = 0;
				foreach (KeyValuePair<string, object> pair in filters)
				{
                    object value = pair.Value;
                    if(pair.Key.Contains("Date"))
                    {
                        DateTime date = (DateTime)pair.Value;
                        value = String.Format(
                            "{1}-{0}-{2}", 
                            date.Day.ToString().Length == 1 ? "0"+ date.Day.ToString() : date.Day.ToString(),
                            date.Month.ToString().Length == 1 ? "0" + date.Month.ToString() : date.Month.ToString(),
                            date.Year);
                    }
					if (pair.Value.GetType() == Type.GetType("System.String") || pair.Value.GetType() == Type.GetType("System.Char") || pair.Value.GetType() == Type.GetType("System.DateTime") || pair.Value.GetType() == Type.GetType("System.TimeSpan"))
                    {
                        
                            if (i == 0)
                                flt += pair.Key + " = '" + value + "'";
                            else
                                flt += " AND " + pair.Key + " = '" + value + "'";
                        
                       
					}
					else
					{
                        if (pair.Value.GetType() == Type.GetType("System.Boolean"))
                        {
                            int? val = null;
                            if ((Boolean)value == false)
                                val = 0;
                            else if ((Boolean)value == true)
                                val = 1;
                            if (i == 0)
                                flt += pair.Key + " = " + val;
                            else
                                flt += " AND " + pair.Key + " = " + val;
                        }
                        else
                        {
                            if (i == 0)
                                flt += pair.Key + " = " + value;
                            else
                                flt += " AND " + pair.Key + " = " + value;
                        }
					}
					i++;
				}
				clause += String.Format(" WHERE {0}", flt);
			}
			return clause;
		}

	}
}
