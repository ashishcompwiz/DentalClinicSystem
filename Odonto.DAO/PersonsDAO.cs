using Dapper;
using Odonto.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Odonto.DAO
{
    public class PersonsDAO
    {
        private string strConnection;

        public PersonsDAO(string strConn)
        {
            strConnection = strConn;
        }

        public List<Person> GetAll()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Person>("SELECT * FROM Persons ORDER BY ID DESC").AsList();
                return list;
            }
        }

        public List<Person> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new SqlConnection(strConnection))
            {
                var list = sql.Query<Person>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Persons) As Persons WHERE Persons.RowIndex > " + IndexStart + " AND Persons.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public Person GetById(int ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<Person>("SELECT * FROM Persons WHERE ID = @ID", new { ID = ID });
            }
        }

        public int Add(Person Person)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.ExecuteScalar<int>(@"INSERT INTO Persons (ClinicID,CPF,Name,LastName,Sex,CEP,Address,Number,City,State,Phone,Phone2,BirthDate,CreatedOn,UpdatedOn,CreatedBy,UpdatedBy)
                                    OUTPUT INSERTED.ID
                                    VALUES (@ClinicID,@CPF,@Name,@LastName,@Sex,@CEP,@Address,@Number,@City,@State,@Phone,@Phone2,@BirthDate,@CreatedOn,@UpdatedOn,@CreatedBy,@UpdatedBy)",
                                        new
                                        {
                                            ClinicID = Person.ClinicID,
                                            CPF = Person.CPF,
                                            Name = Person.Name,
                                            LastName = Person.LastName,
                                            Sex = Person.Sex,
                                            CEP = Person.CEP,
                                            Address = Person.Address,
                                            Number = Person.Number,
                                            City = Person.City,
                                            State = Person.State,
                                            Phone = Person.Phone,
                                            Phone2 = Person.Phone2,
                                            BirthDate = Person.BirthDate,
                                            CreatedOn = Person.CreatedOn,
                                            UpdatedOn = Person.UpdatedOn,
                                            CreatedBy = Person.CreatedBy,
                                            UpdatedBy = Person.UpdatedBy
                                        });
                return resp;
            }
        }

        public bool Edit(Person Person)
        {
            Person.UpdatedOn = DateTime.Now;

            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Persons SET ClinicID = @ClinicID,CPF = @CPF,Name = @Name,LastName = @LastName,Sex = @Sex,CEP = @CEP,Address = @Address,Number = @Number,City = @City,State = @State,Phone = @Phone,Phone2 = @Phone2,BirthDate = @BirthDate,UpdatedOn = @UpdatedOn,UpdatedBy = @UpdatedBy
                                        WHERE ID=@ID",
                                        new
                                        {
                                            ID = Person.ID,
                                            ClinicID = Person.ClinicID,
                                            CPF = Person.CPF,
                                            Name = Person.Name,
                                            LastName = Person.LastName,
                                            Sex = Person.Sex,
                                            CEP = Person.CEP,
                                            Address = Person.Address,
                                            Number = Person.Number,
                                            City = Person.City,
                                            State = Person.State,
                                            Phone = Person.Phone,
                                            Phone2 = Person.Phone2,
                                            BirthDate = Person.BirthDate,
                                            UpdatedOn = Person.UpdatedOn,
                                            UpdatedBy = Person.UpdatedBy
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Remove(string ID)
        {
            using (var sql = new SqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM Persons WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new SqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Persons");
            }
        }
    }

}
