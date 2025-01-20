using Ardalis.Result;
using MediatR;

namespace DigitalRightsManagement.Common.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
