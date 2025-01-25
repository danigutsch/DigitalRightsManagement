using DigitalRightsManagement.Application.Authorization;

namespace DigitalRightsManagement.UnitTests.Common.TestDoubles;

[Authorize]
internal sealed class AuthorizedRequestWithoutRole : TestRequest;
