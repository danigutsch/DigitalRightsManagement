using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application.UserAggregate;

[Authorize]
public sealed record GetCurrentUserInformationQuery : IQuery<UserDto>
{
    internal sealed class GetCurrentUserQueryHandler(ICurrentUserProvider currentUserProvider) : IQueryHandler<GetCurrentUserInformationQuery, UserDto>
    {
        public async Task<Result<UserDto>> Handle(GetCurrentUserInformationQuery request, CancellationToken cancellationToken)
        {
            return await currentUserProvider.Get(cancellationToken)
                .MapAsync(user => new UserDto(
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role,
                    [.. user.Products]));
        }
    }
}
