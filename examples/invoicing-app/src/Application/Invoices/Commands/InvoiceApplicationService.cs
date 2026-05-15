using LADCI.AI.Invoicing.Application.Abstractions;
using LADCI.AI.Invoicing.Domain.Aggregates;
using LADCI.AI.Invoicing.Domain.ValueObjects;

namespace LADCI.AI.Invoicing.Application.Invoices.Commands;

public sealed class InvoiceApplicationService(IInvoiceRepository invoiceRepository) : IInvoiceApplicationService
{
    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto request, CancellationToken cancellationToken = default)
    {
        var invoice = new Invoice(
            new InvoiceId(request.InvoiceId),
            new CustomerId(request.CustomerId),
            request.IssueDate,
            new DueDate(request.DueDate),
            request.LineItems
                .Select(item => new LineItem(
                    item.Description,
                    item.Quantity,
                    new Money(item.Subtotal, request.Currency)))
                .ToArray(),
            new Money(request.TotalAmount, request.Currency));

        await invoiceRepository.AddAsync(invoice, cancellationToken);

        return Map(invoice);
    }

    public async Task<InvoiceDto> MarkAsPaidAsync(MarkInvoiceAsPaidDto request, CancellationToken cancellationToken = default)
    {
        var invoiceId = new InvoiceId(request.InvoiceId);
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
            ?? throw new InvalidOperationException("Invoice was not found.");

        invoice.MarkAsPaid();

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);

        return Map(invoice);
    }

    private static InvoiceDto Map(Invoice invoice)
        => new(
            invoice.Id.Value,
            invoice.CustomerId.Value,
            invoice.IssueDate,
            invoice.DueDate.Value,
            invoice.Status.ToString(),
            invoice.TotalAmount.Amount,
            invoice.TotalAmount.Currency,
            invoice.LineItems
                .Select(item => new InvoiceLineItemDto(
                    item.Description,
                    item.Quantity,
                    item.Subtotal.Amount,
                    item.Subtotal.Currency))
                .ToArray());
}
