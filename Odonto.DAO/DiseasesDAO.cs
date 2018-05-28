using Dapper;
using Npgsql;
using Odonto.Models;
using System;
using System.Collections.Generic;

namespace Odonto.DAO
{
    public class DiseasesDAO
    {
        private string strConnection;

        public DiseasesDAO(string strConn)
        {
            strConnection = strConn;
        }

        public List<Disease> GetAll(int clinicId)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<Disease>("SELECT ID, Name FROM Diseases WHERE ClinicId = @clinicId ORDER BY Name", new { clinicId }).AsList();
                return list;
            }
        }

        public Disease GetById(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Disease>("SELECT * FROM Diseases WHERE ID = @ID", new { ID = ID });
            }
        }

        public bool Add(Disease Disease)
        {
            using (var sql = new NpgsqlConnection(strConnection))
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
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Diseases SET ClinicID = @ClinicID,Name = @Name
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

        public bool Remove(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM Diseases WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Diseases");
            }
        }
    }
}