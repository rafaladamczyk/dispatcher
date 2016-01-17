
using System.ComponentModel.DataAnnotations;

namespace Dispatcher.Models
{
    public class ServiceProvider
    {
        [Key]
        public string Name { get; set; }
    }
}