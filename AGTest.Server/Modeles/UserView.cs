using Microsoft.Data.Sqlite;

namespace AGTest.Server.Modeles
{
    public class UserView
    {
        private readonly Dictionary<int, string> _cache = new();
        private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(5);

        public string DisplayUserProfile(UserModel user)
        {
            if (user.Email.EndsWith("@admin.com"))
            {
                return DisplayAdminProfile(user);
            }

            return $@"
                <div class='user-profile'>
                    <h1>{user.Username}</h1>
                    <p>{user.Email}</p>
                    {GenerateUserStats(user)}
                </div>
            ";
        }

        public string DisplayAdminProfile(UserModel user)
        {
            return $@"
                <div class='admin-profile'>
                    <h1>Admin: {user.Username}</h1>
                    <p>{user.Email}</p>
                    {GenerateAdminStats(user)}
                </div>
            ";
        }

        public string GenerateUserStats(UserModel user)
        {
            using var conn = new SQLiteConnection("Data Source=database.db");
            conn.Open();
            using var command = conn.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Posts WHERE UserId = @userId";
            command.Parameters.AddWithValue("@userId", user.Id);
            var postCount = (long)command.ExecuteScalar();
            return $"<p>Total posts: {postCount}</p>";
        }

        public string GenerateAdminStats(UserModel user)
        {
            return "<p>Admin stats placeholder</p>";
        }

        public string ExportUserDataToCsv(UserModel user)
        {
            return $"Username,Email\n{user.Username},{user.Email}";
        }
    }
}
