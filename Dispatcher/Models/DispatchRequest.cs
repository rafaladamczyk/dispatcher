using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dispatcher.Models
{
    public class DispatchRequest
    {
        public int Id { get; set; }
        [Index]
        public bool Active { get; set; }
        public RequestType Type { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PickedUpDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public TimeSpan? Duration { get; set; }
        public TimeSpan? ServiceDuration { get; set; }  

        public int RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public virtual DispatchRequester Requester { get; set; }

        public string ProvidingUserId { get; set; }

        [ForeignKey("ProvidingUserId")]
        [ConcurrencyCheck]
        public virtual ApplicationUser ProvidingUser { get; set; }
    }

    public enum RequestType
    {
        BringMaterials = 0,
        TakeAwayProduct = 1,
    }
}