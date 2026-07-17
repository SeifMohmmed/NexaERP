namespace NexaERP.DAL.Entities;

public abstract class Entity
{
    protected Entity(Guid id)  // Creates a new entity with a unique identifier.
    {
        Id = id;
    }

    //EF Migration usage
    protected Entity()
    {

    }

    public Guid Id { get; init; }  // Unique identifier of the entity.

    public bool IsDeleted { get; set; }

}
