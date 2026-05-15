using System.Text.Json.Serialization;
using LADCI.AI.Invoicing.API.Contracts;
using LADCI.AI.Invoicing.Application.Invoices.Commands;
using LADCI.AI.Invoicing.Application.Invoices.Queries;
using Microsoft.AspNetCore.Mvc;

namespace LADCI.AI.Invoicing.API.Controllers;

[ApiController]
[Route("api")]
public sealed class InvoicesController(
    IInvoiceApplicationService invoiceApplicationService,
    IInvoiceQueryService invoiceQueryService) : ControllerBase
{
    [HttpPost("invoices")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<InvoiceResponse>> CreateInvoice(
        [FromBody] CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await invoiceApplicationService.CreateAsync(
            new CreateInvoiceDto(
                Guid.NewGuid(),
                request.CustomerId,
                request.IssueDate,
                request.DueDate,
                request.LineItems
                    .Select(item => new CreateInvoiceLineItemDto(
                        item.Description,
                        item.Quantity,
                        item.Subtotal))
                    .ToArray(),
                request.TotalAmount,
                request.Currency),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetInvoiceDetails),
            new { customerId = result.CustomerId, invoiceId = result.InvoiceId },
            Map(result));
    }

    [HttpPost("invoices/{invoiceId:guid}/payments")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<InvoiceResponse>> MarkInvoiceAsPaid(
        Guid invoiceId,
        CancellationToken cancellationToken)
    {
        var result = await invoiceApplicationService.MarkAsPaidAsync(
            new MarkInvoiceAsPaidDto(invoiceId),
            cancellationToken);

        return Ok(Map(result));
    }

    [HttpGet("customers/{customerId:guid}/invoices")]
    [ProducesResponseType(typeof(IReadOnlyCollection<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceResponse>>> GetInvoices(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await invoiceQueryService.GetAllAsync(
            new GetInvoicesDto(customerId),
            cancellationToken);

        return Ok(result.Select(Map).ToArray());
    }

    [HttpGet("customers/{customerId:guid}/invoices/{invoiceId:guid}")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<InvoiceResponse>> GetInvoiceDetails(
        Guid customerId,
        Guid invoiceId,
        CancellationToken cancellationToken)
    {
        var result = await invoiceQueryService.GetDetailsAsync(
            new GetInvoiceDetailsDto(customerId, invoiceId),
            cancellationToken);

        return Ok(Map(result));
    }

    private static InvoiceResponse Map(InvoiceDto invoice)
        => new(
            invoice.InvoiceId,
            invoice.CustomerId,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.Status,
            invoice.TotalAmount,
            invoice.Currency,
            invoice.LineItems
                .Select(item => new InvoiceLineItemResponse(
                    item.Description,
                    item.Quantity,
                    item.Subtotal,
                    item.Currency))
                .ToArray());

    private static InvoiceResponse Map(InvoiceDetailsDto invoice)
        => new(
            invoice.InvoiceId,
            invoice.CustomerId,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.Status,
            invoice.TotalAmount,
            invoice.Currency,
            invoice.LineItems
                .Select(item => new InvoiceLineItemResponse(
                    item.Description,
                    item.Quantity,
                    item.Subtotal,
                    item.Currency))
                .ToArray());
}
