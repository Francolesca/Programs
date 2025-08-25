using System;
using Core.Mappy.Interfaces;
using Core.mediatOR.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Coffes.DTOs;
using Starbucks.Domain;
using Starbucks.Domain.Abstractions;
using Starbucks.Persistence;

namespace Starbucks.Application.Coffes.Commands;

public class CoffeCreate
{
    public class Command : IRequest<Result<Guid>>
    {
        public required CoffeCreateRequest CoffeCreateRequest { get; set; }

    }
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.CoffeCreateRequest)
                .SetValidator(new RequestValidator());
        }
    }
    public class RequestValidator : AbstractValidator<CoffeCreateRequest> {
        public RequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The Name is required.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("The Description is required.");
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("The Category is required.");
        }
    }
    public class Handler(
        StarbucksDbContext context,
        IMapper mapper
        )
        : IRequestHandler<Command, Result<Guid>>
    {
        private readonly StarbucksDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Result<Guid>> Handle(
            Command request,
            CancellationToken cancellationToken
            )
        {
            var nameCoffeIsExists = await _context
                .Coffes
                .AnyAsync(
                    x => x.Name
                    == request.CoffeCreateRequest.Name
                    );
            if (nameCoffeIsExists)
            {
                return Result<Guid>.Failure(
                    CoffeErrors.NameDuplicate
                );
            }
            var coffe = _mapper.Map<Domain.Coffe>(request.CoffeCreateRequest);
            _context.Add(coffe);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(coffe.Id);
        }
    }
}
