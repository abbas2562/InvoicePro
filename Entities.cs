namespace InvoiceProAPI.Models;

public class CompanyProfile
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string NTNCNIC { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Product
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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NTNCNIC { get; set; }
    public string? Province { get; set; }
    public string? Address { get; set; }
    public string RegistrationType { get; set; } = "Registered";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string Status { get; set; } = "draft";
    public string? Notes { get; set; }

    public decimal Subtotal { get; set; }
    public decimal GstTotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

    // FBR IRIS fields (for future integration)
    public string ScenarioId { get; set; } = "SN001";
    public string? InvoiceRefNo { get; set; }
    public bool FbrSubmitted { get; set; } = false;
    public string? FbrResponse { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UoM { get; set; } = "KG";
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GstPercent { get; set; }
    public decimal TotalValue { get; set; }
    public decimal ValueSalesExcludingST { get; set; }
    public decimal SalesTaxApplicable { get; set; }
    public decimal FixedNotifiedValueOrRetailPrice { get; set; }
    public decimal SalesTaxWithheldAtSource { get; set; }
    public decimal ExtraTax { get; set; }
    public string FurtherTax { get; set; } = "0";
    public string SroScheduleNo { get; set; } = "";
    public string FedPayable { get; set; } = "0";
    public string Discount { get; set; } = "0";
    public string SaleType { get; set; } = "Goods at standard rate (default)";
    public string SroItemSerialNo { get; set; } = "";
}
