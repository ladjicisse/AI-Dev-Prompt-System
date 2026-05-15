namespace LADCI.AI.Invoicing.Application.Invoices.Queries;

public sealed record InvoiceDetailsDto(
    Guid InvoiceId,
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    string Status,
    decimal TotalAmount,
    string Currency,
    IReadOnlyCollection<InvoiceDetailsLineItemDto> LineItems);
