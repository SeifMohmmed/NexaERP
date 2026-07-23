namespace NexaERP.DAL.Repositories.Abstraction;

public interface ISoftDeletable
{
    // Indicates whether the entity is soft deleted.
    bool IsDeleted { get; set; }
}
