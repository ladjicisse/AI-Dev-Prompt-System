namespace LADCI.AI.Invoicing.Application.Invoices.Queries;

public interface IInvoiceQueryService
{
    Task<IReadOnlyCollection<InvoiceDetailsDto>> GetAllAsync(
        GetInvoicesDto request,
        CancellationToken cancellationToken = default);

    Task<InvoiceDetailsDto> GetDetailsAsync(
        GetInvoiceDetailsDto request,
        CancellationToken cancellationToken = default);
}
