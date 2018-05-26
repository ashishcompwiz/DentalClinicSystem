using Dapper;
using Odonto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Odonto.DAO
{
    public class DentistsDAO
    {
        private string strConnection;

        public DentistsDAO(string strConn)
        {
            strConnection = strConn;
        }

        public List<Dentist> GetAll(int clinicId)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Dentist>("SELECT * FROM Dentists LEFT JOIN Persons ON Dentists.ID = Persons.ID WHERE Persons.ClinicID = @ClinicID ORDER BY Name", new { ClinicID = clinicId }).AsList();
                return list;
            }
        }

        public List<Dentist> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Dentist>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Dentists) As Dentists WHERE Dentists.RowIndex > " + IndexStart + " AND Dentists.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public Dentist GetById(int ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Dentist>("SELECT * FROM Dentists LEFT JOIN Persons ON Dentists.ID = Persons.ID WHERE Dentists.ID = @ID", new { ID = ID });
            }
        }

        public int Add(Dentist Dentist)
        {
            try
            {
                PersonsDAO PersonsDAO = new PersonsDAO(strConnection);
                Dentist.CreatedOn = DateTime.Now;
                Dentist.UpdatedOn = DateTime.Now;

                Person person = Dentist.GetBase();
                int insertedId = PersonsDAO.Add(person);
                Dentist.ID = insertedId;
            }
            catch (Exception e)
            {
                return 0;
            }
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Dentists (ID,Specialty,CRO)
                                        VALUES (@ID,@Specialty,@CRO)",
                                        new
                                        {
                                            ID = Dentist.ID,
                                            Specialty = Dentist.Specialty,
                                            CRO = Dentist.CRO
                                        });
                return Dentist.ID;
            }
        }

        public bool Edit(Dentist Dentist)
        {
            PersonsDAO PersonsDAO = new PersonsDAO(strConnection);
            Dentist.UpdatedOn = DateTime.Now;
            Person person = Dentist.GetBase();
            if (PersonsDAO.Edit(person))
            {
                using (var sql = new SqlConnection(strConnection))
                {
                    var resp = sql.Execute(@"UPDATE Dentists SET Specialty = @Specialty,CRO = @CRO
                                            WHERE ID=@ID",
                                            new
                                            {
                                                ID = Dentist.ID,
                                                Specialty = Dentist.Specialty,
                                                CRO = Dentist.CRO
                                            });
                    return Convert.ToBoolean(resp);
                }
            }

            return false;
        }

        public bool Remove(string ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                sql.Execute("DELETE FROM Dentists WHERE ID = @ID", new { ID = ID });
                var resp = sql.Execute("DELETE FROM Persons WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Dentists");
            }
        }
    }
}