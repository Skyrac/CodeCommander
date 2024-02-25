namespace Backend.Shared.Entities;

public class AuditableEntity : BaseEntity
{
    public Guid? CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? LastModifiedById { get; set; }
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
}
