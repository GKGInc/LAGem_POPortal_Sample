using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class SalesProgramTask
    {
        public int SalesProgramTaskId { get; set; }
        public int SalesProgramId { get; set; }
        public string? ProductProgramBOMId { get; set; }
        public int TaskId { get; set; }
        public bool RequiredTask { get; set; }
        public string? Note { get; set; }
    }
}
