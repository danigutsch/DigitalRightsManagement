﻿using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Application.Authorization;

[AttributeUsage(AttributeTargets.Class)]
public abstract class AuthorizeResourceOwnerAttribute(string idPropertyPath) : Attribute
{
    public string IdPropertyPath { get; } = idPropertyPath;
}


[AttributeUsage(AttributeTargets.Class)]
public sealed class AuthorizeResourceOwnerAttribute<TResource>(string idPropertyPath) : AuthorizeResourceOwnerAttribute(idPropertyPath)
    where TResource : AggregateRoot
{
    public AuthorizeResourceOwnerAttribute() : this($"{typeof(TResource).Name}Id") { }
}
