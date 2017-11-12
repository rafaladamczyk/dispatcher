using System.ComponentModel.DataAnnotations;

namespace Dispatcher.Models
{
    public class Machine
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }
    }
}