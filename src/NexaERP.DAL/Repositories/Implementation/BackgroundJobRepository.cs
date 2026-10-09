using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class BackgroundJobRepository(
    ApplicationDbContext context)
    : GenericRepository<BackgroundJob>(context),
        IBackgroundJobRepository;
