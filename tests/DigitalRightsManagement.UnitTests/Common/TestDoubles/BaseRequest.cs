using Ardalis.Result;
using MediatR;

namespace DigitalRightsManagement.UnitTests.Common.TestDoubles;

internal abstract record BaseRequest : IRequest<Result> { }
