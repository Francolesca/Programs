using Core.mediatOR.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starbucks.Application.Coffes.Commands;
using Starbucks.Application.Coffes.DTOs;

namespace Starbucks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<Guid> CreateCoffe(
            CoffeCreateRequest req,
            CancellationToken cancellationToken)
        {
            var res = await _mediator.Send(
                new CoffeCreate.Command { CoffeCreateRequest = req },
                cancellationToken
                );

            return res;
        }

    }
}
