using LADCI.AI.Invoicing.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LADCI.AI.Invoicing.Infrastructure.Persistence.Configurations;

internal sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.IssueDate)
            .IsRequired();

        builder.Property(x => x.Id)
            .HasConversion(
                invoiceId => invoiceId.Value,
                value => new Domain.ValueObjects.InvoiceId(value))
            .HasColumnName("InvoiceId")
            .ValueGeneratedNever();

        builder.Property(x => x.CustomerId)
            .HasConversion(
                customerId => customerId.Value,
                value => new Domain.ValueObjects.CustomerId(value))
            .HasColumnName("CustomerId")
            .IsRequired();

        builder.Property(x => x.DueDate)
            .HasConversion(
                dueDate => dueDate.Value,
                value => new Domain.ValueObjects.DueDate(value))
            .HasColumnName("DueDate")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.OwnsOne(x => x.TotalAmount, totalAmountBuilder =>
        {
            totalAmountBuilder.Property(x => x.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            totalAmountBuilder.Property(x => x.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsMany(x => x.LineItems, lineItemBuilder =>
        {
            lineItemBuilder.ToTable("InvoiceLineItems");
            lineItemBuilder.WithOwner().HasForeignKey("InvoiceId");

            lineItemBuilder.Property<int>("Id");
            lineItemBuilder.HasKey("Id");

            lineItemBuilder.Property(x => x.Description)
                .HasMaxLength(256)
                .IsRequired();

            lineItemBuilder.Property(x => x.Quantity)
                .IsRequired();

            lineItemBuilder.OwnsOne(x => x.Subtotal, subtotalBuilder =>
            {
                subtotalBuilder.Property(x => x.Amount)
                    .HasColumnName("SubtotalAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                subtotalBuilder.Property(x => x.Currency)
                    .HasColumnName("SubtotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.Navigation(x => x.TotalAmount).IsRequired();
        builder.Navigation(x => x.LineItems).HasField("_lineItems");
        builder.Navigation(x => x.LineItems).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
