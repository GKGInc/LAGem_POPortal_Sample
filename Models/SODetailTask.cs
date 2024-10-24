using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class SODetailTask
    {
        public int SODetailTaskId { get; set; }
        public int SODetailMaterialId { get; set; }
        public int SODetailId { get; set; }
        public int SOLineNo { get; set; }
        public int SOSubLineNo { get; set; }
        public int SoSubLineTypeId { get; set; }
        public string? SoSubLineType { get; set; }  
        public int ProductId { get; set; }
        public int MaterialId { get; set; }
        public int Qty { get; set; }

        public int TaskId { get; set; }             //BusinessPartnerTask.TaskId --> Tasks.TaskId
        public string? Task { get; set; }
        public int? TaskStatusId { get; set; }
        public string? TaskNote { get; set; }
        public int? LegacySystemId { get; set; }

        public string? TaskName { get; set; }       //BusinessPartnerTask
        public string? TaskDescription { get; set; }//BusinessPartnerTask
        public string? TaskType { get; set; }       //BusinessPartnerTask
        public int? TaskSequence { get; set; }      //BusinessPartnerTask
        public bool? Required { get; set; }         //BusinessPartnerTask
        public int? TaskQty { get; set; }           //BusinessPartnerTask.Qty
        public string? AssignedTo { get; set; }     //BusinessPartnerTask

    }
}
