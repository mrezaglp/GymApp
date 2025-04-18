using System.Reflection;
using GymApp.Core.Interfaces;
using Gym.ValueObjects;
using MediatR;

public abstract class BaseDomainEvent : IDomainEvent, IEvent, INotification, Intent
{
    public DateTime DateOccurred { get; private set; }

    public string IntentId { get; private set; }

    protected BaseDomainEvent()
    {
        IntentId = IDentifiable.New.ToString();
        DateOccurred = DateTimeService.Current.Now;
    }

    protected BaseDomainEvent(string intentId)
    {
        IntentId = intentId ?? IDentifiable.New.ToString();
        DateOccurred = DateTimeService.Current.Now;
    }

    public virtual int GenerateHashCode(params object[] properties)
    {
        HashCode hashCode = default(HashCode);
        if (properties == null || properties.Length == 0)
        {
            properties = GetType()?.GetProperties(BindingFlags.Default)?.Where((PropertyInfo x) => x.Name != "DateOccurred")?.Select((PropertyInfo x) => x.GetValue(x) ?? new object())?.ToArray() ?? Array.Empty<object>();
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

    public virtual string GetKey()
    {
        return IntentId;
    }
}