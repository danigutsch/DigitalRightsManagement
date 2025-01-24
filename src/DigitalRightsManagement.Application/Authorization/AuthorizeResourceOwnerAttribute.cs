using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Application.Authorization;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AuthorizeResourceOwnerAttribute<TResource>(string idPropertyPath) : Attribute
    where TResource : AggregateRoot
{
    public AuthorizeResourceOwnerAttribute() : this($"{typeof(TResource).Name}Id") { }

    public string IdPropertyPath { get; } = idPropertyPath;
}
