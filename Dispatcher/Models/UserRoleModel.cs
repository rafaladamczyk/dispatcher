namespace Dispatcher.Models
{
    public class UserRoleModel
    {
        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsRequester { get; set; }

        public bool IsServiceProvider { get; set; }
    }
}