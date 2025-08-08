using Core.Mappy.Interfaces;    
using Core.mediatOR.Contracts;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Categories.DTOs;
using Starbucks.Persistence;

namespace Starbucks.Application.Queries;

public class CategoryListGet
{
    public class Query : IRequest<List<CategoryDTO>>
    {}
    public class Handler(
        StarbucksDbContext context, 
        IMapper mapper
        ) 
        : IRequestHandler<Query, List<CategoryDTO>>
    {
        private readonly StarbucksDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<List<CategoryDTO>> Handle(
            Query request,
            CancellationToken cancellationToken
            )
        {
            var categories = await _context.Categories.ToListAsync();
            return _mapper.Map<List<CategoryDTO>>(categories);
        }
    }
}