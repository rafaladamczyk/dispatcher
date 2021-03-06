﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Dispatcher.Models
{
    public class DispatchRequest
    {
        public int Id { get; set; }
        [Index]
        public bool Active { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? PickedUpDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        [NotMapped]
        public TimeSpan? Duration { get { return TimeSpan.FromTicks(DurationTicks); } set { DurationTicks = value?.Ticks ?? 0; } }
        [JsonIgnore]
        public long DurationTicks { get; set; }
        [NotMapped]
        public TimeSpan? ServiceDuration { get { return TimeSpan.FromTicks(ServiceDurationTicks); } set { ServiceDurationTicks = value?.Ticks ?? 0; } }
        [JsonIgnore]
        public long ServiceDurationTicks { get; set; }
        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual DispatchRequestType Type { get; set; }
        [ConcurrencyCheck]
        public string ProvidingUserName { get; set; }
        public string RequestingUserName { get; set; }
    }
}