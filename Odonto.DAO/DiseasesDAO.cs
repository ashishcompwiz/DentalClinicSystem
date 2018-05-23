using Dapper;
using Odonto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Odonto.DAO
{
    public class DiseasesDAO
    {
        private string strConnection;

        public DiseasesDAO(string strConn)
        {
            strConnection = strConn;
        }

        public List<Disease> GetAll()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Disease>("SELECT * FROM Diseases ORDER BY ID DESC").AsList();
                return list;
            }
        }

        public List<Disease> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Disease>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Diseases) As Diseases WHERE Diseases.RowIndex > " + IndexStart + " AND Diseases.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public Disease Get(int ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Disease>("SELECT * FROM Diseases WHERE ID = @ID", new { ID = ID });
            }
        }

        public bool Add(Disease Disease)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Diseases (ID,ClinicID,Name)
                                        VALUES (@ID,@ClinicID,@Name)",
                                        new
                                        {
                                            ClinicID = Disease.ClinicID,
                                            Name = Disease.Name
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Edit(Disease Disease)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Diseases SET ID = @ID,ClinicID = @ClinicID,Name = @Name
                                            WHERE ID=@ID",
                                        new
                                        {
                                            ID = Disease.ID,
                                            ClinicID = Disease.ClinicID,
                                            Name = Disease.Name
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Remove(string ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM Diseases WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Diseases");
            }
        }
    }
}