using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class Tasks
    {
        public int TaskId { get; set; }

        public string? TaskName { get; set; }
        public string? TaskDescription { get; set; }
        public string? TaskType { get; set; }

        public int? TaskSequence { get; set; }
        public bool? Required { get; set; }
        public int? Qty { get; set; }
        public string? AssignedTo { get; set; }

        public int? LegacySystemId { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    public class TasksExt : Tasks
    {
        public bool isUsed { get; set; }

        public TasksExt(Tasks t)
        {
            this.TaskId = t.TaskId;
            this.TaskName = t.TaskName;
            this.TaskDescription = t.TaskDescription;
            this.TaskType = t.TaskType;

            this.TaskSequence = t.TaskSequence;
            this.Required = t.Required;
            this.Qty = t.Qty;
            this.AssignedTo = t.AssignedTo;

            this.LegacySystemId = t.LegacySystemId;
            this.CreatedOn = t.CreatedOn;
            this.isUsed = false;
        }
    }
}
