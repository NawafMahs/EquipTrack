using EquipTrack.Domain.Common;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public virtual ICollection<WorkOrder> AssignedWorkOrders { get; set; } = new List<WorkOrder>();
    public virtual ICollection<WorkOrder> CreatedWorkOrders { get; set; } = new List<WorkOrder>();

    public string FullName => $"{FirstName} {LastName}";
}