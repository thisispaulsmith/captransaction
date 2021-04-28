using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CapTrans.Domain.EventHandlers
{
    public class AnotherPersonCreatedEventHandler : INotificationHandler<PersonCreatedEvent>
    {
        public AnotherPersonCreatedEventHandler(MyDbContext context)
        {
        }

        public Task Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Might need to use DB context as part of ambient transaction.
            return Task.CompletedTask;
        }
    }
}
