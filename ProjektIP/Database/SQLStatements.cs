﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace ProjektIP.Database
{
    public static class SQLStatements
    {
        public static bool CheckUser(string login, string password)
        {
            List<string> cols = new List<string>();
            cols.Add("Login");
            cols.Add("Password");

            Dictionary<string, object> filter = new Dictionary<string, object>();
            filter.Add("Login", login);
            filter.Add("Password", password);

            if (Select("Users", cols, filter).Count == 1)
                return true;

            return false;            
        }
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
                        for(int i = 0;i<columns.Count;i++)
                        {
                            if(i==columns.Count-1)
                                cols += columns[i] ;
                            else
                            cols += columns[i] + ", ";
                        }
                    }
                    else
                        cols = "*";


                    string sqlQuery = String.Format("SELECT {0} FROM {1}", cols, table);

                    if (filters != null && filters.Count > 0)
                    {
                        string flt = "";
                        int i = 0;
                        foreach (KeyValuePair<string, object> pair in filters)
                        {
                            if (pair.Value.GetType() == Type.GetType("System.String") || pair.Value.GetType() == Type.GetType("System.Char"))
                            {
                                if (i == 0)
                                    flt += pair.Key + " = '" + pair.Value+"'";
                                else
                                    flt += " AND " + pair.Key + " = '" + pair.Value+"'";
                            }
                            else
                            {
                                if (i == 0)
                                    flt += pair.Key + " = " + pair.Value;
                                else
                                    flt += " AND " + pair.Key + " = " + pair.Value;
                            }
                            i++;
                        }
                        sqlQuery += String.Format(" WHERE {0}", flt);
                    }

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
            List<object[]> result = null;
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


                            Type t = pair.Value.GetType();
                            if (pair.Value.GetType() == Type.GetType("System.String") || pair.Value.GetType() == Type.GetType("System.Char"))
                            {
                                if (i != values.Count - 1)
                                    value += "'"+pair.Value+"'" + ",";
                                else
                                    value += "'" + pair.Value + "'" ;
                              
                            }
                            else
                            {
                                if (i != values.Count - 1)
                                    value += pair.Value + ",";
                                else
                                    value += pair.Value;
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

    }
}
