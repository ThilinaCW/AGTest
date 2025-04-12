using AGTest.Server.Modeles;
using Microsoft.AspNetCore.Mvc;

namespace AGTest.Server.Controllers
{
    public class UserController : Controller
    {
        private readonly UserView _view;
        private UserModel _model;
        private readonly Dictionary<string, bool> _notificationSettings;

        public UserController()
        {
            _view = new UserView();
            _notificationSettings = new Dictionary<string, bool>
            {
                { "EmailNotifications", true },
                { "SlackNotifications", false }
            };
        }

        public object CreateUser(string username, string email)
        {
            if (username.Length < 3)
            {
                return new { Success = false, Error = "Username too short" };
            }

            _model = new UserModel(username, email);
            if (_model.Save())
            {
                return new
                {
                    Success = true,
                    Html = _view.DisplayUserProfile(_model)
                };
            }
            return new { Success = false, Error = "Failed to save user" };
        }

        public object GetUserProfile(int userId)
        {
            var user = UserModel.FindById(userId);
            if (user != null)
            {
                _model = user;
                return new
                {
                    Success = true,
                    Html = _view.DisplayUserProfile(user)
                };
            }
            return new { Success = false, Error = "User not found" };
        }

        public object UpdateEmail(string newEmail)
        {
            if (_model == null)
            {
                return new { Success = false, Error = "No user selected" };
            }

            _model.Email = newEmail;
            if (_model.Save())
            {
                return new
                {
                    Success = true,
                    Html = _view.DisplayUserProfile(_model)
                };
            }
            return new { Success = false, Error = "Failed to update email" };
        }

        public bool SendWelcomeEmail(UserModel user)
        {
            if (!_notificationSettings["EmailNotifications"])
            {
                return false;
            }
            Console.WriteLine($"Sending welcome email to {user.Email}");
            return true;
        }

        public bool ValidatePasswordStrength(string password)
        {
            return password.Length >= 8 && password.Any(char.IsDigit);
        }
    }
}
