namespace LADCI.AI.Invoicing.Application.Invoices.Commands;

public interface IInvoiceApplicationService
{
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto request, CancellationToken cancellationToken = default);

    Task<InvoiceDto> MarkAsPaidAsync(MarkInvoiceAsPaidDto request, CancellationToken cancellationToken = default);
}
