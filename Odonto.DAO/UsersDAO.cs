using Dapper;
using Npgsql;
using Odonto.Models;
using System;
using System.Collections.Generic;

namespace Odonto.DAO
{
    public class UsersDAO
    {
        private string strConnection;

        public UsersDAO(string strConn){
            strConnection = strConn;
        }

        public List<User> GetAll()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<User>("SELECT * FROM Users").AsList();
                return list;
            }
        }

        public List<User> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<User>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM Users) As Users WHERE Users.RowIndex > " + IndexStart + " AND Users.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public User GetById(string ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE ID = @ID", new { ID });
            }
        }

        public User GetByEmail(string email)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @email", new { email });
            }
        }

        public bool Add(User User)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO Users (ID,Active,Email,Password,Type)
                                        VALUES (@ID,@Active,@Email,@Password,@Type)",
                                        new
                                        {
                                            ID = User.ID,
                                            Active = User.Active,
                                            Email = User.Email,
                                            Password = User.Password,
                                            Type = User.Type
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Edit(User User)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE Users SET Active = @Active, Email = @Email, Password = @Password, Type = @Type
                                            WHERE ID=@ID",
                                        new
                                        {
                                            Active = User.Active,
                                            Email = User.Email,
                                            Password = User.Password,
                                            Type = User.Type
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Remove(string ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM Users WHERE ID = @ID", new { ID = ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Users");
            }
        }
    }
}