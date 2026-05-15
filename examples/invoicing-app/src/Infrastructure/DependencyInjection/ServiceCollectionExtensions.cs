using LADCI.AI.Invoicing.Application.Abstractions;
using LADCI.AI.Invoicing.Infrastructure.Persistence;
using LADCI.AI.Invoicing.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LADCI.AI.Invoicing.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInvoicingInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<InvoicingDbContext>(options =>
            options.UseInMemoryDatabase("InvoicingDb"));

        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        return services;
    }
}
