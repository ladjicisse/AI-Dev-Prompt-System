namespace LADCI.AI.Invoicing.API.Contracts;

public sealed record InvoiceLineItemResponse(
    string Description,
    int Quantity,
    decimal Subtotal,
    string Currency);
