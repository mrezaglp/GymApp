
using GymApp.Core.Interfaces;
using GymApp.Core.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public abstract class DbContextAsyncBase<TDbInstance> : ICommandDbContextAsync<TDbInstance>, ICommandDbContext<TDbInstance>, IBaseDbContextServiceCollection<TDbInstance>, IDbContextInfrastructure<IServiceProvider>, IDbContext<TDbInstance>, IDbContext, IDbContextGetter, IDisposable where TDbInstance : BaseDbInstance
{
    public delegate Task<int> CustomCommitOperation(CancellationToken cancellationToken);

    public delegate Task<int> CustomExecuteOperation(CancellationToken cancellationToken);

    public delegate int CustomCommitExceptionEncontered(Exception exp, ref int retryCount, ref bool retry);

    public delegate (BaseDomainEntity[] WithEvents, BaseDomainEntity[] WithCommands) CustomLoadEntitesWithIntents();

    internal class InnerDbContextAsyncBase<TDbInstance> : DbContextAsyncBase<TDbInstance> where TDbInstance : BaseDbInstance
    {
        private readonly IDbContext<TDbInstance> _dbInstance;

        public override IDbContextProvider DbContextProvider { get; protected set; }

        protected override IDbContext<TDbInstance> _dbContextGetter { get; set; }

        protected override int MaxRetryCount { get; set; }

        public InnerDbContextAsyncBase(IDbContext<TDbInstance> dbInstance, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _dbInstance = dbInstance;
        }

        public static DbContextAsyncBase<TDbInstance> GetInstance(IDbContext<TDbInstance> db, IServiceProvider serviceProvider, int maxRetryCount = 3)
        {
            return new DbContextAsyncBase<TDbInstance>.InnerDbContextAsyncBase<TDbInstance>(db, serviceProvider)
            {
                MaxRetryCount = maxRetryCount
            };
        }

        public override T GetContext<T>()
        {
            return _dbInstance as T;
        }

        protected override Task<int> CommitExceptionEncontered(Exception exp, ref int retryCount, ref bool retry)
        {
            retry = false;
            return Task.FromResult(0);
        }

        protected override Task<int> CommitOperation(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }

    private IEnumerable<IEventMapper> _eventMappers;

    protected readonly IDomainEventDispatcher _dispatcher;

    protected readonly List<Func<Task<long>>> _commandsAsync;

    protected abstract IDbContext<TDbInstance> _dbContextGetter { get; set; }

    public DbContextChangeTracker ChangeTracker { get; protected set; }

    public CustomCommitOperation CommitOperationFunc { get; set; }

    public CustomCommitOperation ExecuteOperationFunc { get; set; }

    public CustomCommitExceptionEncontered CommitExceptionEnconteredFunc { get; set; }

    public CustomLoadEntitesWithIntents LoadEntitesWithIntentsFunc { get; set; }

    public IServiceProvider Instance { get; protected set; }

    protected abstract int MaxRetryCount { get; set; }

    public abstract IDbContextProvider DbContextProvider { get; protected set; }

    public string DbProviderName { get; protected set; }

    protected DbContextAsyncBase(IServiceProvider serviceProvider)
    {
        Instance = serviceProvider;
        _commandsAsync = new List<Func<Task<long>>>();
        ChangeTracker = new DbContextChangeTracker();
        _dispatcher = GetService<IDomainEventDispatcher>();
        _eventMappers = GetServices<IEventMapper>() ?? Enumerable.Empty<IEventMapper>();
    }

    public static DbContextAsyncBase<TDbInstance> GetDbContextAsyncBase(IDbContext<TDbInstance> db, IServiceProvider serviceProvider, int maxRetryCount = 3)
    {
        return InnerDbContextAsyncBase<TDbInstance>.GetInstance(db, serviceProvider, maxRetryCount);
    }

    public virtual void CleanEntityState()
    {
        ChangeTracker.Clear();
        _commandsAsync.Clear();
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public abstract T GetContext<T>() where T : class;

    private async Task<int> HandleExceptionAndRetryAsync(Func<Task<int>> operation)
    {
        bool retry = false;
        int retryCount = 0;
        int result;
        do
        {
            try
            {
                result = await operation();
                retry = false;
            }
            catch (Exception exp)
            {
                int num = ((CommitExceptionEnconteredFunc == null) ? (await CommitExceptionEncontered(exp, ref retryCount, ref retry)) : CommitExceptionEnconteredFunc(exp, ref retryCount, ref retry));
                result = num;
                if (retryCount >= MaxRetryCount)
                {
                    throw;
                }
            }
        }
        while (retry);
        return result;
    }

    protected abstract Task<int> CommitOperation(CancellationToken cancellationToken);

    protected abstract Task<int> CommitExceptionEncontered(Exception exp, ref int retryCount, ref bool retry);

    private async Task<int> CommmitChangesAsync(CancellationToken cancellationToken, bool dispatchIntents)
    {
        int result = await HandleExceptionAndRetryAsync(() => (CommitOperationFunc != null) ? CommitOperationFunc(cancellationToken) : CommitOperation(cancellationToken));
        if (_dispatcher == null)
        {
            CleanEntityState();
            return result;
        }

        if (result > 0 && dispatchIntents)
        {
            (BaseDomainEntity[], BaseDomainEntity[]) entities = ((LoadEntitesWithIntentsFunc != null) ? LoadEntitesWithIntentsFunc() : LoadEntitesWithIntents());
            await handleEventsAsync(entities);
        }

        CleanEntityState();
        return result;
    }

    private async Task handleEventsAsync((BaseDomainEntity[] WithEvents, BaseDomainEntity[] WithCommands) entities)
    {
        List<Task> taskToDo = new List<Task>();
        List<IEvent> eventsOfCommandResults = new List<IEvent>();
        if (entities.WithCommands.Any())
        {
            await foreach (IEvent item in _dispatcher.ExecuteCommands(entities.WithCommands))
            {
                if (item != null)
                {
                    eventsOfCommandResults.Add(item);
                }
            }
        }

        if (entities.WithEvents.Any())
        {
            taskToDo.Add(_dispatcher.DispatchAndClearEvents(entities.WithEvents));
        }

        if (entities.WithCommands.Any())
        {
            taskToDo.Add(_dispatcher.DispatchAndClearEvents(eventsOfCommandResults));
        }

        if (taskToDo.Any())
        {
            await Task.WhenAll(taskToDo);
        }
        else
        {
            await Task.CompletedTask;
        }
    }

    protected virtual (BaseDomainEntity[] WithEvents, BaseDomainEntity[] WithCommands) LoadEntitesWithIntents()
    {
        BaseDomainEntity[] item = (from e in ChangeTracker.Entries<BaseDomainEntity>()
                                   select e.Entity into e
                                   where e.DomainEvents.Any() || e.IntegerationlEvents.Any() || e.DomainCommands.Any()
                                   select e).ToArray();
        BaseDomainEntity[] item2 = (from e in ChangeTracker.Entries<BaseDomainEntity>()
                                    select e.Entity into e
                                    where e.DomainCommands.Any()
                                    select e).ToArray();
        return (WithEvents: item, WithCommands: item2);
    }

    public virtual int SaveChanges(CancellationToken cancellationToken = default(CancellationToken), bool dispatchIntents = true)
    {
        return SaveChangesAsync(cancellationToken, dispatchIntents).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken), bool dispatchIntents = true)
    {
        return CommmitChangesAsync(cancellationToken, dispatchIntents);
    }

    public virtual IEnumerable<TService> GetServices<TService>() where TService : class
    {
        return Instance.GetServices<TService>();
    }

    public TService GetService<TService>() where TService : class
    {
        return Instance.GetService<TService>();
    }

    public TService GetService<TService>(Func<TService, bool> predict) where TService : class
    {
        IEnumerable<TService> services = Instance.GetServices<TService>();
        if (services != null && services.Any())
        {
            return services.Where(predict).FirstOrDefault();
        }

        return null;
    }

    public TService GetService<TService>(Func<bool> predict) where TService : class
    {
        Func<bool> predict2 = predict;
        IEnumerable<TService> services = Instance.GetServices<TService>();
        if (services != null && services.Any())
        {
            return services.Where((TService x) => predict2()).FirstOrDefault();
        }

        return null;
    }

    public IEnumerable<object> GetServices(Type tservice)
    {
        return Instance.GetServices(tservice);
    }

    public Task<T> AddCommand<T>(T entery, DbContextChangeTracker.DbContextEntryState state, Func<Task<long>> func) where T : class, IMuteEntity
    {
        ChangeTracker.Add(entery, state);
        _commandsAsync.Add(func);
        return ValueTask.FromResult(entery).AsTask();
    }

    public Task<IEnumerable<T>> AddCommand<T>(IEnumerable<T> enteries, DbContextChangeTracker.DbContextEntryState state, Func<Task<long>> func) where T : class, IMuteEntity
    {
        foreach (T entery in enteries)
        {
            ChangeTracker.Add(entery, state);
        }

        _commandsAsync.Add(func);
        return ValueTask.FromResult(enteries).AsTask();
    }

    public T AddCommand<T>(T entery, DbContextChangeTracker.DbContextEntryState state, Action action) where T : class, IMuteEntity
    {
        Action action2 = action;
        Func<Task<long>> func = delegate
        {
            action2();
            return ValueTask.FromResult(1L).AsTask();
        };
        AddCommand(entery, state, func);
        return entery;
    }

    public IEnumerable<T> AddCommand<T>(IEnumerable<T> enteries, DbContextChangeTracker.DbContextEntryState state, Action action) where T : class, IMuteEntity
    {
        Action action2 = action;
        foreach (T entery in enteries)
        {
            ChangeTracker.Add(entery, state);
        }

        Func<Task<long>> item = delegate
        {
            action2();
            return ValueTask.FromResult(1L).AsTask();
        };
        _commandsAsync.Add(item);
        return enteries;
    }

    public Task BulkAsync<T>(BulkOperation<T> bulkOperation, CancellationToken cancellationToken, bool dispatchIntents) where T : class, IEntity
    {
        return AddCommand((T)null, DbContextChangeTracker.DbContextEntryState.Bulk, (Func<Task<long>>)(() => ValueTask.FromResult(0L).AsTask()));
    }
}