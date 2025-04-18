
namespace GymApp.Core.Common;

public interface IEntity<TId>
{
    TId Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? ModifiedAt { get; set; }
    bool IsDeleted { get; set; }
}