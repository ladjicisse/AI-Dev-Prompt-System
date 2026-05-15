namespace LADCI.AI.Invoicing.Application.Invoices.Commands;

public sealed record InvoiceDto(
    Guid InvoiceId,
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    string Status,
    decimal TotalAmount,
    string Currency,
    IReadOnlyCollection<InvoiceLineItemDto> LineItems);
