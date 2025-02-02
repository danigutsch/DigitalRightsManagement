using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Application.Persistence;

#pragma warning disable S2326
public interface IRepository <T> where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
