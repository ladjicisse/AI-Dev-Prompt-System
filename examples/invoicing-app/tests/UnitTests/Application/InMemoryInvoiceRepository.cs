using LADCI.AI.Invoicing.Application.Abstractions;
using LADCI.AI.Invoicing.Domain.Aggregates;
using LADCI.AI.Invoicing.Domain.ValueObjects;

namespace LADCI.AI.Invoicing.UnitTests.Application;

internal sealed class InMemoryInvoiceRepository : IInvoiceRepository
{
    private readonly List<Invoice> _storedInvoices = [];

    public IReadOnlyCollection<Invoice> StoredInvoices => _storedInvoices.AsReadOnly();

    public int UpdateCalls { get; private set; }

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _storedInvoices.Add(invoice);
        return Task.CompletedTask;
    }

    public Task<Invoice?> GetByIdAsync(InvoiceId invoiceId, CancellationToken cancellationToken = default)
        => Task.FromResult(_storedInvoices.SingleOrDefault(x => x.Id == invoiceId));

    public Task<IReadOnlyCollection<Invoice>> GetByCustomerIdAsync(
        CustomerId customerId,
        CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<Invoice>>(
            _storedInvoices.Where(x => x.CustomerId == customerId).ToArray());

    public Task<Invoice?> GetByIdAsync(
        InvoiceId invoiceId,
        CustomerId customerId,
        CancellationToken cancellationToken = default)
        => Task.FromResult(
            _storedInvoices.SingleOrDefault(x => x.Id == invoiceId && x.CustomerId == customerId));

    public Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        UpdateCalls++;
        return Task.CompletedTask;
    }

    public void Seed(params Invoice[] invoices)
    {
        _storedInvoices.AddRange(invoices);
    }
}
