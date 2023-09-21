using CQRS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Handlers;
public interface IEventSourcingHandler<TType>
{
    Task SaveAsync(AggregateRoot aggregate);
    Task<TType> GetByIdAsync(Guid aggregateId);
    Task RepublishEventsAsync();
}
