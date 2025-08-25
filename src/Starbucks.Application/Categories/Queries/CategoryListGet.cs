using Core.Mappy.Interfaces;    
using Core.mediatOR.Contracts;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Categories.DTOs;
using Starbucks.Persistence;

namespace Starbucks.Application.Queries;

public class CategoryListGet
{
    public class Query : IRequest<Result<List<CategoryDTO>>>
    {}
    public class Handler(
        StarbucksDbContext context, 
        IMapper mapper
        ) 
        : IRequestHandler<Query, Result<List<CategoryDTO>>>
    {
        private readonly StarbucksDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Result<List<CategoryDTO>>> Handle(
            Query request,
            CancellationToken cancellationToken
            )
        {
            var categories = await _context.Categories.ToListAsync();
            var res = _mapper.Map<List<CategoryDTO>>(categories);
            return Result<List<CategoryDTO>>.Success(res);
        }
    }
}