using System.ComponentModel.DataAnnotations;

namespace LADCI.AI.Invoicing.API.Contracts;

public sealed record CreateInvoiceRequest(
    [Required]
    Guid CustomerId,
    [Required]
    DateOnly IssueDate,
    [Required]
    DateOnly DueDate,
    [Required]
    [MinLength(1)]
    IReadOnlyCollection<CreateInvoiceLineItemRequest> LineItems,
    [Range(0.01d, double.MaxValue)]
    decimal TotalAmount,
    [Required]
    [MinLength(1)]
    string Currency);
