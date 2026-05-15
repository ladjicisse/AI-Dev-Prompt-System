namespace LADCI.AI.Invoicing.Application.Invoices.Commands;

public sealed record CreateInvoiceDto(
    Guid InvoiceId,
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    IReadOnlyCollection<CreateInvoiceLineItemDto> LineItems,
    decimal TotalAmount,
    string Currency);
