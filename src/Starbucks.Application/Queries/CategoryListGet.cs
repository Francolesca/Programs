using System;
using Core.mediatOR.Contracts;
using Microsoft.EntityFrameworkCore;
using Starbucks.Domain;
using Starbucks.Persistence;

namespace Starbucks.Application.Queries;

public class CategoryListGet
{
    public class Query : IRequest<List<Category>>
    {

    }
    public class Handler(StarbucksDbContext context) : IRequestHandler<Query, List<Category>>
    {
        private readonly StarbucksDbContext _context = context;
        public async Task<List<Category>> Handle(
            Query request,
            CancellationToken cancellationToken
            )
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
