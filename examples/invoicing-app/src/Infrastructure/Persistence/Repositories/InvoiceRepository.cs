using LADCI.AI.Invoicing.Application.Abstractions;
using LADCI.AI.Invoicing.Domain.Aggregates;
using LADCI.AI.Invoicing.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LADCI.AI.Invoicing.Infrastructure.Persistence.Repositories;

internal sealed class InvoiceRepository(InvoicingDbContext dbContext) : IInvoiceRepository
{
    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await dbContext.Invoices.AddAsync(invoice, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Invoice?> GetByIdAsync(InvoiceId invoiceId, CancellationToken cancellationToken = default)
        => dbContext.Invoices
            .SingleOrDefaultAsync(x => x.Id == invoiceId, cancellationToken);

    public async Task<IReadOnlyCollection<Invoice>> GetByCustomerIdAsync(
        CustomerId customerId,
        CancellationToken cancellationToken = default)
        => await dbContext.Invoices
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.IssueDate)
            .ToArrayAsync(cancellationToken);

    public Task<Invoice?> GetByIdAsync(
        InvoiceId invoiceId,
        CustomerId customerId,
        CancellationToken cancellationToken = default)
        => dbContext.Invoices
            .SingleOrDefaultAsync(x => x.Id == invoiceId && x.CustomerId == customerId, cancellationToken);

    public async Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        dbContext.Invoices.Update(invoice);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
