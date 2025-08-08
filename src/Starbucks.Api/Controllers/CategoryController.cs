using System;
using Core.mediatOR.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Categories.DTOs;
using Starbucks.Domain;
using Starbucks.Persistence;
using static Starbucks.Application.Queries.CategoryListGet;

namespace Starbucks.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<List<CategoryDTO>> Get(CancellationToken cancellationToken)
    {
        var query = new Query();
        var res = await _mediator.Send(query, cancellationToken);
        return res;
    }
    
}   
