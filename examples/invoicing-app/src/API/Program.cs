using System.Text.Json.Serialization;
using LADCI.AI.Invoicing.Application.Invoices.Commands;
using LADCI.AI.Invoicing.Application.Invoices.Queries;
using LADCI.AI.Invoicing.Infrastructure.DependencyInjection;
using LADCI.AI.Invoicing.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IInvoiceApplicationService, InvoiceApplicationService>();
builder.Services.AddScoped<IInvoiceQueryService, InvoiceQueryService>();
builder.Services.AddInvoicingInfrastructure();

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "LADCI.AI.Invoicing API v1");
    options.RoutePrefix = "swagger";
});

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
