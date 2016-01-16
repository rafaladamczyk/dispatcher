﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dispatcher.Models
{
    public class DispatchRequest
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime CompletionDate { get; set; }

        public int RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public DistpachRequester Requester { get; set; }
        
        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public ServiceProvider Provider { get; set; }
    }
}