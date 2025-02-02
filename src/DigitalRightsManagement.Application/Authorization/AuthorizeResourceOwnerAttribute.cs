using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Application.Authorization;

public abstract class AuthorizeResourceOwnerAttribute(string resourceIdPropertyPath) : Attribute
{
    public string ResourceIdPropertyPath { get; } = resourceIdPropertyPath;
}


[AttributeUsage(AttributeTargets.Class)]
public sealed class AuthorizeResourceOwnerAttribute<TResource>(string resourceIdPropertyPath) : AuthorizeResourceOwnerAttribute(resourceIdPropertyPath)
    where TResource : IAggregateRoot
{
    public AuthorizeResourceOwnerAttribute() : this($"{typeof(TResource).Name}Id") { }
}
