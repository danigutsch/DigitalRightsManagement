using DigitalRightsManagement.Common.Messaging;
using MediatR;

namespace DigitalRightsManagement.Application.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
