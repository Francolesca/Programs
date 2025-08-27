using Core.mediatOR.Contracts;
using Microsoft.AspNetCore.Mvc;
using Starbucks.Application.Flavours.Commands;
using Starbucks.Application.Flavours.DTOs;
using static Starbucks.Application.Flavours.Commands.FlavorImageUpdate;
using static Starbucks.Application.Flavours.Queries.FlavorGet;

namespace Starbucks.Api.Controllers
{
    [Route("api/flavors")]
    [ApiController]
    public class FlavorController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var query = new Query();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpPut("{id}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put(
            [FromForm] FlavorImageUpdateRequest req,
            int id,
            CancellationToken cancellationToken
            )
        {
            await using var stream = req.ImageFile?.OpenReadStream();
            var flavorRequest = new FlavorUpdateRequest
            {
                Name = req.Name!,
                Stream = stream!
            };
            var command = new Command(id, flavorRequest);
            var res = await _mediator.Send(command, cancellationToken);
            return res.IsSucces ? Ok(res) : BadRequest();
        }
    }

}
public class FlavorImageUpdateRequest
{
    public string? Name { get; set; }

    public IFormFile? ImageFile { get; set; }
}