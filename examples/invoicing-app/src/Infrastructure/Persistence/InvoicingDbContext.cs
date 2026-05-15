using LADCI.AI.Invoicing.Domain.Aggregates;
using LADCI.AI.Invoicing.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LADCI.AI.Invoicing.Infrastructure.Persistence;

public sealed class InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
