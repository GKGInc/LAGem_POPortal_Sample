using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class ProductTest
    {
        public int Id { get; set; }

        public int ProductTestId { get; set; }      // ProductTestTask
        public int ProductId { get; set; }          // ProductTestTask

        public int Qty { get; set; }                // ProductTestTask
        public DateTime? RequestedDate { get; set; }    // ProductTestTask
        public string? RequestedBy { get; set; }    // ProductTestTask
        public DateTime? ReceivedDate { get; set; }     // ProductTestTask
        public DateTime?PassedDate { get; set; }    // ProductTestTask
        public string? PassedBy { get; set; }       // ProductTestTask
        public DateTime? FailedDate { get; set; }   // ProductTestTask
        public string? FailedBy { get; set; }       // ProductTestTask
        public string? Comments { get; set; }       // ProductTestTask
        public string? Attachment { get; set; }     // ProductTestTask

        public string? ProductNo { get; set; }      // Product
        public string? ProductName { get; set; }    // Product


        public int? TasksCount { get; set; }
        public bool? isSelected { get; set; }
        public string? TestStatus { get; set; }

    }
}
