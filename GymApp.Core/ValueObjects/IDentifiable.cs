

using GymApp.Core.Interfaces;
using Newtonsoft.Json;

namespace GymApp.ValueObjects;

[JsonConverter(typeof(IDentifiableJsonConverter))]
public class IDentifiable : IIdentifiable, ICloneable
{
    public Guid Id { get; set; }

    public static string New => new IDentifiable();

    public static string Empty => new IDentifiable(Guid.Empty);

    public IDentifiable()
        : this(Guid.NewGuid())
    {
    }

    public IDentifiable(string id)
        : this(CheckNull(id))
    {
    }

    private static Guid CheckNull(string guid)
    {
        if (!Guid.TryParse(guid, out var result))
        {
            return Guid.Empty;
        }

        return result;
    }

    public IDentifiable(Guid id)
    {
        Id = id;
    }

    public bool IsEmpty()
    {
        return Guid.Empty.Equals(Id);
    }

    public static string From(Guid g)
    {
        return new IDentifiable(g);
    }

    public static string From(string s)
    {
        return new IDentifiable(s);
    }

    public static bool IsValid(string _id)
    {
        Guid result;
        return Guid.TryParse(_id + string.Empty, out result);
    }

    public static bool IsValid(IDentifiable _id)
    {
        if ((object)_id != null)
        {
            _ = _id.Id;
            Guid result;
            return Guid.TryParse((_id?.Id).ToString() + string.Empty, out result);
        }

        return false;
    }

    public static IDentifiable Parse(string id)
    {
        return new IDentifiable(id);
    }

    public static IDentifiable Parse(Guid id)
    {
        return new IDentifiable(id);
    }

    public object Clone()
    {
        return new IDentifiable(Id);
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public string ToStringAsNumbersOnly()
    {
        return $"{Id:N}";
    }

    public override bool Equals(object obj)
    {
        if (obj is IDentifiable dentifiable)
        {
            return Id.Equals(dentifiable.Id);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public static implicit operator IDentifiable(Guid id)
    {
        return new IDentifiable(id.ToString());
    }

    public static implicit operator IDentifiable(string id)
    {
        return new IDentifiable(id);
    }

    public static implicit operator Guid(IDentifiable id)
    {
        return id.Id;
    }

    public static implicit operator string(IDentifiable id)
    {
        return id?.Id.ToString();
    }

    public static implicit operator IDentifiableParent(IDentifiable id)
    {
        Guid? guid = id?.Id;
        if (!guid.HasValue)
        {
            return null;
        }

        return guid.GetValueOrDefault();
    }

    public static bool operator ==(IDentifiable a, IDentifiable b)
    {
        if ((object)a == null && (object)b == null)
        {
            return true;
        }

        if ((object)a != null && (object)b == null)
        {
            return false;
        }

        if ((object)b != null && (object)a == null)
        {
            return false;
        }

        return a.Id.Equals(b.Id);
    }

    public static bool operator !=(IDentifiable a, IDentifiable b)
    {
        if ((object)a == null && (object)b == null)
        {
            return true;
        }

        if ((object)a != null && (object)b == null)
        {
            return false;
        }

        if ((object)b != null && (object)a == null)
        {
            return false;
        }

        return !a.Id.Equals(b.Id);
    }
}