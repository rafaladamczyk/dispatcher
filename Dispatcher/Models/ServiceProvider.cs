
using System.ComponentModel.DataAnnotations;

namespace Dispatcher.Models
{
    public class ServiceProvider
    {
        [Key]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}