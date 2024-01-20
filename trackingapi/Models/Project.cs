using System.ComponentModel.DataAnnotations.Schema;

namespace trackingapi.Models
{
    public class Project
    {
        public int Id { get; set; }
        [ForeignKey("Issue")]
        public virtual int IssueId { get; set; }
        public virtual Issue Issue { get; set; }
    }
}
