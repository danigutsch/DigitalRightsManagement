using Ardalis.Result;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Domain.UserAggregate.Events;

namespace DigitalRightsManagement.Domain.UserAggregate;

public sealed class User : AggregateRoot
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public UserRoles Role { get; private set; }

    private User(string username, string email, UserRoles role, Guid? id = null) : base(id ?? Guid.CreateVersion7())
    {
        Username = username.Trim();
        Email = email.Trim();
        Role = role;

        QueueDomainEvent(new UserCreated(Id, username, email, role));
    }

#pragma warning disable CS8618, CS9264
    private User() { } // Do not use
#pragma warning restore CS8618, CS9264

    public static Result<User> Create(string username, string email, UserRoles role, Guid? id = null)
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
            return Errors.User.UnknownRole();
        }

        if (id is not null && id == Guid.Empty)
        {
            return Errors.User.EmptyId();
        }

        var user = new User(username, email, role, id);

        return user;
    }

    public Result ChangeRole(User admin, UserRoles newRole)
    {
        if (!Enum.IsDefined(newRole))
        {
            return Errors.User.UnknownRole();
        }

        if (Role == newRole)
        {
            return Errors.User.AlreadyInRole(Id, newRole);
        }

        if (admin.Role != UserRoles.Admin)
        {
            return Errors.User.UnauthorizedToPromote(admin.Id, Id, newRole);
        }

        Role = newRole;

        QueueDomainEvent(new UserPromoted(admin.Id, Id, Role, newRole));

        return Result.Success();
    }

    public Result ChangeEmail(string newEmail)
    {
        var emailValidation = ValidateEmail(newEmail);
        if (!emailValidation.IsSuccess)
        {
            return emailValidation;
        }

        Email = newEmail.Trim();

        QueueDomainEvent(new EmailUpdated(Id, newEmail));

        return Result.Success();
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

        if (indexOfAt != trimmedEmail.LastIndexOf('@'))
        {
            return Errors.User.InvalidEmail();
        }

        var indexOfDot = trimmedEmail.LastIndexOf('.');
        if (indexOfDot <= 0)
        {
            return Errors.User.InvalidEmail();
        }

        if (indexOfDot <= indexOfAt + 2)
        {
            return Errors.User.InvalidEmail();
        }

        if (indexOfDot == trimmedEmail.Length - 1)
        {
            return Errors.User.InvalidEmail();
        }

        return Result.Success();
    }
}
