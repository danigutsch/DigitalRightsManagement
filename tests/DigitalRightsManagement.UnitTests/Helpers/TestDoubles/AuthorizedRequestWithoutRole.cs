using DigitalRightsManagement.Application.Authorization;

namespace DigitalRightsManagement.UnitTests.Helpers.TestDoubles;

[Authorize]
internal sealed record AuthorizedRequestWithoutRole : BaseRequest;
