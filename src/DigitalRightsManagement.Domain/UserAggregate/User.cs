﻿using Ardalis.Result;
using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.UserAggregate;

public sealed class User : AggregateRoot
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public UserRoles Role { get; private set; }

    private User(string username, string email, UserRoles role) : base(Guid.CreateVersion7())
    {
        Username = username.Trim();
        Email = email.Trim();
        Role = role;

        QueueDomainEvent(new UserCreated(Id, username, email, role));
    }

    public static Result<User> Create(string username, string email, UserRoles role)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Errors.User.InvalidUsername();
        }

        var emailValidation = ValidateEmail(email);
        if (!emailValidation.IsSuccess)
        {
            return emailValidation;
        }

        if (!Enum.IsDefined(role))
        {
            return Errors.User.InvalidRole();
        }

        var user = new User(username, email, role);

        return user;
    }

    private static Result ValidateEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.AsSpan().ContainsAny([' ', '\n', '\r', '\t']))
        {
            return Errors.User.InvalidEmail();
        }

        var indexOfAt = trimmedEmail.IndexOf('@', StringComparison.Ordinal);

        if (indexOfAt <= 0)
        {
            return Errors.User.InvalidEmail();
        }

        if (indexOfAt == trimmedEmail.Length - 1)
        {
            return Errors.User.InvalidEmail();
        }

        if (indexOfAt != trimmedEmail.LastIndexOf('@'))
        {
            return Errors.User.InvalidEmail();
        }

        return Result.Success();
    }
}
