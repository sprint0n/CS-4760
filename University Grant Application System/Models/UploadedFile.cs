using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;
using Microsoft.Extensions.Primitives;

namespace University_Grant_Application_System.Models
{

    public enum AttachmentType
    {
        IRB,
        SupportingDoc,
        OptionalDoc1,
        OptionalDoc2,
        Budget,
    }
    public class UploadedFile
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public AttachmentType Category { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int FormTableId { get; set; }
        [ForeignKey("FormTableId")]
        public FormTable? FormTable { get; set; }

        public int? ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report? Report { get; set; }

        [Required]
        public string StoredFileName { get; set; } = string.Empty;
    }
}
