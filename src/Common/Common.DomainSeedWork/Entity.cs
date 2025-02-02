﻿#region

using System.Text.Json.Serialization;
using MediatR;

#endregion

namespace HimuOJ.Common.DomainSeedWork;

/// <summary>
///     The foundation of entity objects in Domain Driven Design
/// </summary>
/// <remarks>
///     reference to https://github.com/dotnet/eShop/blob/main/src/Ordering.Domain/SeedWork/Entity.cs
/// </remarks>
public abstract class Entity
{
    private List<INotification> _domainEvents;
    int? _requestedHashCode;

    public int Id { get; protected set; }

    [JsonIgnore]
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents = _domainEvents ?? new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public bool IsTransient()
    {
        return this.Id == default;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (this.GetType() != obj.GetType())
            return false;

        Entity item = (Entity) obj;

        if (item.IsTransient() || this.IsTransient())
            return false;
        return item.Id == this.Id;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = this.Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (Equals(left, null))
            return (Equals(right, null)) ? true : false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}