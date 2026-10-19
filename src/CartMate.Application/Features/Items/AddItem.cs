using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Items;
using CartMate.Domain.Lists;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Items;

public sealed record AddItemCommand(Guid UserId, Guid ListId, string Name, int Quantity, string? Category, string? Note)
    : ICommand<ItemDto>;

public sealed class AddItemValidator : AbstractValidator<AddItemCommand>
{
    public AddItemValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(GroceryItem.MaxNameLength);
        RuleFor(c => c.Quantity).InclusiveBetween(GroceryItem.MinQuantity, GroceryItem.MaxQuantity);
        RuleFor(c => c.Category).MaximumLength(GroceryItem.MaxCategoryLength);
        RuleFor(c => c.Note).MaximumLength(GroceryItem.MaxNoteLength);
    }
}

public sealed class AddItemHandler(IAppDbContext db, TimeProvider time) : ICommandHandler<AddItemCommand, ItemDto>
{
    public async ValueTask<ItemDto> Handle(AddItemCommand command, CancellationToken cancellationToken)
    {
        // Only members are needed to check permission; existing items are not loaded.
        var list = await db.GroceryLists
            .Include(l => l.Members)
            .GetOrThrowAsync(command.ListId, cancellationToken);

        var details = new ItemDetails(command.Name, command.Quantity, command.Category, command.Note);
        var item = list.AddItem(command.UserId, details, time.GetUtcNow());

        await db.SaveChangesAsync(cancellationToken);
        return item.ToDto(); // Version is filled in by EF from PostgreSQL after the insert.
    }
}
