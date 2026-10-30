using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceProAPI.Data;
using InvoiceProAPI.DTOs;
using InvoiceProAPI.Models;

namespace InvoiceProAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;

    // GET api/products
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var products = await _db.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => ToDto(p))
            .ToListAsync();
        return Ok(products);
    }

    // GET api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null || !p.IsActive) return NotFound();
        return Ok(ToDto(p));
    }

    // POST api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Product name is required.");

        var product = new Product
        {
            Name        = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            Price       = dto.Price,
            GstPercent  = dto.GstPercent,
            Stock       = dto.Stock,
            HsCode      = dto.HsCode?.Trim(),
            UoM         = dto.UoM,
            SaleType    = dto.SaleType
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToDto(product));
    }

    // PUT api/products/5
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, CreateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null || !product.IsActive) return NotFound();

        product.Name        = dto.Name.Trim();
        product.Description = dto.Description?.Trim();
        product.Price       = dto.Price;
        product.GstPercent  = dto.GstPercent;
        product.Stock       = dto.Stock;
        product.HsCode      = dto.HsCode?.Trim();
        product.UoM         = dto.UoM;
        product.SaleType    = dto.SaleType;

        await _db.SaveChangesAsync();
        return Ok(ToDto(product));
    }

    // DELETE api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        product.IsActive = false; // soft delete
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static ProductDto ToDto(Product p) => new()
    {
        Id          = p.Id,
        Name        = p.Name,
        Description = p.Description,
        Price       = p.Price,
        GstPercent  = p.GstPercent,
        Stock       = p.Stock,
        HsCode      = p.HsCode,
        UoM         = p.UoM,
        SaleType    = p.SaleType
    };
}
