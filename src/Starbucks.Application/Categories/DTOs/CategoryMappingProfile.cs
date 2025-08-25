using System;
using Core.Mappy.Interfaces;
using Starbucks.Domain;

namespace Starbucks.Application.Categories.DTOs;

public class CategoryMappingProfile : IMappingProfile
{
    public void Configure(IMapper mapper)
    {
        mapper.CreateMap<Category, CategoryDTO>(
            cfg => {
                cfg.Map(dest => dest.CategoryId, src => src.Id);
                cfg.Map(dest => dest.Name, src => src.Name);
            }
        );
    }
}
