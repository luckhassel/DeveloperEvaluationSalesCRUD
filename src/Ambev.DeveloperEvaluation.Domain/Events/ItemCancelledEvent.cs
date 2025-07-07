using System;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid ItemId { get; }
    public DateTime CancelledAt { get; }

    public ItemCancelledEvent(Guid saleId, Guid itemId)
    {
        SaleId = saleId;
        ItemId = itemId;
        CancelledAt = DateTime.UtcNow;
    }
}