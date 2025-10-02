using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace ECommerceBackend.Infrastructure;


/// HDHiep - 09/24/2025
/// <summary>
/// The application's database context, responsible for managing entity sets and persisting changes.
/// </summary>
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Application);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            int result = await base.SaveChangesAsync(cancellationToken);

            //await PublishDomainEventsAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(new Error("ConcurrencyException", "Concurrency exception occurred in ApplicationDbContext", ErrorType.Conflict), ex);
        }

    }

    //private async Task PublishDomainEventsAsync()
    //{
    //    var domainEvents = ChangeTracker
    //        .Entries<Entity>()
    //        .Select(entry => entry.Entity)
    //        .SelectMany(entity =>
    //        {
    //            //var domainEvents = entity.GetDomainEvents();
    //            entity.ClearDomainEvents();
    //            return domainEvents;
    //        })
    //        .ToList();

    //    foreach (var domainEvent in domainEvents)
    //    {
    //        await _publisher.Publish(domainEvent);
    //    }
    //}

}
