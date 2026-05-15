using LADCI.AI.Invoicing.Domain.Entities;
using LADCI.AI.Invoicing.Domain.Events;
using LADCI.AI.Invoicing.Domain.ValueObjects;

namespace LADCI.AI.Invoicing.Domain.Aggregates;

public sealed class Invoice
{
    private readonly List<object> _domainEvents = [];
    private readonly List<LineItem> _lineItems = [];

    private Invoice()
    {
        Id = null!;
        CustomerId = null!;
        DueDate = null!;
        TotalAmount = null!;
    }

    public Invoice(
        InvoiceId invoiceId,
        CustomerId customerId,
        DateOnly issueDate,
        DueDate dueDate,
        IReadOnlyCollection<LineItem> lineItems,
        Money totalAmount)
    {
        Id = invoiceId ?? throw new ArgumentNullException(nameof(invoiceId));
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        DueDate = dueDate ?? throw new ArgumentNullException(nameof(dueDate));
        TotalAmount = totalAmount ?? throw new ArgumentNullException(nameof(totalAmount));

        if (lineItems is null)
        {
            throw new ArgumentNullException(nameof(lineItems));
        }

        if (lineItems.Count == 0)
        {
            throw new InvalidOperationException("An invoice must contain at least one line item.");
        }

        var items = lineItems.ToArray();
        var sum = items.Sum(static item => item.Subtotal.Amount);

        if (sum != totalAmount.Amount)
        {
            throw new InvalidOperationException("The total amount must equal the sum of line items.");
        }

        IssueDate = issueDate;
        _lineItems.AddRange(items);
        Status = InvoiceStatus.Draft;

        AddDomainEvent(new InvoiceCreated(Id));
    }

    public InvoiceId Id { get; private set; }

    public CustomerId CustomerId { get; private set; }

    public DateOnly IssueDate { get; private set; }

    public DueDate DueDate { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public IReadOnlyCollection<LineItem> LineItems => _lineItems.AsReadOnly();

    public Money TotalAmount { get; private set; }

    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    public void MarkAsPaid()
    {
        if (Status == InvoiceStatus.Paid)
        {
            throw new InvalidOperationException("An invoice cannot be marked as paid more than once.");
        }

        Status = InvoiceStatus.Paid;
        AddDomainEvent(new InvoiceMarkedAsPaid(Id));
    }

    private void AddDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
