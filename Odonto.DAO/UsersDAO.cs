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

        public List<User> GetAll(int clinicId)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<User>(@"SELECT Users.ID, Users.Type, Users.Active, Users.Email, UserType.Name As TypeName 
                                            FROM Users LEFT JOIN UserType ON Users.Type = UserType.ID
                                            WHERE EXISTS(SELECT 1 FROM Persons WHERE clinicId = @clinicId)", 
                                            new { clinicId }).AsList();
                return list;
            }
        }

        public User GetById(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<User>(@"SELECT Users.ID, Users.Type, Users.Active, Users.Email, UserType.Name As TypeName 
                                                        FROM Users LEFT JOIN UserType ON Users.Type = UserType.ID WHERE Users.ID = @ID", new { ID });
            }
        }

        public User GetByEmail(string email)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @email", new { email });
            }
        }

        public bool EmailRepeated(User user)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var exist = sql.QueryFirstOrDefault<int>("SELECT 1 FROM Users WHERE Email = @Email AND ID <> @ID", new { Email = user.Email, ID = user.ID });
                return Convert.ToBoolean(exist);
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
                var updateSQL = "UPDATE Users SET Active = @Active, Email = @Email, Type = @Type ";
                updateSQL += !string.IsNullOrEmpty(User.Password) ? ", Password = @Password " : string.Empty;
                updateSQL += "WHERE ID = @ID";

                var resp = sql.Execute(updateSQL,
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

        public IEnumerable<UserType> GetTypes()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                
                var list = sql.Query<UserType>(@"SELECT * FROM UserType").AsList();
                return list;
            }
        }
    }
}