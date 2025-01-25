using Ardalis.Result;
using MediatR;

namespace DigitalRightsManagement.UnitTests.Helpers.TestDoubles;

internal abstract record BaseRequest : IRequest<Result> { }
