using System;
using Core.Mappy.Interfaces;
using Starbucks.Domain;

namespace Starbucks.Application.Coffes.DTOs;

public class CoffeCreateMapping : IMappingProfile
{
    public void Configure(IMapper mapper)
    {
        mapper.CreateMap<CoffeCreateRequest, Coffe>();
    }
}
