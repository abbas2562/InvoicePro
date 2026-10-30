# InvoicePro — Complete Setup Guide
## .NET 8 Backend + SQL Server (SSMS) + HTML Frontend

---

## WHAT YOU NOW HAVE

```
InvoiceProAPI/
├── Controllers/
│   ├── ProductsController.cs      — CRUD for products
│   ├── CustomersController.cs     — CRUD for customers
│   ├── InvoicesController.cs      — Invoices + FBR JSON endpoint
│   └── DashboardController.cs     — Dashboard stats + Company profile
├── Models/
│   └── Entities.cs                — All database entity classes
├── DTOs/
│   └── Dtos.cs                    — Request/response shapes + FBR JSON shape
├── Data/
│   └── AppDbContext.cs            — Entity Framework Core DB context
├── Services/
│   └── FbrService.cs              — FBR IRIS 2.0 JSON builder (API stub ready)
├── Program.cs                     — App startup, DI, CORS
├── appsettings.json               — Connection string config
├── InvoiceProAPI.csproj           — NuGet packages
├── Database_Schema.sql            — Run this in SSMS first
└── Full_System_Connected.html     — Your updated frontend (calls the API)
```

---

## STEP 1 — Install Prerequisites

### 1a. Install .NET 8 SDK
Download from: https://dotnet.microsoft.com/download/dotnet/8.0
After install, verify in terminal:
```
dotnet --version
```
Should print `8.x.x`

### 1b. Make sure SQL Server is running
Open SSMS → connect to your server.
Your server name is usually one of:
- `localhost\SQLEXPRESS`   ← most common for local installs
- `localhost`
- `(localdb)\MSSQLLocalDB`

---

## STEP 2 — Create the Database in SSMS

1. Open SSMS
2. Click **New Query**
3. Open `Database_Schema.sql` (from this folder)
4. Paste the entire content into the query window
5. Click **Execute (F5)**
6. You should see: *"InvoiceProDB schema created successfully."*
7. In Object Explorer, refresh and verify `InvoiceProDB` appears

---

## STEP 3 — Configure Connection String

Open `appsettings.json` and check/edit the connection string:

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=InvoiceProDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

**Common variations:**
| Your setup              | Change `Server=` to              |
|-------------------------|----------------------------------|
| SQL Express (default)   | `localhost\\SQLEXPRESS`          |
| Full SQL Server         | `localhost`                      |
| Named instance          | `localhost\\YourInstanceName`    |
| LocalDB                 | `(localdb)\\MSSQLLocalDB`        |

If you use SQL Server Authentication (username/password) instead of Windows Auth:
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=InvoiceProDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

---

## STEP 4 — Run the Backend

Open a terminal in the `InvoiceProAPI` folder and run:

```bash
dotnet restore        # Downloads NuGet packages (first time only)
dotnet run            # Starts the API server
```

You should see output like:
```
Now listening on: http://localhost:5000
```

To verify it works, open your browser and go to:
```
http://localhost:5000/swagger
```
You will see the Swagger API documentation UI with all endpoints listed.

---

## STEP 5 — Open the Frontend

1. Open `Full_System_Connected.html` in your browser (double-click it)
2. Make sure the API is still running in the terminal
3. The frontend will automatically load data from the API

> **Important:** The frontend expects the API at `http://localhost:5000`.
> If your API runs on a different port, edit this line at the top of the
> `<script>` section in `Full_System_Connected.html`:
> ```javascript
> const API_BASE = 'http://localhost:5000/api';
> ```

---

## STEP 6 — Set Up Your Company Profile

Before creating invoices for FBR, update your company info.

**Option A — via Swagger UI:**
1. Go to `http://localhost:5000/swagger`
2. Find `PUT /api/company`
3. Click "Try it out"
4. Paste your company details and click Execute

**Option B — via any REST client (Postman/Thunder Client):**
```
PUT http://localhost:5000/api/company
Content-Type: application/json

{
  "BusinessName": "HAMZA PACKAGES INDUSTRIES (PRIVATE) LIMITED",
  "NTNCNIC": "8007550",
  "Province": "PUNJAB",
  "Address": "12-KM, NEAR GARDEZI MORR, BAHAWALPUR ROAD, Multan Cantt",
  "Phone": "03xx-xxxxxxx",
  "Email": "info@yourcompany.com"
}
```

---

## ALL API ENDPOINTS REFERENCE

