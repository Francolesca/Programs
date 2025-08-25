using System;
using Core.mediatOR.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Starbucks.Application.Abstractions;
using Starbucks.Domain;
using Starbucks.Persistence;

namespace Starbucks.Application.Coffes.Commands;

public class CoffeDelete
{
    public class Command : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {

            RuleFor(x => x.Id).NotEmpty()
                .WithMessage("You must send the ID of the coffe to be deleted.");
        }
    }
    public class Handler(StarbucksDbContext context) : IRequestHandler<Command, Result>
    {
        private readonly StarbucksDbContext _context = context;
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var coffeExists = await _context.Coffes
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (coffeExists is null)
            {
                return Result.Failure(CoffeErrors.CoffeNoExist);
            }

            _context.Coffes.Remove(coffeExists);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }

}
