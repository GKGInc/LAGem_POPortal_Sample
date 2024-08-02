using DevExpress.Drawing.Internal.Fonts.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static DevExpress.Utils.HashCodeHelper;


namespace LAGem_POPortal.Models
{
    public class CostData // 
    {
        public int Id { get; set; }

        // Jewelry First Cost	
        // Jewelry Cost Amount
        // Packaging Cost Amount
        // Jewelry + packaging
        // Sell Price
        // Sell Price Amount
        // Metal
        // Duty%	
        // Duty Amount
        // Labor + freight
        // Labor amount
        // Disney Royalty 16 %
        // Total cost landed
        // COG %

        public int SODetailId { get; set; }
        public int SOHeaderId { get; set; }
        public string SONumber { get; set; }
        public string PONumber { get; set; }
        public int POHeaderId { get; set; }
        public int ProductId { get; set; }
        public string ProductNo { get; set; }


        public int QtyOrdered { get; set; }
        public decimal FirstCost { get; set; }
        public decimal JewelryCost { get; set; }
        public decimal PackagingCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Price { get; set; }
        public decimal SellAmount { get; set; }
        public decimal HTSCode { get; set; }
        public decimal DutyPercent { get; set; }
        public decimal DutyAmount { get; set; }
        public decimal LaborFreight { get; set; }
        public decimal LaborAmount { get; set; }
        public decimal DisneyRoyalty { get; set; }
        public decimal TotalCostLanded { get; set; }
        public decimal COGPercent { get; set; }
    }
}
