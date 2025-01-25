using Ardalis.Result;
using MediatR;

namespace DigitalRightsManagement.Common.Messaging;

#pragma warning disable CA1040
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
#pragma warning restore CA1040
