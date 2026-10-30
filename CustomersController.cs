using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceProAPI.Data;
using InvoiceProAPI.DTOs;
using InvoiceProAPI.Models;

namespace InvoiceProAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db) => _db = db;

    // GET api/customers
    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> GetAll()
    {
        var customers = await _db.Customers
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CustomerDto
            {
                Id               = c.Id,
                Name             = c.Name,
                Phone            = c.Phone,
                Email            = c.Email,
                NTNCNIC          = c.NTNCNIC,
                Province         = c.Province,
                Address          = c.Address,
                RegistrationType = c.RegistrationType,
                InvoiceCount     = c.Invoices.Count
            })
            .ToListAsync();
        return Ok(customers);
    }

    // GET api/customers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var c = await _db.Customers
            .Include(x => x.Invoices)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (c == null) return NotFound();
        return Ok(new CustomerDto
        {
            Id               = c.Id,
            Name             = c.Name,
            Phone            = c.Phone,
            Email            = c.Email,
            NTNCNIC          = c.NTNCNIC,
            Province         = c.Province,
            Address          = c.Address,
            RegistrationType = c.RegistrationType,
            InvoiceCount     = c.Invoices.Count
        });
    }

    // POST api/customers
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Customer name is required.");

        var customer = new Customer
        {
            Name             = dto.Name.Trim(),
            Phone            = dto.Phone.Trim(),
            Email            = dto.Email?.Trim(),
            NTNCNIC          = dto.NTNCNIC?.Trim(),
            Province         = dto.Province?.Trim(),
            Address          = dto.Address?.Trim(),
            RegistrationType = dto.RegistrationType
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = customer.Id },
            new CustomerDto { Id = customer.Id, Name = customer.Name, Phone = customer.Phone,
                              Email = customer.Email, NTNCNIC = customer.NTNCNIC,
                              Province = customer.Province, Address = customer.Address,
                              RegistrationType = customer.RegistrationType });
    }

    // PUT api/customers/5
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> Update(int id, CreateCustomerDto dto)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null || !customer.IsActive) return NotFound();

        customer.Name             = dto.Name.Trim();
        customer.Phone            = dto.Phone.Trim();
        customer.Email            = dto.Email?.Trim();
        customer.NTNCNIC          = dto.NTNCNIC?.Trim();
        customer.Province         = dto.Province?.Trim();
        customer.Address          = dto.Address?.Trim();
        customer.RegistrationType = dto.RegistrationType;

        await _db.SaveChangesAsync();
        return Ok(new CustomerDto { Id = customer.Id, Name = customer.Name, Phone = customer.Phone,
                                    Email = customer.Email, NTNCNIC = customer.NTNCNIC,
                                    Province = customer.Province, Address = customer.Address,
                                    RegistrationType = customer.RegistrationType });
    }

    // DELETE api/customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        customer.IsActive = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
