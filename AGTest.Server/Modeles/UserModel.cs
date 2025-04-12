using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AGTest.Server.Modeles
{
    public class UserModel
    {
        public SQLiteConnection Connection;
        public int? Id;
        public string Username;
        public string Email;

        public UserModel(string username, string email)
        {
            Connection = new SQLiteConnection("Data Source=database.db");
            Username = username;
            Email = email;
        }

        public bool Save()
        {
            try
            {
                Connection.Open();
                using var command = Connection.CreateCommand();

                if (!Id.HasValue)
                {
                    command.CommandText = "INSERT INTO Users (Username, Email) VALUES (@username, @email)";
                    command.Parameters.AddWithValue("@username", Username);
                    command.Parameters.AddWithValue("@email", Email);
                    command.ExecuteNonQuery();
                    Id = (int)Connection.LastInsertRowId;
                }
                else
                {
                    command.CommandText = "UPDATE Users SET Username = @username, Email = @email WHERE Id = @id";
                    command.Parameters.AddWithValue("@username", Username);
                    command.Parameters.AddWithValue("@email", Email);
                    command.Parameters.AddWithValue("@id", Id.Value);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user: {ex.Message}");
                return false;
            }
        }

        public bool ValidateEmail()
        {
            string pattern = @"^[\w\.-]+@[\w\.-]+\.\w+$";
            return Regex.IsMatch(Email, pattern);
        }

        public string ToJson()
        {
            var data = new
            {
                Id,
                Username,
                Email,
                IsAdmin = Email.EndsWith("@admin.com")
            };
            return JsonSerializer.Serialize(data);
        }

        public static UserModel FindById(int id)
        {
            using var conn = new SqliteConnection("Data Source=database.db");
            conn.Open();
            using var command = conn.CreateCommand();
            command.CommandText = "SELECT Username, Email FROM Users WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var user = new UserModel(
                    reader.GetString(0),
                    reader.GetString(1)
                );
                user.Id = id;
                return user;
            }
            return null;
        }
    }
}
