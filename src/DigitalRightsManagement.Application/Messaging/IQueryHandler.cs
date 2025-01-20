using Ardalis.Result;
using DigitalRightsManagement.Common.Messaging;
using MediatR;

namespace DigitalRightsManagement.Application.Messaging;

internal interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
