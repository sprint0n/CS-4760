using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class Report
    {
        [Key]
        [Required]
        public int ReportID { get; set; }
        [Required]
        public int FormTableId { get; set; }
        [ForeignKey("FormTableId")]
        public virtual FormTable? FormTable { get; set; }


        public string newDescription { get; set; } = string.Empty;

        public string newFundingUse { get; set; } = string.Empty;

        public virtual ICollection<UploadedFile> ReportFiles { get; set; } = new List<UploadedFile>();




    }
}
