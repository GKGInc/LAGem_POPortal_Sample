using Azure;
using DevExpress.DataAccess.DataFederation;
using DevExpress.Map.Native;
using DevExpress.Office;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using static DevExpress.Utils.HashCodeHelper;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace LAGem_POPortal.Models
{
    public class BOMData
    {
        public int Id { get; set; }

        public string PONumber { get; set; }        //PO Number
        public DateTime PODate { get; set; }        //PO Order Date 
        public string SONumber { get; set; }        //SO Number

        public string ProgramName { get; set; }    //Program
        public string Product { get; set; }        //Card to use
        
        public string ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string POSuffix { get; set; }


        public string BoxStyle { get; set; }            //BOX Style + Country of Application
        public string Accessories { get; set; }         //Accessories
        public string TicketEdiStyle { get; set; }      //Ticket / EDI Style# (PID)	
        public string TicketInfo { get; set; }          //Ticket Info(Retail Barcode#, SKU)	
        public string TicketTypeDestination { get; set; }   //Ticket Type + Destination
        public string TicketSource { get; set; }            //Ticket Source
        public string TicketProofApproval { get; set; }     //Ticket proof Approval & Order Date + ETA/Tracking
        public string BoxPONo { get; set; }                 //Box PO# + ETA	
        public string CardsManufacturer { get; set; }       //Cards Manufacturer + PO + ETA

        public int SODetailId { get; set; }        //
        public int OrderQty { get; set; }          //
        public decimal Cost { get; set; }          //
        public decimal Price { get; set; }         //



    }
}
