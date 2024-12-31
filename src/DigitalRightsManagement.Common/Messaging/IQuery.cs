using MediatR;

namespace DigitalRightsManagement.Common.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>;
