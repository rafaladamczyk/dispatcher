using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dispatcher.Models
{
    public class DispatchRequest
    {
        public int Id { get; set; }
        public RequestType Type { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public int RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public virtual DispatchRequester Requester { get; set; }
        
        [ConcurrencyCheck]
        public string ProviderName { get; set; }
        [ForeignKey("ProviderName")]
        public virtual ServiceProvider Provider { get; set; }
    }

    public enum RequestType
    {
        BringMaterials = 0,
        TakeAwayProduct = 1,
    }
}