using DotNetCore.CAP;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CapTrans
{
    public class PublishPersonCreatedEventHandler : INotificationHandler<PersonCreatedEvent>
    {
        private readonly ICapPublisher _publisher;

        public PublishPersonCreatedEventHandler(ICapPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(PersonCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Need to add to current transaction
            // This wouldn't work (null reference)
            // _publisher.Transaction.Value.Begin(_context.GetCurrentTransaction(), false);


            if (_publisher.Transaction.Value == null)
            {
                throw new System.Exception("Publisher not enlisted in transaction");
            }
            await _publisher.PublishAsync(nameof(PersonCreatedEvent), new Person() { Name = "Test" });
        }
    }
}
