using System.ComponentModel.DataAnnotations;

namespace UAApp.Domain.Common
{
    public class CommonEntity
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string? CreatedByName { get; set; }
        [MaxLength(50)]
        public string? CreatedBy { get; set; }
        [MaxLength(50)]
        public string? CreatedEmail { get; set; }
        [MaxLength(50)]
        public string? UpdatedByName { get; set; }
        [MaxLength(50)]
        public string? UpdatedBy { get; set; }
        [MaxLength(50)]
        public string? UpdatedByEmail { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        [MaxLength(50)]
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
