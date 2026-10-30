using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceProAPI.Data;
using InvoiceProAPI.DTOs;
using InvoiceProAPI.Models;
using InvoiceProAPI.Services;

namespace InvoiceProAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly FbrService   _fbr;

    public InvoicesController(AppDbContext db, FbrService fbr)
    {
        _db  = db;
        _fbr = fbr;
    }

    // GET api/invoices
    [HttpGet]
    public async Task<ActionResult<List<InvoiceResponseDto>>> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] int? customerId = null)
    {
        var query = _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(i => i.Status == status);

        if (customerId.HasValue)
            query = query.Where(i => i.CustomerId == customerId.Value);

        var invoices = await query
            .OrderByDescending(i => i.InvoiceDate)
            .Select(i => MapToDto(i))
            .ToListAsync();

        return Ok(invoices);
    }

    // GET api/invoices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound();
        return Ok(MapToDto(invoice));
    }

    // POST api/invoices
    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> Create(CreateInvoiceDto dto)
    {
        if (!dto.Items.Any())
            return BadRequest("Invoice must have at least one item.");

        var customer = await _db.Customers.FindAsync(dto.CustomerId);
        if (customer == null) return BadRequest("Customer not found.");

        // Generate invoice number
        var lastNum = await _db.Invoices
            .OrderByDescending(i => i.Id)
            .Select(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();
        int nextNum = 101;
        if (lastNum != null && lastNum.StartsWith("INV-"))
        {
            if (int.TryParse(lastNum[4..], out int n)) nextNum = n + 1;
        }

        var invoice = new Invoice
        {
            InvoiceNumber   = $"INV-{nextNum:D4}",
            InvoiceDate     = dto.InvoiceDate.Date,
            DueDate         = dto.DueDate?.Date,
            CustomerId      = dto.CustomerId,
            Status          = dto.Status,
            Notes           = dto.Notes?.Trim(),
            DiscountPercent = dto.DiscountPercent
        };

        // Build line items + compute totals
        decimal subtotal = 0, gstTotal = 0;
        foreach (var itemDto in dto.Items)
        {
            var lineExcl   = itemDto.Quantity * itemDto.UnitPrice;
            var lineTax    = lineExcl * (itemDto.GstPercent / 100m);
            var lineTotal  = lineExcl + lineTax;

            subtotal += lineExcl;
            gstTotal += lineTax;

            invoice.Items.Add(new InvoiceItem
            {
                ProductId                   = itemDto.ProductId,
                ProductName                 = itemDto.ProductName.Trim(),
                HsCode                      = itemDto.HsCode?.Trim(),
                UoM                         = itemDto.UoM,
                Quantity                    = itemDto.Quantity,
                UnitPrice                   = itemDto.UnitPrice,
                GstPercent                  = itemDto.GstPercent,
                TotalValue                  = Math.Round(lineTotal, 2),
                ValueSalesExcludingST       = Math.Round(lineExcl, 2),
                SalesTaxApplicable          = Math.Round(lineTax, 2),
                SaleType                    = itemDto.SaleType
            });
        }

        invoice.Subtotal       = Math.Round(subtotal, 2);
        invoice.GstTotal       = Math.Round(gstTotal, 2);
        var beforeDiscount     = subtotal + gstTotal;
        invoice.DiscountAmount = Math.Round(beforeDiscount * (dto.DiscountPercent / 100m), 2);
        invoice.GrandTotal     = Math.Round(beforeDiscount - invoice.DiscountAmount, 2);

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        // Reload with navigation props
        var created = await _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .FirstAsync(i => i.Id == invoice.Id);

        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, MapToDto(created));
    }

    // PUT api/invoices/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var valid = new[] { "draft", "pending", "paid", "overdue" };
        if (!valid.Contains(status)) return BadRequest("Invalid status.");

        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice == null) return NotFound();

        invoice.Status    = status;
        invoice.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/invoices/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice == null) return NotFound();
        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET api/invoices/5/fbr-json
    // Returns the FBR IRIS 2.0 formatted JSON for this invoice
    [HttpGet("{id}/fbr-json")]
    public async Task<ActionResult<FbrInvoiceJson>> GetFbrJson(int id)
    {
        var json = await _fbr.GenerateFbrJson(id);
        if (json == null) return NotFound("Invoice or company profile not found.");
        return Ok(json);
    }

    // ── Helper mapper ────────────────────────────────────────
    private static InvoiceResponseDto MapToDto(Invoice i) => new()
    {
        Id              = i.Id,
        InvoiceNumber   = i.InvoiceNumber,
        InvoiceDate     = i.InvoiceDate,
        DueDate         = i.DueDate,
        CustomerId      = i.CustomerId,
        CustomerName    = i.Customer?.Name ?? "",
        CustomerNTN     = i.Customer?.NTNCNIC,
        Status          = i.Status,
        Notes           = i.Notes,
        Subtotal        = i.Subtotal,
        GstTotal        = i.GstTotal,
        DiscountPercent = i.DiscountPercent,
        DiscountAmount  = i.DiscountAmount,
        GrandTotal      = i.GrandTotal,
        FbrSubmitted    = i.FbrSubmitted,
        Items           = i.Items.Select(item => new InvoiceItemResponseDto
        {
            Id                    = item.Id,
            ProductId             = item.ProductId,
            ProductName           = item.ProductName,
            HsCode                = item.HsCode,
            UoM                   = item.UoM,
            Quantity              = item.Quantity,
            UnitPrice             = item.UnitPrice,
            GstPercent            = item.GstPercent,
            TotalValue            = item.TotalValue,
            ValueSalesExcludingST = item.ValueSalesExcludingST,
            SalesTaxApplicable    = item.SalesTaxApplicable,
            SaleType              = item.SaleType
        }).ToList()
    };
}
