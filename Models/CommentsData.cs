using DevExpress.Drawing.Internal.Fonts.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static DevExpress.Utils.HashCodeHelper;


namespace LAGem_POPortal.Models
{
    public class CommentsData // 
    {
        public int Id { get; set; }

        public int QualityControlId { get; set; }
        public int QualityControlTypeId { get; set; }
        public int ProductId { get; set; }
        public int PODetailID { get; set; }
        public string QualityControlStatus { get; set; }

        public int CommentaryId { get; set; }
        public int CommentaryTypeId { get; set; }
        public string Comments { get; set; } //Comments/Issues	

        public string SamplesApproval { get; set; } //PP samples Approval
        public string DisneyStatus { get; set; } //Disney Status
        public string ImageApproval { get; set; } //TOP Image Approval

    }
}
