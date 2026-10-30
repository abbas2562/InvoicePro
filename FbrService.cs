using InvoiceProAPI.DTOs;
using InvoiceProAPI.Models;
using InvoiceProAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace InvoiceProAPI.Services;

/// <summary>
/// Builds the FBR IRIS 2.0 compatible JSON from a saved invoice.
/// Call GenerateFbrJson(invoiceId) to produce the object ready for submission.
/// When you integrate the FBR API in the future, just call PostToFbr() here.
/// </summary>
public class FbrService
{
    private readonly AppDbContext _db;

    public FbrService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Generates the FBR IRIS 2.0 JSON structure for a given invoice.
    /// </summary>
    public async Task<FbrInvoiceJson?> GenerateFbrJson(int invoiceId)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice == null) return null;

        var company = await _db.CompanyProfiles.FirstOrDefaultAsync();
        if (company == null) return null;

        var fbr = new FbrInvoiceJson
        {
            invoiceType          = "Sale Invoice",
            invoiceDate          = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
            sellerNTNCNIC        = company.NTNCNIC,
            sellerBusinessName   = company.BusinessName,
            sellerProvince       = company.Province,
            sellerAddress        = company.Address,
            buyerNTNCNIC         = invoice.Customer?.NTNCNIC ?? "",
            buyerBusinessName    = invoice.Customer?.Name ?? "",
            buyerProvince        = invoice.Customer?.Province ?? "",
            buyerAddress         = invoice.Customer?.Address ?? "",
            invoiceRefNo         = invoice.InvoiceRefNo ?? "",
            buyerRegistrationType = invoice.Customer?.RegistrationType ?? "Registered",
            scenarioId           = invoice.ScenarioId,
            items                = invoice.Items.Select(item => new FbrItemJson
            {
                hsCode                          = item.HsCode ?? "",
                productDescription              = item.ProductName,
                rate                            = item.GstPercent + "%",
                uoM                             = item.UoM,
                quantity                        = item.Quantity,
                totalValues                     = item.TotalValue,
                valueSalesExcludingST           = item.ValueSalesExcludingST,
                salesTaxApplicable              = item.SalesTaxApplicable,
                fixedNotifiedValueOrRetailPrice = item.FixedNotifiedValueOrRetailPrice,
                salesTaxWithheldAtSource        = item.SalesTaxWithheldAtSource,
                extraTax                        = item.ExtraTax,
                furtherTax                      = item.FurtherTax,
                sroScheduleNo                   = item.SroScheduleNo,
                fedPayable                      = item.FedPayable,
                discount                        = item.Discount,
                saleType                        = item.SaleType,
                sroItemSerialNo                 = item.SroItemSerialNo
            }).ToList()
        };

        return fbr;
    }

    // ─────────────────────────────────────────────────────────
    // TODO (Future): Uncomment and implement when connecting FBR IRIS 2.0 API
    // ─────────────────────────────────────────────────────────
    //
    // private readonly HttpClient _http;
    // private readonly string _fbrApiUrl = "https://iris.fbr.gov.pk/api/...";
    // private readonly string _fbrToken  = "YOUR_FBR_TOKEN";
    //
    // public async Task<string> PostToFbr(int invoiceId)
    // {
    //     var json = await GenerateFbrJson(invoiceId);
    //     if (json == null) return "Invoice not found";
    //
    //     var content = new StringContent(
    //         System.Text.Json.JsonSerializer.Serialize(json),
    //         System.Text.Encoding.UTF8,
    //         "application/json"
    //     );
    //     _http.DefaultRequestHeaders.Authorization =
    //         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _fbrToken);
    //
    //     var response = await _http.PostAsync(_fbrApiUrl, content);
    //     var body     = await response.Content.ReadAsStringAsync();
    //
    //     // Mark as submitted in DB
    //     var invoice = await _db.Invoices.FindAsync(invoiceId);
    //     if (invoice != null)
    //     {
    //         invoice.FbrSubmitted = true;
    //         invoice.FbrResponse  = body;
    //         await _db.SaveChangesAsync();
    //     }
    //     return body;
    // }
}
