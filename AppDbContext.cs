using Microsoft.EntityFrameworkCore;
using InvoiceProAPI.Models;

namespace InvoiceProAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CompanyProfile
        modelBuilder.Entity<CompanyProfile>(e =>
        {
            e.ToTable("CompanyProfile");
            e.Property(x => x.BusinessName).HasMaxLength(200);
            e.Property(x => x.NTNCNIC).HasMaxLength(50);
            e.Property(x => x.Province).HasMaxLength(100);
            e.Property(x => x.Address).HasMaxLength(500);
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.GstPercent).HasColumnType("decimal(5,2)");
        });

        // Customer
        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Phone).HasMaxLength(50);
            e.Property(x => x.NTNCNIC).HasMaxLength(50);
        });

        // Invoice
        modelBuilder.Entity<Invoice>(e =>
        {
            e.ToTable("Invoices");
            e.HasIndex(x => x.InvoiceNumber).IsUnique();
            e.Property(x => x.InvoiceNumber).HasMaxLength(50);
            e.Property(x => x.Status).HasMaxLength(20);
            e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.GstTotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountPercent).HasColumnType("decimal(5,2)");
            e.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.GrandTotal).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.Customer)
             .WithMany(c => c.Invoices)
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // InvoiceItem
        modelBuilder.Entity<InvoiceItem>(e =>
        {
            e.ToTable("InvoiceItems");
            e.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.GstPercent).HasColumnType("decimal(5,2)");
            e.Property(x => x.TotalValue).HasColumnType("decimal(18,2)");
            e.Property(x => x.ValueSalesExcludingST).HasColumnType("decimal(18,2)");
            e.Property(x => x.SalesTaxApplicable).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.Invoice)
             .WithMany(i => i.Items)
             .HasForeignKey(x => x.InvoiceId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Product)
             .WithMany(p => p.InvoiceItems)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
