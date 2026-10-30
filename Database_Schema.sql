-- ============================================================
--  InvoicePro Database Schema
--  Run this in SSMS against your target database
-- ============================================================

CREATE DATABASE InvoiceProDB;
GO

USE InvoiceProDB;
GO

-- ── Company Profile (seller info for FBR JSON) ──────────────
CREATE TABLE CompanyProfile (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    BusinessName    NVARCHAR(200)  NOT NULL,
    NTNCNIC         NVARCHAR(50)   NOT NULL,
    Province        NVARCHAR(100)  NOT NULL,
    Address         NVARCHAR(500)  NOT NULL,
    Phone           NVARCHAR(50)   NULL,
    Email           NVARCHAR(200)  NULL,
    UpdatedAt       DATETIME2      DEFAULT GETUTCDATE()
);
GO

-- ── Products ─────────────────────────────────────────────────
CREATE TABLE Products (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(200)  NOT NULL,
    Description     NVARCHAR(500)  NULL,
    Price           DECIMAL(18,2)  NOT NULL DEFAULT 0,
    GstPercent      DECIMAL(5,2)   NOT NULL DEFAULT 0,
    Stock           INT            NOT NULL DEFAULT 0,
    HsCode          NVARCHAR(50)   NULL,   -- FBR HS Code
    UoM             NVARCHAR(20)   NULL DEFAULT 'KG',  -- Unit of Measure
    SaleType        NVARCHAR(100)  NULL DEFAULT 'Goods at standard rate (default)',
    IsActive        BIT            NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2      DEFAULT GETUTCDATE()
);
GO

-- ── Customers ────────────────────────────────────────────────
CREATE TABLE Customers (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(200)  NOT NULL,
    Phone           NVARCHAR(50)   NOT NULL,
    Email           NVARCHAR(200)  NULL,
    NTNCNIC         NVARCHAR(50)   NULL,
    Province        NVARCHAR(100)  NULL,
    Address         NVARCHAR(500)  NULL,
    RegistrationType NVARCHAR(50)  NULL DEFAULT 'Registered', -- Registered / Unregistered
    IsActive        BIT            NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2      DEFAULT GETUTCDATE()
);
GO

-- ── Invoices ─────────────────────────────────────────────────
CREATE TABLE Invoices (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber   NVARCHAR(50)   NOT NULL UNIQUE,
    InvoiceDate     DATE           NOT NULL,
    DueDate         DATE           NULL,
    CustomerId      INT            NOT NULL REFERENCES Customers(Id),
    Status          NVARCHAR(20)   NOT NULL DEFAULT 'draft',  -- draft/pending/paid/overdue
    Notes           NVARCHAR(1000) NULL,
    Subtotal        DECIMAL(18,2)  NOT NULL DEFAULT 0,
    GstTotal        DECIMAL(18,2)  NOT NULL DEFAULT 0,
    DiscountPercent DECIMAL(5,2)   NOT NULL DEFAULT 0,
    DiscountAmount  DECIMAL(18,2)  NOT NULL DEFAULT 0,
    GrandTotal      DECIMAL(18,2)  NOT NULL DEFAULT 0,
    -- FBR IRIS fields
    ScenarioId      NVARCHAR(20)   NULL DEFAULT 'SN001',
    InvoiceRefNo    NVARCHAR(100)  NULL,
    FbrSubmitted    BIT            NOT NULL DEFAULT 0,
    FbrResponse     NVARCHAR(MAX)  NULL,
    CreatedAt       DATETIME2      DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2      DEFAULT GETUTCDATE()
);
GO

-- ── Invoice Line Items ───────────────────────────────────────
CREATE TABLE InvoiceItems (
    Id                          INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceId                   INT            NOT NULL REFERENCES Invoices(Id) ON DELETE CASCADE,
    ProductId                   INT            NULL REFERENCES Products(Id),
    ProductName                 NVARCHAR(200)  NOT NULL,
    HsCode                      NVARCHAR(50)   NULL,
    UoM                         NVARCHAR(20)   NULL DEFAULT 'KG',
    Quantity                    DECIMAL(18,4)  NOT NULL DEFAULT 1,
    UnitPrice                   DECIMAL(18,2)  NOT NULL DEFAULT 0,
    GstPercent                  DECIMAL(5,2)   NOT NULL DEFAULT 0,
    TotalValue                  DECIMAL(18,2)  NOT NULL DEFAULT 0,
    ValueSalesExcludingST       DECIMAL(18,2)  NOT NULL DEFAULT 0,
    SalesTaxApplicable          DECIMAL(18,2)  NOT NULL DEFAULT 0,
    FixedNotifiedValueOrRetailPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
    SalesTaxWithheldAtSource    DECIMAL(18,2)  NOT NULL DEFAULT 0,
    ExtraTax                    DECIMAL(18,2)  NOT NULL DEFAULT 0,
    FurtherTax                  NVARCHAR(10)   NULL DEFAULT '0',
    SroScheduleNo               NVARCHAR(100)  NULL DEFAULT '',
    FedPayable                  NVARCHAR(10)   NULL DEFAULT '0',
    Discount                    NVARCHAR(10)   NULL DEFAULT '0',
    SaleType                    NVARCHAR(100)  NULL DEFAULT 'Goods at standard rate (default)',
    SroItemSerialNo             NVARCHAR(100)  NULL DEFAULT ''
);
GO

-- ── Indexes ──────────────────────────────────────────────────
CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId);
CREATE INDEX IX_Invoices_Status     ON Invoices(Status);
CREATE INDEX IX_Invoices_Date       ON Invoices(InvoiceDate);
CREATE INDEX IX_InvoiceItems_InvoiceId ON InvoiceItems(InvoiceId);
GO

-- ── Seed: default company profile ────────────────────────────
INSERT INTO CompanyProfile (BusinessName, NTNCNIC, Province, Address, Phone, Email)
VALUES (
    'HAMZA PACKAGES INDUSTRIES (PRIVATE) LIMITED',
    '8007550',
    'PUNJAB',
    '12-KM,, NEAR GARDEZI MORR,, BAHAWALPUR ROAD, Multan Cantt. Mumtazabad Town',
    '',
    ''
);
GO

PRINT 'InvoiceProDB schema created successfully.';
