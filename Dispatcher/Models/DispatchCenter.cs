using System.Collections.Generic;

namespace Dispatcher.Models
{
    public class DispatchCenter
    {
        public int Id { get; set; }

        ICollection<DispatchRequest> requests { get; set; }
    }
}