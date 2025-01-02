using System.Data;
using HimuOJ.Services.Submits.Infrastructure.EntityConfigurations;

namespace HimuOJ.Services.Submits.Infrastructure;

public class SubmitsDbContext : DbContext, IUnitOfWork
{
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<TestPointResult> TestPointResults { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("submits");
        modelBuilder.ApplyConfiguration(new SubmissionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TestPointResultEntityConfiguration());
    }

    private readonly IMediator _mediator;
    private IDbContextTransaction _currentTransaction;

    public SubmitsDbContext(DbContextOptions<SubmitsDbContext> options)
        : base(options)
    {
    }

    public SubmitsDbContext(DbContextOptions<SubmitsDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        System.Diagnostics.Debug.WriteLine("SubmitsDbContext::ctor ->" + base.GetHashCode());
    }

    public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Strong consistency assurance
        await DispatchDomainEventsAsync();
        await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
                             .Entries<Entity>()
                             .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                             .ToList();

        var domainEvents = domainEntities
                           .SelectMany(x => x.Entity.DomainEvents)
                           .ToList();

        domainEntities.ToList()
                      .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("A transaction is already started");
        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (transaction != _currentTransaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}