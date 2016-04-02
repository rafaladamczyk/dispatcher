using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Dispatcher.Models
{
    public class TasksForProviderDto
    {
        public string Name { get; set; }
        public List<TaskDto> Tasks { get; set; }
        public List<TaskDto> SpecialTasks { get; set; }
    }

    public class TaskDto
    {
        public string Name { get; set; }
        public DateTime? PickedUpDate { get; set; }
    }
}