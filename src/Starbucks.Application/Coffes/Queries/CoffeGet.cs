using System;
using Core.Mappy.Interfaces;
using Core.mediatOR.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Coffes.DTOs;
using Starbucks.Persistence;

namespace Starbucks.Application.Coffes.Queries;

public class CoffeGet
{
    public class Query : IRequest<Result<List<CoffeResponse>>>
    {

    }

    public class Handler(
        StarbucksDbContext context, 
        IMapper mapper,
        IConfiguration configuration
        )
    : IRequestHandler<Query, Result<List<CoffeResponse>>>
    {
        private readonly StarbucksDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;
        public async Task<Result<List<CoffeResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var quantityCoffes = _configuration.GetValue<int>("Coffes:RecordsQuantity");

            var coffes = await _context.Coffes.Take(quantityCoffes).ToListAsync();
            var coffeResponse = _mapper.Map<List<CoffeResponse>>(coffes);
            return Result<List<CoffeResponse>>.Success(coffeResponse);
        }
    }

}
