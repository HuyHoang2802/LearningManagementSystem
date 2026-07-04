using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LMS.Models.Entities
{
    [Table("AuditLog")]
    public partial class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [StringLength(100)]
        public string Action { get; set; } = null!;

        [StringLength(255)]
        public string Path { get; set; } = null!;

        public DateTime Timestamp { get; set; }

        public string? Details { get; set; }
    }
}