### Products
| Method | URL                  | What it does          |
|--------|----------------------|-----------------------|
| GET    | /api/products        | Get all products      |
| GET    | /api/products/{id}   | Get one product       |
| POST   | /api/products        | Create product        |
| PUT    | /api/products/{id}   | Update product        |
| DELETE | /api/products/{id}   | Soft-delete product   |

### Customers
| Method | URL                   | What it does          |
|--------|-----------------------|-----------------------|
| GET    | /api/customers        | Get all customers     |
| GET    | /api/customers/{id}   | Get one customer      |
| POST   | /api/customers        | Create customer       |
| PUT    | /api/customers/{id}   | Update customer       |
| DELETE | /api/customers/{id}   | Soft-delete customer  |

### Invoices
| Method | URL                            | What it does                   |
|--------|--------------------------------|--------------------------------|
| GET    | /api/invoices                  | Get all invoices               |
| GET    | /api/invoices/{id}             | Get one invoice with items     |
| POST   | /api/invoices                  | Create invoice (auto-numbers)  |
| PUT    | /api/invoices/{id}/status      | Update status only             |
| DELETE | /api/invoices/{id}             | Delete invoice                 |
| GET    | /api/invoices/{id}/fbr-json    | Get FBR IRIS 2.0 JSON          |

### Dashboard & Company
| Method | URL              | What it does                   |
|--------|------------------|--------------------------------|
| GET    | /api/dashboard   | Revenue, stats, charts data    |
| GET    | /api/company     | Get company profile            |
| PUT    | /api/company     | Update company profile         |

---

## FBR IRIS 2.0 JSON

Every invoice has a dedicated endpoint that returns the exact JSON format
required by FBR IRIS 2.0 (matches the format in your Excel screenshot):

```
GET http://localhost:5000/api/invoices/1/fbr-json
```

Response example:
```json
{
  "invoiceType": "Sale Invoice",
  "invoiceDate": "2025-05-14",
  "sellerNTNCNIC": "8007550",
  "sellerBusinessName": "HAMZA PACKAGES INDUSTRIES (PRIVATE) LIMITED",
  "sellerProvince": "PUNJAB",
  "sellerAddress": "12-KM, NEAR GARDEZI MORR...",
  "buyerNTNCNIC": "7918698",
  "buyerBusinessName": "MEHER SPRING PVT LIMITED",
  "buyerProvince": "PUNJAB",
  "buyerAddress": "20 KM,OFF MULTAN ROAD...",
  "invoiceRefNo": "",
  "buyerRegistrationType": "Registered",
  "scenarioId": "SN001",
  "items": [
    {
      "hsCode": "5205.1100",
      "productDescription": "COTTON YARN",
      "rate": "18%",
      "uoM": "KG",
      "quantity": 7076.16,
      "totalValues": 4878120,
      "valueSalesExcludingST": 4134000,
      "salesTaxApplicable": 744120,
      ...
    }
  ]
}
```

In the frontend, click the **`< >`** button on any invoice row to view its
FBR JSON in a new tab.

---

## CONNECTING FBR IRIS 2.0 API IN THE FUTURE

When you're ready to submit to FBR, open `Services/FbrService.cs`.
There is a commented block at the bottom called `PostToFbr()`.
Uncomment it, fill in your FBR token and URL, and call it from the controller.
The JSON structure is already 100% correct — no changes needed.

---

## TROUBLESHOOTING

| Problem                              | Fix                                                    |
|--------------------------------------|--------------------------------------------------------|
| `dotnet: command not found`          | Install .NET 8 SDK from microsoft.com/dotnet           |
| `Cannot open database InvoiceProDB`  | Run Database_Schema.sql in SSMS first                  |
| `Login failed for user`              | Check connection string username/password              |
| Frontend shows "API Error"           | Make sure `dotnet run` is still running in terminal     |
| CORS error in browser console        | API URL in HTML must match the port in terminal         |
| Port 5000 already in use             | Change port: `dotnet run --urls http://localhost:5050`  |
|                                      | Then update `API_BASE` in the HTML file                 |

---

## RUNNING IN PRODUCTION (later)

To publish a release build:
```bash
dotnet publish -c Release -o ./publish
```
Then copy the `publish/` folder to your server and run:
```bash
dotnet InvoiceProAPI.dll
```

---
*Built for InvoicePro — FBR IRIS 2.0 ready*
