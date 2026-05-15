namespace LADCI.AI.Invoicing.Application.Invoices.Queries;

public sealed record GetInvoiceDetailsDto(
    Guid CustomerId,
    Guid InvoiceId);
