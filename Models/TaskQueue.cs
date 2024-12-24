using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class TaskQueue
    {
        public int Id { get; set; }

        public int TaskQueueId { get; set; }        // TaskQueue

        public int SoHeaderId { get; set; }         // SOHeader
        public string? SONumber { get; set; }       // SOHeader
        public DateTime SODate { get; set; }        // SOHeader
        public int SOStatusId { get; set; }         // SOHeader
        public int SalesProgramId { get; set; }     // SOHeader

        public int ProductId { get; set; }          // Product
        public string? ProductNo { get; set; }      // Product
        public string? ProductName { get; set; }    // Product
        public string? ProductTypeName { get; set; }// ProductType
        public string? StyleNo { get; set; }        // Product
        public string? SKU { get; set; }            // Product

        public int TaskId { get; set; }             // TaskQueue        
        public string? TaskName { get; set; }       // Tasks
        public string? TaskDescription { get; set; }// TaskQueue
        public bool Required { get; set; }          // TaskQueue
        public int Qty { get; set; }                // TaskQueue
        public string? AssignedTo { get; set; }     // TaskQueue

        public string? TaskType { get; set; }       // Tasks
        public int TaskSequence { get; set; }       // Tasks

        public string? CustomerCode { get; set; }   // BusinessPartner
        public string? CustomerName { get; set; }   // BusinessPartner

        public int TaskStatusId { get; set; }       // TaskQueue
        public string? TaskStatusName { get; set; } // TaskQueue
        public bool TaskCompleted { get; set; }     // TaskQueue
        public string? TaskNote { get; set; }       // TaskQueue
        public int SODetailMaterialId { get; set; } // TaskQueue
        public int SODetailId { get; set; }         // TaskQueue
        public int SOLineNo { get; set; }           // TaskQueue
        public int SOSubLineNo { get; set; }        // TaskQueue
        public int SoSubLineTypeId { get; set; }    // TaskQueue
        public string? SoSubLineType { get; set; }  // TaskQueue
        public int LegacySystemId { get; set; }     // TaskQueue
        public DateTime CreatedOn { get; set; }     // TaskQueue


        public int? TasksCount { get; set; }
        public bool? isSelected { get; set; }

    }
}
