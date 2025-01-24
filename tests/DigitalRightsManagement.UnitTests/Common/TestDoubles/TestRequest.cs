using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Domain.UserAggregate;
using MediatR;

namespace DigitalRightsManagement.UnitTests.Common.TestDoubles;

internal class TestRequest : IRequest<Result> { }

[Authorize]
internal sealed class AuthorizedRequestWithoutRole : TestRequest;

[Authorize(UserRoles.Manager)]
internal sealed class AuthorizedRequest : TestRequest;