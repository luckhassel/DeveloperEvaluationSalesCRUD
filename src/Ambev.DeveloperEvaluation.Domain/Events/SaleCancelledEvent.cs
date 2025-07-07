using System;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public DateTime CancelledAt { get; }

    public SaleCancelledEvent(Guid saleId)
    {
        SaleId = saleId;
        CancelledAt = DateTime.UtcNow;
    }
}