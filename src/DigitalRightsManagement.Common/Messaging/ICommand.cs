using Ardalis.Result;
using MediatR;

namespace DigitalRightsManagement.Common.Messaging;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
