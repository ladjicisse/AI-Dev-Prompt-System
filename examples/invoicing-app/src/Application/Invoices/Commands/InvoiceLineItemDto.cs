namespace LADCI.AI.Invoicing.Application.Invoices.Commands;

public sealed record InvoiceLineItemDto(
    string Description,
    int Quantity,
    decimal Subtotal,
    string Currency);
