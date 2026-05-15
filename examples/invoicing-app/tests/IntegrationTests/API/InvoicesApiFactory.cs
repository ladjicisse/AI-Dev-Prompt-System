using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LADCI.AI.Invoicing.API.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LADCI.AI.Invoicing.IntegrationTests.API;

public sealed class InvoicesApiFactory : WebApplicationFactory<Program>
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}

internal static class InvoiceApiRequests
{
    public static CreateInvoiceRequest ValidCreateInvoiceRequest(
        Guid? customerId = null,
        decimal totalAmount = 150m,
        string currency = "EUR")
    {
        var firstSubtotal = totalAmount / 3m;

        return new CreateInvoiceRequest(
            customerId ?? Guid.NewGuid(),
            new DateOnly(2026, 4, 28),
            new DateOnly(2026, 5, 28),
            [
                new CreateInvoiceLineItemRequest("Subscription", 1, firstSubtotal),
                new CreateInvoiceLineItemRequest("Support", 1, totalAmount - firstSubtotal)
            ],
            totalAmount,
            currency);
    }

    public static Task<HttpResponseMessage> CreateInvoiceAsync(
        this HttpClient client,
        CreateInvoiceRequest request)
        => client.PostAsJsonAsync("/api/invoices", request, InvoicesApiFactory.JsonOptions);

    public static Task<InvoiceResponse?> ReadInvoiceAsync(this HttpContent content)
        => content.ReadFromJsonAsync<InvoiceResponse>(InvoicesApiFactory.JsonOptions);

    public static Task<IReadOnlyCollection<InvoiceResponse>?> ReadInvoicesAsync(this HttpContent content)
        => content.ReadFromJsonAsync<IReadOnlyCollection<InvoiceResponse>>(InvoicesApiFactory.JsonOptions);
}
