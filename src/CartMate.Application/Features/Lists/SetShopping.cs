using CartMate.Application.Common.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace CartMate.Application.Features.Lists;

public sealed record SetShoppingCommand(Guid UserId, Guid ListId, bool IsShopping) : ICommand;

public sealed class SetShoppingHandler(IAppDbContext db, TimeProvider time) : ICommandHandler<SetShoppingCommand>
{
    public async ValueTask<Unit> Handle(SetShoppingCommand command, CancellationToken cancellationToken)
    {
        var list = await db.GroceryLists
            .Include(l => l.Members)
            .GetOrThrowAsync(command.ListId, cancellationToken);

        if (command.IsShopping)
        {
            list.StartShopping(command.UserId, time.GetUtcNow());
        }
        else
        {
            list.StopShopping(command.UserId);
        }

        await db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
