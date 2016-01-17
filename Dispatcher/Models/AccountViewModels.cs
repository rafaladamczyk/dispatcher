using System;
using System.Collections.Generic;

namespace Dispatcher.Models
{
    // Models returned by AccountController actions.

    public class UserInfoViewModel
    {
        public string UserName { get; set; }

        public bool HasRegistered { get; set; }
    }
}
