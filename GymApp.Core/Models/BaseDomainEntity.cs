using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using GymApp.Core.Events;
using GymApp.Core.Interfaces;
using MediatR;


namespace GymApp.Core.Models;

public abstract class BaseDomainEntity : IEntity, IMuteEntity, ICreatedDateTime, ISoftDelete, IVisibility, IModifiedDateTime, IVersioned
{
    private bool hasModifiedOnce;

    [NotMapped]
    protected Dictionary<Type, Intent> _domainMessages = new Dictionary<Type, Intent>();

    [NotMapped]
    public IEnumerable<BaseDomainEvent> DomainEvents
    {
        get
        {
            List<BaseDomainEvent> list = new List<BaseDomainEvent>();
            foreach (BaseDomainEvent item in _domainMessages.Select<KeyValuePair<Type, Intent>, Intent>((KeyValuePair<Type, Intent> x) => x.Value).OfType<BaseDomainEvent>())
            {
                list.Add(item);
            }

            return new _003C_003Ez__ReadOnlyList<BaseDomainEvent>(list);
        }
    }

    [NotMapped]
    public IEnumerable<BaseIntegerationEvent> IntegerationlEvents => _domainMessages.Select<KeyValuePair<Type, Intent>, Intent>((KeyValuePair<Type, Intent> x) => x.Value).OfType<BaseIntegerationEvent>();

    [NotMapped]
    public IEnumerable<ICommand<IEvent>> DomainCommands => _domainMessages.Select<KeyValuePair<Type, Intent>, Intent>((KeyValuePair<Type, Intent> x) => x.Value).OfType<ICommand<IEvent>>();

    public DateTime CreatedAt { get; private set; }

    public bool IsVisibled { get; private set; }

    public DateTime? ModifiedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public int RawVersion { get; protected set; }

    public byte[]? RowVersion { get; protected set; }

    protected BaseDomainEntity()
        : this(DateTimeService.Current.Now)
    {
    }

    protected BaseDomainEntity(DateTime createdAt)
    {
        CreatedAt = createdAt;
        ModifiedAt = null;
        IsVisibled = true;
        IsDeleted = false;
        RawVersion = 0;
        RowVersion = BitConverter.GetBytes(DateTimeService.Current.Now.Ticks);
    }

    public virtual string RowVersionHex()
    {
        if (RowVersion == null)
        {
            return string.Empty;
        }

        return BitConverter.ToString(RowVersion).Replace("-", "");
    }

    public virtual string RowVersionHex(byte[]? anotherVersion)
    {
        if (anotherVersion == null)
        {
            return string.Empty;
        }

        return BitConverter.ToString(anotherVersion).Replace("-", "");
    }

    public void Delete(bool registerEntityDeletedEvent = true)
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            if (registerEntityDeletedEvent)
            {
                RegisterDomainEvent(GetDeletedEvent() ?? EntityDeletedEvent.WithEntity(this));
            }
        }
    }

    public void InVisible(bool registerEntityUpdatedEvent = true)
    {
        if (!IsDeleted && IsVisibled)
        {
            IsVisibled = false;
            Modified(registerEntityUpdatedEvent);
        }
    }

    public void Visible(bool registerEntityUpdatedEvent = true)
    {
        if (!IsDeleted && !IsVisibled)
        {
            IsVisibled = true;
            Modified(registerEntityUpdatedEvent);
        }
    }

    protected void RegisterDomainEvent(BaseDomainEvent domainEvent)
    {
        registerIntents(domainEvent);
    }

    protected void RegisterIntents(Intent messageToNotify)
    {
        registerIntents(messageToNotify);
    }

    private void registerIntents(Intent msg)
    {
        if (_domainMessages.ContainsKey(msg.GetType()))
        {
            _domainMessages.Remove(msg.GetType());
        }

        _domainMessages.Add(msg.GetType(), msg);
    }

    protected virtual BaseDomainEvent GetDeletedEvent()
    {
        return EntityDeletedEvent.WithEntity(this);
    }

    protected virtual BaseDomainEvent GetUpdatedEvent()
    {
        return EntityUpdatedEvent.WithEntity(this);
    }

    public void ClearDomainEvents()
    {
        _domainMessages.Clear();
    }

    public abstract object? GetPrimaryKey();

    public virtual void Modified(bool registerEntityUpdatedEvent = false)
    {
        ModifiedAt = DateTimeService.Current.Now;
        RowVersion = BitConverter.GetBytes(DateTimeService.Current.Now.Ticks);
        if (!hasModifiedOnce)
        {
            RawVersion++;
        }

        hasModifiedOnce = true;
        if (registerEntityUpdatedEvent)
        {
            RegisterDomainEvent(GetUpdatedEvent() ?? EntityUpdatedEvent.WithEntity(this));
        }
    }

    protected virtual int GenerateHashCode(params object[] properties)
    {
        HashCode hashCode = default(HashCode);
        if (properties == null || properties.Length == 0)
        {
            properties = (from x in GetType().GetProperties(BindingFlags.Default)
                          select x.GetValue(x)).ToArray();
        }

        object[] array = properties;
        foreach (object obj in array)
        {
            if (obj != null)
            {
                hashCode.Add(HashCode.Combine(obj.GetHashCode()));
            }
        }

        return hashCode.ToHashCode();
    }
}