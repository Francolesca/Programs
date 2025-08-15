using Core.mediatOR.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Coffes.Commands;
using Starbucks.Application.Coffes.DTOs;

namespace Starbucks.Api.Controllers
{
    [Route("api/coffes")]
    [ApiController]
    public class CoffeController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> CreateCoffe(
            CoffeCreateRequest req,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CoffeCreate.Command { CoffeCreateRequest = req },
                cancellationToken
                );
            if (result.IsSucces)
            {
                var coffeId = result.Value;
                return Created($"api/coffes/{coffeId}", coffeId);
            }

            return BadRequest(result.Errors);
        }

    }
}
