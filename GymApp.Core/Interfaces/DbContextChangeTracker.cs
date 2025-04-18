
using GymApp.Core.Interfaces;

public class DbContextChangeTracker
{
    public enum DbContextEntryState
    {
        Detached,
        Unchanged,
        Deleted,
        Modified,
        Added,
        Bulk
    }

    public class Entry<T> where T : class, IMuteEntity
    {
        public DbContextEntryState State { get; set; }

        public T Entity { get; private set; }

        public Entry(T entery, DbContextEntryState state)
        {
            Entity = entery;
            State = state;
        }
    }

    private List<Entry<IMuteEntity>> _entries = new List<Entry<IMuteEntity>>();

    public IEnumerable<Entry<T>> Entries<T>() where T : class, IMuteEntity
    {
        return from x in _entries
               where x.Entity is T
               select new Entry<T>(x.Entity as T, x.State);
    }

    public IEnumerable<Entry<IMuteEntity>> Entries()
    {
        return _entries.Select((Entry<IMuteEntity> x) => new Entry<IMuteEntity>(x.Entity, x.State));
    }

    public void Add<T>(T entery, DbContextEntryState state) where T : class, IMuteEntity
    {
        _entries.Add(new Entry<IMuteEntity>(entery, state));
    }

    public void Clear()
    {
        _entries?.Clear();
    }
}