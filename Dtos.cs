namespace InvoiceProAPI.DTOs;

// ── Product DTOs ─────────────────────────────────────────────
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal GstPercent { get; set; }
    public int Stock { get; set; }
    public string? HsCode { get; set; }
    public string UoM { get; set; } = "KG";
    public string SaleType { get; set; } = "Goods at standard rate (default)";
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal GstPercent { get; set; }
    public int Stock { get; set; }
    public string? HsCode { get; set; }
    public string UoM { get; set; } = "KG";
    public string SaleType { get; set; } = "Goods at standard rate (default)";
}

// ── Customer DTOs ────────────────────────────────────────────
public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NTNCNIC { get; set; }
    public string? Province { get; set; }
    public string? Address { get; set; }
    public string RegistrationType { get; set; } = "Registered";
    public int InvoiceCount { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NTNCNIC { get; set; }
    public string? Province { get; set; }
    public string? Address { get; set; }
    public string RegistrationType { get; set; } = "Registered";
}

// ── Invoice DTOs ─────────────────────────────────────────────
public class InvoiceItemDto
{
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UoM { get; set; } = "KG";
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GstPercent { get; set; }
    public string SaleType { get; set; } = "Goods at standard rate (default)";
}

public class CreateInvoiceDto
{
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "draft";
    public string? Notes { get; set; }
    public decimal DiscountPercent { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public class InvoiceResponseDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerNTN { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal Subtotal { get; set; }
    public decimal GstTotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public bool FbrSubmitted { get; set; }
    public List<InvoiceItemResponseDto> Items { get; set; } = new();
}

public class InvoiceItemResponseDto
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UoM { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GstPercent { get; set; }
    public decimal TotalValue { get; set; }
    public decimal ValueSalesExcludingST { get; set; }
    public decimal SalesTaxApplicable { get; set; }
    public string SaleType { get; set; } = string.Empty;
}

// ── Dashboard DTO ────────────────────────────────────────────
public class DashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal PendingAmount { get; set; }
    public int TotalInvoices { get; set; }
    public int TotalCustomers { get; set; }
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    public InvoiceStatusCountDto StatusCounts { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class InvoiceStatusCountDto
{
    public int Paid { get; set; }
    public int Pending { get; set; }
    public int Draft { get; set; }
    public int Overdue { get; set; }
}

// ── FBR JSON DTO (matches IRIS 2.0 format) ──────────────────
public class FbrInvoiceJson
{
    public string invoiceType { get; set; } = "Sale Invoice";
    public string invoiceDate { get; set; } = string.Empty;
    public string sellerNTNCNIC { get; set; } = string.Empty;
    public string sellerBusinessName { get; set; } = string.Empty;
    public string sellerProvince { get; set; } = string.Empty;
    public string sellerAddress { get; set; } = string.Empty;
    public string buyerNTNCNIC { get; set; } = string.Empty;
    public string buyerBusinessName { get; set; } = string.Empty;
    public string buyerProvince { get; set; } = string.Empty;
    public string buyerAddress { get; set; } = string.Empty;
    public string invoiceRefNo { get; set; } = string.Empty;
    public string buyerRegistrationType { get; set; } = "Registered";
    public string scenarioId { get; set; } = "SN001";
    public List<FbrItemJson> items { get; set; } = new();
}

public class FbrItemJson
{
    public string hsCode { get; set; } = string.Empty;
    public string productDescription { get; set; } = string.Empty;
    public string rate { get; set; } = string.Empty;
    public string uoM { get; set; } = "KG";
    public decimal quantity { get; set; }
    public decimal totalValues { get; set; }
    public decimal valueSalesExcludingST { get; set; }
    public decimal salesTaxApplicable { get; set; }
    public decimal fixedNotifiedValueOrRetailPrice { get; set; }
    public decimal salesTaxWithheldAtSource { get; set; }
    public decimal extraTax { get; set; }
    public string furtherTax { get; set; } = "0";
    public string sroScheduleNo { get; set; } = "";
    public string fedPayable { get; set; } = "0";
    public string discount { get; set; } = "0";
    public string saleType { get; set; } = "Goods at standard rate (default)";
    public string sroItemSerialNo { get; set; } = "";
}

// ── Company Profile DTO ─────────────────────────────────────
public class CompanyProfileDto
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string NTNCNIC { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
