using System.ComponentModel.DataAnnotations;

namespace LADCI.AI.Invoicing.API.Contracts;

public sealed record CreateInvoiceLineItemRequest(
    [Required]
    [MinLength(1)]
    string Description,
    [Range(1, int.MaxValue)]
    int Quantity,
    [Range(0.01d, double.MaxValue)]
    decimal Subtotal);
