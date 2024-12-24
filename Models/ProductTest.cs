using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;


namespace LAGem_POPortal.Models
{
    public class ProductTest
    {
        public int Id { get; set; }

        public int ProductTestId { get; set; }      // ProductTestTask
        public int ProductId { get; set; }          // ProductTestTask

        public int Qty { get; set; }                // ProductTestTask
        public DateTime? RequestedDate { get; set; }// ProductTestTask
        public string? RequestedBy { get; set; }    // ProductTestTask
        public DateTime? ReceivedDate { get; set; } // ProductTestTask
        public DateTime? PassedDate { get; set; }    // ProductTestTask
        public string? PassedBy { get; set; }       // ProductTestTask
        public DateTime? FailedDate { get; set; }   // ProductTestTask
        public string? FailedBy { get; set; }       // ProductTestTask
        public string? Comments { get; set; }       // ProductTestTask
        public string? Attachment { get; set; }     // ProductTestTask

        public string? ProductNo { get; set; }      // Product
        public string? ProductName { get; set; }    // Product
        public string? StyleNo { get; set; }        // Product
        public string? SKU { get; set; }            // Product


        public int? TasksCount { get; set; }
        public bool? isSelected { get; set; }
        public string? TestStatus { get; set; }


        public int PODetailId { get; set; }         // ProductTestTask
        public string? PONumber { get; set; }       // ProductTestTask
        public string? Supplier { get; set; }       // ProductTestTask


        public decimal Amount { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? TestNumber { get; set; }
        public int TestDocId { get; set; }
        public DateTime TestDocCreated { get; set; }
        public string? TestType { get; set; }
        public DateTime Requested2 { get; set; }
        public DateTime QuoteReceived { get; set; }
        public DateTime QuoteApproved { get; set; }
        public decimal SupplierAmount { get; set; }
        public string? TestGroup { get; set; }

        public int POHeaderId { get; set; }
        public int SOHeaderId { get; set; }
        public int VendorId { get; set; }
        public int CustomerId { get; set; }
        public int CustomerName { get; set; }
        public DateTime SODate { get; set; }
        public DateTime PODate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public string? SONumber { get; set; }
    }
}
