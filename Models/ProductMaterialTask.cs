using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class ProductMaterialTask
    {
        public int Id { get; set; }

        public int ProductTaskId { get; set; }          // ProductTask
        public int ProductId { get; set; }              // ProductTask
        public int MaterialId { get; set; }             // ProductTask
        public int Qty { get; set; }                    // ProductTask
        public int? TaskId { get; set; }                // ProductTask.TaskId --> Tasks.TaskId
        public string? TaskName { get; set; }           // ProductTask.Task
        public string? TaskDescription { get; set; }    // ProductTask.TaskNote
        public int? TaskStatusId { get; set; }          // ProductTask
        public int? LegacySystemId { get; set; }        // ProductTask

        public string? ProductNo { get; set; }          // Product
        public string? ProductName { get; set; }        // Product

        public string? MaterialNo { get; set; }         // Product
        public string? MaterialName { get; set; }       // Product

        public string? OrigTaskName { get; set; }       //Task
        public string? OrigTaskDescription { get; set; }//Task
        public string? OrigTaskType { get; set; }       //Task
        public int? OrigTaskSequence { get; set; }      //Task
        public string? OrigTaskAssignedTo { get; set; } //Task
        public bool? OrigTaskRequired { get; set; }     //Task

        public int ItemId { get; set; }                // ProductId/MaterialId
        public string ItemNo { get; set; }             // ProductNo/MaterialNo
        public string ItemName { get; set; }           // ProductName/MaterialName
        public string LineTypeName { get; set; }       // "Product"/"Material"

        public int? TasksCount { get; set; }
        public bool? isSelected { get; set; }    
    }
}
