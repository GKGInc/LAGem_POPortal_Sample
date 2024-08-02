using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LAGem_POPortal.Models
{
    public class DepartmentQueueProcessesView
    {
        public int QOID { get; set; }
        public int POID { get; set; }
        public int LOID { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Workcenter { get; set; }
        public string LocationWorkCenter { get; set; }
        public string OperatorId { get; set; }
        public string OperatorName { get; set; }
        public string sono { get; set; }
        public int opno { get; set; }
        public string PartNo { get; set; }
        public string Location { get; set; }
        public string Process { get; set; }
        public string ProcessDescrip { get; set; }
        public int Qty { get; set; }
        public string Reason { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TimeSpent { get; set; }
        public string HoursSpent { get; set; }
        public decimal MinutesSpent { get; set; }
        public string EndTimeUsed { get; set; }
        public string ProcessNotes { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
