using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.UnitTests.Common.TestDoubles;

[Authorize(UserRoles.Manager)]
internal sealed record RequestWithAuthorizeAttribute : BaseRequest;
