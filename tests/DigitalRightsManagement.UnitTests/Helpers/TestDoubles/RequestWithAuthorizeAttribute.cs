using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.UnitTests.Helpers.TestDoubles;

[Authorize(AgentRoles.Manager)]
internal sealed record RequestWithAuthorizeAttribute : BaseRequest;
