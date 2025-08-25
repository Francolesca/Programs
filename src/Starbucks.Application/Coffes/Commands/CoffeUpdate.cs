using System;
using Core.Mappy.Interfaces;
using Core.mediatOR.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Application.Coffes.DTOs;
using Starbucks.Domain;
using Starbucks.Persistence;

namespace Starbucks.Application.Coffes.Commands;

public class CoffeUpdate
{
    public class Command : IRequest<Result>
    {
        public required Guid Id { get; set; }
        public required CoffeUpdateRequest CoffeUpdateRequest { get; set; }
    }
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.CoffeUpdateRequest)
                .SetValidator(new RequestValidator());
        }
    }
    public class RequestValidator : AbstractValidator<CoffeUpdateRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The name cannot be empty.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("The description cannot be empty");
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("The category cannot be empty.");
        }
    }
    public class Handler(
        StarbucksDbContext context,
        IMapper mapper
    )
    : IRequestHandler<Command, Result>
    {
        private readonly StarbucksDbContext _context = context;
        public async Task<Result> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var coffe = await _context
                .Coffes
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id
                );
            if (coffe is null)
            {
                return Result.Failure(
                    CoffeErrors.CoffeNoExist
                );
            }

            var coffeName = await _context
                .Coffes
                .AnyAsync(x => x.Name == request.CoffeUpdateRequest.Name);
            if (coffeName)
            {
                return Result.Failure(
                    CoffeErrors.NameDuplicate
                );
            }

            coffe.Name = request.CoffeUpdateRequest.Name;
            coffe.Description = request.CoffeUpdateRequest.Description;
            coffe.CategoryId = request.CoffeUpdateRequest.CategoryId;
            coffe.Price = request.CoffeUpdateRequest.Price;

            _context.Entry(coffe).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
