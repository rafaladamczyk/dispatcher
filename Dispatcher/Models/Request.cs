using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dispatcher.Data;

namespace Dispatcher.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Index]
        public bool Active { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public DateTime? PickedUpDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        [NotMapped]
        public TimeSpan? Duration { get { return TimeSpan.FromTicks(DurationTicks); } set { DurationTicks = value?.Ticks ?? 0; } }
        public long DurationTicks { get; set; }

        [NotMapped]
        public TimeSpan? ServiceDuration { get { return TimeSpan.FromTicks(ServiceDurationTicks); } set { ServiceDurationTicks = value?.Ticks ?? 0; } }
        public long ServiceDurationTicks { get; set; }

        public int TypeId { get; set; }

        [Required]
        [ForeignKey("TypeId")]
        public virtual RequestType Type { get; set; }
        
        public string CreatorId { get; set; }

        [Required]
        [ForeignKey("CreatorId")]
        public virtual ApplicationUser Creator { get; set; }
        
        public string ProviderId { get; set; }

        [ForeignKey("ProviderId")]
        public virtual ApplicationUser Provider { get; set; }
    }
}