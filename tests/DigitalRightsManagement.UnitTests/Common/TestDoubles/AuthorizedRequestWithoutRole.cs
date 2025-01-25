using DigitalRightsManagement.Application.Authorization;

namespace DigitalRightsManagement.UnitTests.Common.TestDoubles;

[Authorize]
internal sealed record AuthorizedRequestWithoutRole : BaseRequest;
