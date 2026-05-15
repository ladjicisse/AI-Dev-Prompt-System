namespace LADCI.AI.Invoicing.API.Contracts;

public sealed record InvoiceResponse(
    Guid InvoiceId,
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    string Status,
    decimal TotalAmount,
    string Currency,
    IReadOnlyCollection<InvoiceLineItemResponse> LineItems);
