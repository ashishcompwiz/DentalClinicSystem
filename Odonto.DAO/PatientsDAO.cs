using Dapper;
using Npgsql;
using Odonto.Models;
using System;
using System.Collections.Generic;

namespace Odonto.DAO
{
    public class PatientsDAO
    {
        private string strConnection;

        public PatientsDAO(string strConn){
            strConnection = strConn;
        }

        public List<Patient> GetAll(int clinicId)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<Patient>("SELECT * FROM Patients LEFT JOIN Persons ON Patients.ID = Persons.ID WHERE Persons.ClinicID = @ClinicID ORDER BY Name", new { ClinicID = clinicId }).AsList();
                return list;
            }
        }

        public List<Patient> GetFiltered(int clinicId, string name, string cpf)
        {
            string query = "SELECT * FROM Patients LEFT JOIN Persons ON Patients.ID = Persons.ID WHERE Persons.ClinicID = " + clinicId;
            query += !string.IsNullOrEmpty(name) ? " AND Name LIKE '%" + name + "%'" : "";
            query += !string.IsNullOrEmpty(cpf) ? " AND CPF LIKE '%" + cpf + "%'" : "";
            query += " ORDER BY Name";

            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<Patient>(query).AsList();
                return list;
            }
        }

        public List<Patient> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<Patient>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Patients) As Patients WHERE Patients.RowIndex > " + IndexStart + " AND Patients.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public Patient GetById(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Patient>("SELECT * FROM Patients LEFT JOIN Persons ON Patients.ID = Persons.ID WHERE Patients.ID = @ID", new { ID = ID });
            }
        }

        public int Add(Patient Patient)
        {
            try
            {
                PersonsDAO PersonsDAO = new PersonsDAO(strConnection);
                Patient.CreatedOn = DateTime.Now;
                Patient.UpdatedOn = DateTime.Now;

                Person person = Patient.GetBase();
                int insertedId = PersonsDAO.Add(person);
                if (insertedId <= 0)
                    return insertedId;
                Patient.ID = insertedId;
            }
            catch (Exception e)
            {
                return 0;
            }
            
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Patients (ID, Profession)
                                        VALUES (@ID, @Profession)",
                                        new
                                        {
                                            ID = Patient.ID,
                                            Profession = Patient.Profession,
                                        });
                return Patient.ID;
            }
        }

        public int Edit(Patient Patient)
        {
            PersonsDAO PersonsDAO = new PersonsDAO(strConnection);
            Patient.UpdatedOn = DateTime.Now;
            Person person = Patient.GetBase();
            int edited = PersonsDAO.Edit(person);
            if (edited <= 0)
                return edited;

            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Patients SET Profession = @Profession
                                        WHERE ID=@ID",
                                        new
                                        {
                                            ID = Patient.ID,
                                            Profession = Patient.Profession,
                                        });
                return resp;
            }
        }

        public bool Remove(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                sql.Execute("DELETE FROM Patients WHERE ID = @ID", new { ID = ID });
                var resp = sql.Execute("DELETE FROM Person WHERE ID = @ID", new { ID = ID });
                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Patients");
            }
        }
    }
}