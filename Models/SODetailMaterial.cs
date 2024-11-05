using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class SODetailMaterial
    {
        public int Id { get; set; }

        public int SODetailMaterialId { get; set; } // SODetailTask
        public int SODetailId { get; set; }         // SODetailTask
        public int SOLineNo { get; set; }           // SODetailTask
        public int SOSubLineNo { get; set; }        // SODetailTask
        public int SoSubLineTypeId { get; set; }    // SODetailTask
        public string? SoSubLineType { get; set; }  // SODetailTask
        public int ProductId { get; set; }          // SODetailTask
        public string? ProductNo { get; set; }
        public string? ProductName { get; set; }
        public int MaterialId { get; set; }         // SODetailTask
        public string? MaterialNo { get; set; }
        public string? MaterialName { get; set; }
        public int Qty { get; set; }                // SODetailTask
        public int? MaterialStatusId { get; set; }
        public string? MaterialNote { get; set; }
        public int SOHeaderId { get; set; }
        public string? LineTypeName { get; set; }
        public string? LineTypeGroup { get; set; }
        public int? ForSoDetailId { get; set; }
        public int? SalesProgramId { get; set; }
        public string? ProgramName { get; set; }
        public int? OrderQty { get; set; }
        public int? ReceivedQty { get; set; }
        public int? UOMId { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string? SONumber { get; set; }
        public int? BusinessPartnerId_Customer { get; set; }
        public string? CustomerName { get; set; }

        public int? SODetailTaskId { get; set; }    // SODetailTask
        public int? TaskId { get; set; }            // SODetailTask --> Tasks.TaskId
        public string? Task { get; set; }           // SODetailTask
        public int? TaskStatusId { get; set; }      // SODetailTask
        public string? TaskNote { get; set; }       // SODetailTask
        public int? LegacySystemId { get; set; }    // SODetailTask

        public string? TaskName { get; set; }       //BusinessPartnerTask
        public string? TaskDescription { get; set; }//BusinessPartnerTask
        public string? TaskType { get; set; }       //BusinessPartnerTask
        public int? TaskSequence { get; set; }      //BusinessPartnerTask
        public bool? Required { get; set; }         //BusinessPartnerTask
        public int? TaskQty { get; set; }           //BusinessPartnerTask.Qty
        public string? AssignedTo { get; set; }     //BusinessPartnerTask


        public DateTime? Requested { get; set; }    // ProductTestTask
        public string? RequestedBy { get; set; }    // ProductTestTask
        public DateTime? Received { get; set; }     // ProductTestTask
        public DateTime? ProcessedDate { get; set; }// ProductTestTask  CASE WHEN...
        public string? ProcessedBy { get; set; }    // ProductTestTask  CASE WHEN...
        public string? TestResult { get; set; }     // ProductTestTask  CASE WHEN...

        public int? TasksCount { get; set; }
        public bool? isSelected { get; set; }
    }
}
