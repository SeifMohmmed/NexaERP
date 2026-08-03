using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Invoice;
using NexaERP.BLL.DTOs.InvoiceLine;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[Authorize]
[Route("invoices")]
[ApiController]
public class InvoicesController(
    IInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork,
    LinkService linkService,
    InvoicePdfService invoicePdfService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResult<InvoiceDto>>> GetInvoices(
    [FromQuery] InvoiceQueryParameters query)
    {
        var invoices = invoiceRepository
            .Filter(
                query.Status,
                query.CustomerId,
                query.From,
                query.To)
            .Select(InvoiceMapping.ProjectToDto());

        var result = await PaginationResult<InvoiceDto>.CreateAsync(
            invoices,
            query.Page,
            query.PageSize);

        if (query.IncludeLinks)
        {
            foreach (var invoice in result.Items)
            {
                invoice.Links =
                    CreateLinksForInvoice(
                        invoice.Id,
                        invoice.Status);
            }

            result.Links =
                CreateLinksForInvoices(
                    query,
                    result.HasNextPage,
                    result.HasPreviousPage);
        }

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> GetById(
    Guid id,
    [FromQuery] InvoiceQueryParameters query)
    {
        var invoice =
            await invoiceRepository.GetWithLinesAsync(id);

        if (invoice is null)
        {
            return NotFound();
        }

        var dto = invoice.ToDto();

        if (query.IncludeLinks)
        {
            dto.Links =
                CreateLinksForInvoice(
                    dto.Id,
                    dto.Status);
        }

        return Ok(dto);
    }


    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        var invoice = await invoiceRepository.GetWithLinesAsync(id);

        if (invoice is null)
        {
            return NotFound();
        }

        var invoiceDto = invoice.ToDto();

        var pdfStream = invoicePdfService.Generate(invoiceDto);

        return File(
            pdfStream,
            "application/pdf",
            $"Invoice-{invoiceDto.Id}.pdf");
    }


    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create(
    [FromBody] CreateInvoiceDto dto,
    [FromServices] IValidator<CreateInvoiceDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var invoice = dto.ToEntity();

        await invoiceRepository.AddAsync(invoice);

        await unitOfWork.SaveChangesAsync();

        invoice = await invoiceRepository.GetWithLinesAsync(invoice.Id);

        if (invoice is null)
        {
            return NotFound();
        }

        var invoiceDto = invoice.ToDto();

        invoiceDto.Links =
            CreateLinksForInvoice(
                invoiceDto.Id,
                invoiceDto.Status);

        return CreatedAtAction(
            nameof(GetById),
            new { id = invoiceDto.Id },
            invoiceDto);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateInvoiceDto dto,
    [FromServices] IValidator<UpdateInvoiceDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var invoice =
            await invoiceRepository.GetByIdAsync(id);

        if (invoice is null)
        {
            return NotFound();
        }

        invoice.UpdateEntity(dto);

        invoiceRepository.Update(invoice);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    [HttpPatch("{id:guid}/pay")]
    public async Task<IActionResult> Pay(
    Guid id,
    [FromBody] PayInvoiceDto dto,
    [FromServices] IValidator<PayInvoiceDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var invoice =
            await invoiceRepository.GetByIdAsync(id);

        if (invoice is null)
        {
            return NotFound();
        }

        if (invoice.Status == InvoiceStatus.Paid)
        {
            return BadRequest("Invoice is already paid.");
        }

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = dto.PaidAt;
        invoice.PaymentMethod = dto.PaymentMethod;

        invoiceRepository.Update(invoice);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var invoice =
            await invoiceRepository.GetByIdAsync(id);

        if (invoice is null)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        if (invoice.Status != InvoiceStatus.Draft)
        {
            return BadRequest(
                "Only draft invoices can be deleted.");
        }

        invoiceRepository.Delete(invoice);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    // Creates HATEOAS links for a single invoice.
    private List<LinkDto> CreateLinksForInvoice(
    Guid id,
    InvoiceStatus status)
    {
        List<LinkDto> links =
        [
            linkService.Create(
            nameof(GetById),
            "self",
            HttpMethods.Get,
            new { id }),

        linkService.Create(
            nameof(Update),
            "update",
            HttpMethods.Put,
            new { id }),

        linkService.Create(
            nameof(DownloadPdf),
            "download-pdf",
            HttpMethods.Get,
            new { id })
        ];

        if (status != InvoiceStatus.Paid)
        {
            links.Add(
                linkService.Create(
                    nameof(Pay),
                    "pay",
                    HttpMethods.Patch,
                    new { id }));
        }

        if (status == InvoiceStatus.Draft)
        {
            links.Add(
                linkService.Create(
                    nameof(Delete),
                    "delete",
                    HttpMethods.Delete,
                    new { id }));
        }

        return links;
    }

    // Creates HATEOAS links for the invoice collection.
    private List<LinkDto> CreateLinksForInvoices(
    InvoiceQueryParameters parameters,
    bool hasNextPage,
    bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(
            nameof(GetInvoices),
            "self",
            HttpMethods.Get,
            new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                status = parameters.Status,
                customerId = parameters.CustomerId,
                from = parameters.From,
                to = parameters.To
            }),

        linkService.Create(
            nameof(Create),
            "create-invoice",
            HttpMethods.Post)
        ];

        if (hasNextPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetInvoices),
                    "next-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page + 1,
                        pageSize = parameters.PageSize,
                        status = parameters.Status,
                        customerId = parameters.CustomerId,
                        from = parameters.From,
                        to = parameters.To
                    }));
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetInvoices),
                    "previous-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page - 1,
                        pageSize = parameters.PageSize,
                        status = parameters.Status,
                        customerId = parameters.CustomerId,
                        from = parameters.From,
                        to = parameters.To
                    }));
        }

        return links;
    }
}
