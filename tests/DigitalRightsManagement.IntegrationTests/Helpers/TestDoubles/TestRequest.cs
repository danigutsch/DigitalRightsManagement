using Ardalis.Result;
using MediatR;

namespace DigitalRightsManagement.IntegrationTests.Helpers.TestDoubles;

internal sealed record TestRequest : IRequest<Result>;
