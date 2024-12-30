using MediatR;

namespace DigitalRightsManagement.Common.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>;
