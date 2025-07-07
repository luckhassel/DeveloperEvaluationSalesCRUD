using System;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public DateTime ModifiedAt { get; }

    public SaleModifiedEvent(Guid saleId)
    {
        SaleId = saleId;
        ModifiedAt = DateTime.UtcNow;
    }
}