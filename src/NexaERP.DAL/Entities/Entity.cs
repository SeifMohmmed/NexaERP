namespace NexaERP.DAL.Entities;

public abstract class Entity
{
    // Creates an entity with the specified ID.
    protected Entity(Guid id)
    {
        Id = id;
    }

    // Required by EF Core.
    protected Entity()
    {
    }

    // Entity identifier.
    public Guid Id { get; init; }
}
