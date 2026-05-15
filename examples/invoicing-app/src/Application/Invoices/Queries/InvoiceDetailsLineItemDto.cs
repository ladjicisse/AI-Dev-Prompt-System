namespace LADCI.AI.Invoicing.Application.Invoices.Queries;

public sealed record InvoiceDetailsLineItemDto(
    string Description,
    int Quantity,
    decimal Subtotal,
    string Currency);
