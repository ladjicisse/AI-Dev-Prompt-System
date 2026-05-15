using LADCI.AI.Invoicing.Application.Invoices.Commands;
using LADCI.AI.Invoicing.Domain.Aggregates;
using LADCI.AI.Invoicing.Domain.ValueObjects;

namespace LADCI.AI.Invoicing.UnitTests.Application;

internal static class TestData
{
    private static readonly DateOnly DefaultIssueDate = new(2026, 4, 28);
    private static readonly DateOnly DefaultDueDate = new(2026, 5, 28);
    private const string DefaultCurrency = "EUR";

    public static CreateInvoiceDto CreateInvoiceRequest(
        Guid? invoiceId = null,
        Guid? customerId = null,
        decimal totalAmount = 150m)
        => new(
            invoiceId ?? Guid.NewGuid(),
            customerId ?? Guid.NewGuid(),
            DefaultIssueDate,
            DefaultDueDate,
            CreateLineItems(totalAmount),
            totalAmount,
            DefaultCurrency);

    public static Invoice CreateInvoiceAggregate(
        Guid? invoiceId = null,
        Guid? customerId = null,
        decimal totalAmount = 150m)
    {
        var request = CreateInvoiceRequest(invoiceId, customerId, totalAmount);

        return new Invoice(
            new InvoiceId(request.InvoiceId),
            new CustomerId(request.CustomerId),
            request.IssueDate,
            new DueDate(request.DueDate),
            request.LineItems
                .Select(x => new LineItem(
                    x.Description,
                    x.Quantity,
                    new Money(x.Subtotal, request.Currency)))
                .ToArray(),
            new Money(request.TotalAmount, request.Currency));
    }

    private static IReadOnlyCollection<CreateInvoiceLineItemDto> CreateLineItems(decimal totalAmount)
    {
        var firstSubtotal = totalAmount / 3m;

        return
        [
            new CreateInvoiceLineItemDto("Subscription", 1, firstSubtotal),
            new CreateInvoiceLineItemDto("Support", 1, totalAmount - firstSubtotal)
        ];
    }
}
