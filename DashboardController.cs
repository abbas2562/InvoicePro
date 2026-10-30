using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceProAPI.Data;
using InvoiceProAPI.DTOs;
using InvoiceProAPI.Models;

namespace InvoiceProAPI.Controllers;

// ── Dashboard ────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    // GET api/dashboard
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var invoices  = await _db.Invoices.ToListAsync();
        var customers = await _db.Customers.CountAsync(c => c.IsActive);

        var totalRevenue  = invoices.Where(i => i.Status == "paid").Sum(i => i.GrandTotal);
        var pendingAmount = invoices.Where(i => i.Status == "pending").Sum(i => i.GrandTotal);

        // Last 6 months revenue
        var now     = DateTime.UtcNow;
        var monthly = new List<MonthlyRevenueDto>();
        for (int i = 5; i >= 0; i--)
        {
            var month    = now.AddMonths(-i);
            var revenue  = invoices
                .Where(inv => inv.Status == "paid"
                           && inv.InvoiceDate.Year  == month.Year
                           && inv.InvoiceDate.Month == month.Month)
                .Sum(inv => inv.GrandTotal);
            monthly.Add(new MonthlyRevenueDto
            {
                Month   = month.ToString("MMM"),
                Revenue = Math.Round(revenue, 2)
            });
        }

        return Ok(new DashboardDto
        {
            TotalRevenue   = Math.Round(totalRevenue, 2),
            PendingAmount  = Math.Round(pendingAmount, 2),
            TotalInvoices  = invoices.Count,
            TotalCustomers = customers,
            MonthlyRevenue = monthly,
            StatusCounts   = new InvoiceStatusCountDto
            {
                Paid    = invoices.Count(i => i.Status == "paid"),
                Pending = invoices.Count(i => i.Status == "pending"),
                Draft   = invoices.Count(i => i.Status == "draft"),
                Overdue = invoices.Count(i => i.Status == "overdue")
            }
        });
    }
}

// ── Company Profile ──────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly AppDbContext _db;
    public CompanyController(AppDbContext db) => _db = db;

    // GET api/company
    [HttpGet]
    public async Task<ActionResult<CompanyProfileDto>> Get()
    {
        var company = await _db.CompanyProfiles.FirstOrDefaultAsync();
        if (company == null) return NotFound("Company profile not configured.");
        return Ok(ToDto(company));
    }

    // PUT api/company  (upsert — one company profile only)
    [HttpPut]
    public async Task<ActionResult<CompanyProfileDto>> Update(CompanyProfileDto dto)
    {
        var company = await _db.CompanyProfiles.FirstOrDefaultAsync();
        if (company == null)
        {
            company = new CompanyProfile();
            _db.CompanyProfiles.Add(company);
        }
        company.BusinessName = dto.BusinessName.Trim();
        company.NTNCNIC      = dto.NTNCNIC.Trim();
        company.Province     = dto.Province.Trim();
        company.Address      = dto.Address.Trim();
        company.Phone        = dto.Phone?.Trim();
        company.Email        = dto.Email?.Trim();
        company.UpdatedAt    = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(ToDto(company));
    }

    private static CompanyProfileDto ToDto(CompanyProfile c) => new()
    {
        Id           = c.Id,
        BusinessName = c.BusinessName,
        NTNCNIC      = c.NTNCNIC,
        Province     = c.Province,
        Address      = c.Address,
        Phone        = c.Phone,
        Email        = c.Email
    };
}
