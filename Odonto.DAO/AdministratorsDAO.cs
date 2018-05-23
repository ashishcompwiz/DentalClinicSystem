using Dapper;
using Odonto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Odonto.DAO
{
    public class AdministratorsDAO
    {
        private string strConnection;

        public AdministratorsDAO(string strConn){
            strConnection = strConn;
        }

        public List<Administrator> GetAll()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Administrator>("SELECT * FROM Administrators ORDER BY ID DESC").AsList();
                return list;
            }
        }

        public List<Administrator> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Administrator>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Administrators) As Administrators WHERE Administrators.RowIndex > " + IndexStart + " AND Administrators.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public Administrator Get(int ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Administrator>("SELECT * FROM Administrators WHERE ID = @ID", new { ID = ID });
            }
        }

        public bool Add(Administrator Administrator)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Administrators (Position)
                                        VALUES (@Position)",
                                        new
                                        {
                                            Position = Administrator.Position
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Edit(Administrator Administrator)
        {
            Administrator.UpdatedOn = DateTime.Now;

            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Administrators SET Position = @Position
                                            WHERE ID=@ID",
                                        new
                                        {
                                            Position = Administrator.Position
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Remove(string ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM Administrators WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Administrators");
            }
        }
    }
}