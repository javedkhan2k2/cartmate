using CartMate.Application.Common.Persistence;
using CartMate.Contracts.Items;
using CartMate.Domain.Lists;
using FluentValidation;
using Mediator;

namespace CartMate.Application.Features.Items;

public sealed record UpdateItemCommand(
    Guid UserId,
    Guid ListId,
    Guid ItemId,
    string Name,
    int Quantity,
    string? Category,
    string? Note,
    uint Version) : ICommand<ItemDto>;

public sealed class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(GroceryItem.MaxNameLength);
        RuleFor(c => c.Quantity).InclusiveBetween(GroceryItem.MinQuantity, GroceryItem.MaxQuantity);
        RuleFor(c => c.Category).MaximumLength(GroceryItem.MaxCategoryLength);
        RuleFor(c => c.Note).MaximumLength(GroceryItem.MaxNoteLength);
    }
}

public sealed class UpdateItemHandler(IAppDbContext db, TimeProvider time) : ICommandHandler<UpdateItemCommand, ItemDto>
{
    public async ValueTask<ItemDto> Handle(UpdateItemCommand command, CancellationToken cancellationToken)
    {
        var list = await db.GroceryLists
            .WithMembersAndItem(command.ItemId)
            .GetOrThrowAsync(command.ListId, cancellationToken);

        var details = new ItemDetails(command.Name, command.Quantity, command.Category, command.Note);
        var item = list.UpdateItem(command.UserId, command.ItemId, details, time.GetUtcNow());

        // Optimistic concurrency: tell EF which version the CLIENT saw. If the row has changed
        // since then, the UPDATE matches 0 rows and EF throws DbUpdateConcurrencyException (→ 409).
        db.Entry(item).Property(i => i.Version).OriginalValue = command.Version;

        await db.SaveChangesAsync(cancellationToken);
        return item.ToDto();
    }
}
