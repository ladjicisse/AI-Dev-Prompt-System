namespace LADCI.AI.Invoicing.Application.Invoices.Commands;

public sealed record CreateInvoiceLineItemDto(
    string Description,
    int Quantity,
    decimal Subtotal);
