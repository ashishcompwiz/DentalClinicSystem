using Dapper;
using Npgsql;
using Odonto.Models;
using System;
using System.Collections.Generic;

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
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<Dentist>("SELECT * FROM Dentists LEFT JOIN Persons ON Dentists.ID = Persons.ID WHERE Persons.ClinicID = @ClinicID ORDER BY Name", new { ClinicID = clinicId }).AsList();
                return list;
            }
        }

        public List<Dentist> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<Dentist>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Dentists) As Dentists WHERE Dentists.RowIndex > " + IndexStart + " AND Dentists.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public Dentist GetById(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
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
                if (insertedId < 0)
                    return insertedId;
                Dentist.ID = insertedId;
            }
            catch (Exception e)
            {
                return 0;
            }
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Dentists (ID,Specialty,CRO,Active)
                                        VALUES (@ID,@Specialty,@CRO,@Active)",
                                        new
                                        {
                                            ID = Dentist.ID,
                                            Specialty = Dentist.Specialty,
                                            CRO = Dentist.CRO,
                                            Active = Dentist.Active
                                        });
                return Dentist.ID;
            }
        }

        public int Edit(Dentist Dentist)
        {
            PersonsDAO PersonsDAO = new PersonsDAO(strConnection);
            Dentist.UpdatedOn = DateTime.Now;
            Person person = Dentist.GetBase();

            int edited = PersonsDAO.Edit(person);
            if (edited <= 0)
                return edited;

            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Dentists SET Specialty = @Specialty,CRO = @CRO,Active = @Active
                                        WHERE ID=@ID",
                                        new
                                        {
                                            ID = Dentist.ID,
                                            Specialty = Dentist.Specialty,
                                            CRO = Dentist.CRO,
                                            Active = Dentist.Active
                                        });
                return resp;
            }
        }

        public bool Remove(string ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                sql.Execute("DELETE FROM Dentists WHERE ID = @ID", new { ID = ID });
                var resp = sql.Execute("DELETE FROM Persons WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Dentists");
            }
        }
    }
}