using DigitalRightsManagement.Application.Authorization;

namespace DigitalRightsManagement.UnitTests.Common.TestDoubles;

[AuthorizeResourceOwner<TestResource>]
#pragma warning disable CS9113
internal sealed record TestResourceRequest(Guid TestResourceId) : BaseRequest;

#pragma warning restore CS9113