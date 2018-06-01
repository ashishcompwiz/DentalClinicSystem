using Dapper;
using Npgsql;
using Odonto.Models;
using System;
using System.Collections.Generic;

namespace Odonto.DAO
{
    public class AdministratorsDAO
    {
        private string strConnection;

        public AdministratorsDAO(string strConn){
            strConnection = strConn;
        }

        public List<Administrator> GetAll(int clinicId)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<Administrator>("SELECT * FROM Administrators LEFT JOIN Persons ON Administrators.ID = Persons.ID WHERE Persons.ClinicID = @ClinicID ORDER BY Name", new { ClinicID = clinicId }).AsList();
                return list;
            }
        }

        public Administrator GetById(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Administrator>("SELECT * FROM Administrators LEFT JOIN Persons ON Administrators.ID = Persons.ID WHERE Administrators.ID = @ID", new { ID });
            }
        }

        public int Add(Administrator Administrator)
        {
            try
            {
                PersonsDAO PersonsDAO = new PersonsDAO(strConnection);
                Administrator.CreatedOn = DateTime.Now;
                Administrator.UpdatedOn = DateTime.Now;

                Person person = Administrator.GetBase();
                int insertedId = PersonsDAO.Add(person);
                if (insertedId < 0)
                    return insertedId;
                Administrator.ID = insertedId;
            }
            catch (Exception e)
            {
                return 0;
            }
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Administrators (ID, Position)
                                        VALUES (@ID, @Position)",
                                        new
                                        {
                                            ID = Administrator.ID,
                                            Position = Administrator.Position
                                        });
                return Administrator.ID;
            }
        }

        public int Edit(Administrator Administrator)
        {
            PersonsDAO PersonsDAO = new PersonsDAO(strConnection);
            Administrator.UpdatedOn = DateTime.Now;
            Person person = Administrator.GetBase();

            int edited = PersonsDAO.Edit(person);
            if (edited <= 0)
                return edited;

            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Administrators SET Position = @Position
                                            WHERE ID=@ID",
                                        new
                                        {
                                            ID = Administrator.ID,
                                            Position = Administrator.Position
                                        });
                return Administrator.ID;
            }
        }

        public bool Remove(string ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM Administrators WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Administrators");
            }
        }
    }
}