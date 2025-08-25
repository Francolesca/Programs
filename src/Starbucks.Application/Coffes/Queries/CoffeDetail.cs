using System;
using Core.Mappy.Interfaces;
using Core.mediatOR.Contracts;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Coffes.DTOs;
using Starbucks.Domain;
using Starbucks.Persistence;

namespace Starbucks.Application.Coffes.Queries;

public class CoffeDetail
{
    public class Query : IRequest<Result<CoffeResponse>>
    {
        public Guid Id { get; set; }
    }
    public class Handler(
        StarbucksDbContext context,
        IMapper mapper
    ) : IRequestHandler<Query, Result<CoffeResponse>>
    {
        private readonly StarbucksDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Result<CoffeResponse>> Handle(Query req, CancellationToken cancellationToken)
        {
            var coffe = await _context.Coffes.FirstOrDefaultAsync(
                x => x.Id == req.Id
            );
            if (coffe is null)
            {
                return Result<CoffeResponse>.Failure(CoffeErrors.CoffeNoExist);
            }
            var coffeResponse = _mapper.Map<CoffeResponse>(coffe);
            return Result<CoffeResponse>.Success(coffeResponse);

        }

    }


}
