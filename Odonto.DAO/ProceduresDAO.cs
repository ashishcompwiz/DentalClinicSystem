using Dapper;
using Odonto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Odonto.DAO
{
    public class ProceduresDAO
    {
        private string strConnection;

        public ProceduresDAO(string strConn)
        {
            strConnection = strConn;
        }

        public List<Procedure> GetAll()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Procedure>("SELECT * FROM Procedures ORDER BY ID DESC").AsList();
                return list;
            }
        }

        public List<Procedure> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Procedure>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Procedures) As Procedures WHERE Procedures.RowIndex > " + IndexStart + " AND Procedures.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public Procedure Get(int ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Procedure>("SELECT * FROM Procedures WHERE ID = @ID", new { ID = ID });
            }
        }

        public bool Add(Procedure Procedure)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Procedures (ClinicID,Name,Description,Observation,Value)
                                        VALUES (@ClinicID,@Name,@Description,@Observation,@Value)",
                                        new
                                        {
                                            ClinicID = Procedure.ClinicID,
                                            Name = Procedure.Name,
                                            Description = Procedure.Description,
                                            Observation = Procedure.Observation,
                                            Value = Procedure.Value
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Edit(Procedure Procedure)
        {

            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Procedures SET ClinicID = @ClinicID,Name = @Name,Description = @Description,Observation = @Observation,Value = @Value
                                            WHERE ID=@ID",
                                        new
                                        {
                                            ClinicID = Procedure.ClinicID,
                                            Name = Procedure.Name,
                                            Description = Procedure.Description,
                                            Observation = Procedure.Observation,
                                            Value = Procedure.Value
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Remove(string ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM Procedures WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Procedures");
            }
        }
    }
}