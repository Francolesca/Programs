using Core.mediatOR.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Coffes.Commands;
using Starbucks.Application.Coffes.DTOs;
using Starbucks.Application.Coffes.Queries;

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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoffe(
            Guid id,
            CoffeUpdateRequest req,
            CancellationToken cancellationToken)
        {
            var command = new CoffeUpdate.Command
            {
                Id = id,
                CoffeUpdateRequest = req
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(Guid id, CancellationToken cancellationToken)
        {
            var query = new CoffeDetail.Query { Id = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoffe(Guid id, CancellationToken cancellationToken)
        {
            var command = new CoffeDelete.Command { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Errors);
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var query = new CoffeGet.Query();
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result.Value);
        }


    }
}
