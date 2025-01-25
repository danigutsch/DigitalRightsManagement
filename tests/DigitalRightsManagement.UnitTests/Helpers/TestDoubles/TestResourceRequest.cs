using DigitalRightsManagement.Application.Authorization;

namespace DigitalRightsManagement.UnitTests.Helpers.TestDoubles;

[AuthorizeResourceOwner<TestResource>]
#pragma warning disable CS9113
internal sealed record TestResourceRequest(Guid TestResourceId) : BaseRequest;

#pragma warning restore CS9113