using LADCI.AI.Invoicing.Application.Abstractions;
using LADCI.AI.Invoicing.Domain.Aggregates;
using LADCI.AI.Invoicing.Domain.ValueObjects;

namespace LADCI.AI.Invoicing.Application.Invoices.Queries;

public sealed class InvoiceQueryService(IInvoiceRepository invoiceRepository) : IInvoiceQueryService
{
    public async Task<IReadOnlyCollection<InvoiceDetailsDto>> GetAllAsync(
        GetInvoicesDto request,
        CancellationToken cancellationToken = default)
    {
        var invoices = await invoiceRepository.GetByCustomerIdAsync(
            new CustomerId(request.CustomerId),
            cancellationToken);

        return invoices
            .Select(Map)
            .ToArray();
    }

    public async Task<InvoiceDetailsDto> GetDetailsAsync(
        GetInvoiceDetailsDto request,
        CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(
            new InvoiceId(request.InvoiceId),
            new CustomerId(request.CustomerId),
            cancellationToken)
            ?? throw new InvalidOperationException("Invoice was not found.");

        return Map(invoice);
    }

    private static InvoiceDetailsDto Map(Invoice invoice)
        => new(
            invoice.Id.Value,
            invoice.CustomerId.Value,
            invoice.IssueDate,
            invoice.DueDate.Value,
            invoice.Status.ToString(),
            invoice.TotalAmount.Amount,
            invoice.TotalAmount.Currency,
            invoice.LineItems
                .Select(item => new InvoiceDetailsLineItemDto(
                    item.Description,
                    item.Quantity,
                    item.Subtotal.Amount,
                    item.Subtotal.Currency))
                .ToArray());
}
