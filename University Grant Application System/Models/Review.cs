using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class Review
    {
        [Key]
        [Required]
        public int ReviewId { get; set; }

        public int ReviewerId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int FormTableId { get; set; }
        [ForeignKey("FormTableId")]
        public virtual FormTable? FormTable { get; set; }


        public double totalScore { get; set; }

        public bool ReviewDone { get; set; } = false;



    }
}
