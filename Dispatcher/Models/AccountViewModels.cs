using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;

namespace Dispatcher.Models
{
    // Models returned by AccountController actions.

    public class UserInfoViewModel
    {
        public string Name { get; set; }

        public IReadOnlyCollection<string> Roles { get; set; }
    }
}
