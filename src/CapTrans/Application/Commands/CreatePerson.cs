using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CapTrans
{
    public class CreatePerson
    {
        public class Command : IRequest<Response>
        {

        }

        public class Response
        {

        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IMediator _mediator;
            private readonly MyDbContext _context;

            public Handler(MyDbContext context, IMediator mediator)
            {
                _mediator = mediator;
                _context = context;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var person = new Person { Name = "test " };

                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                var @event = new PersonCreatedEvent { Person = person };

                await _mediator.Publish(@event);

                return new Response();
            }
        }
    }
}
