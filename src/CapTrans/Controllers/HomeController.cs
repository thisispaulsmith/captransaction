using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CapTrans
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var cmd = new CreatePerson.Command();

            await _mediator.Send(cmd);

            return Ok();
        }
    }
}
