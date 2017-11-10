using System.Collections.Generic;

namespace Dispatcher.Models
{
    // Models returned by AccountController actions.

    public class UserInfoViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ISet<string> Roles { get; set; }
    }
}
