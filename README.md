# InvoicePro – Invoice Management System

**InvoicePro** is an ASP.NET Core Web API application designed to simplify business invoice management, customer records, product management, and financial operations.

Built with C# and ASP.NET Core, the project provides a structured backend architecture with dedicated controllers, database management, and an FBR integration service component.

The project also provides the foundation for integrating Pakistan's Federal Board of Revenue (FBR) Digital Invoicing APIs, enabling businesses to move towards compliant electronic invoicing workflows.

## 🚀 Key Features

- **Invoice Management:** Manage and maintain business invoices through dedicated API endpoints.
- **Customer Management:** Create and manage customer records.
- **Product Management:** Organize and maintain product information.
- **Dashboard:** Dashboard API for retrieving business-related information.
- **Database Management:** Centralized database context and SQL database schema.
- **FBR Integration:** Dedicated FBR service component for integrating with Pakistan's Digital Invoicing system.
- **RESTful API Architecture:** Organized API endpoints using ASP.NET Core controllers.
- **Scalable Backend:** Modular architecture designed for future enhancements.

## 🛠️ Technology Stack

| Technology | Purpose |
|---|---|
| C# | Backend programming language |
| ASP.NET Core Web API | Backend framework |
| .NET | Application platform |
| SQL Server | Database |
| Entity Framework Core | Database access (if configured) |
| REST APIs | Communication architecture |
| HTML | System integration/test page |

## 📁 Project Structure

```text
InvoicePro/
│
├── AppDbContext.cs
├── appsettings.json
├── CustomersController.cs
├── DashboardController.cs
├── Database_Schema.sql
├── Dtos.cs
├── Entities.cs
├── FbrService.cs
├── Full_System_Connected.html
├── InvoiceProAPI.csproj
├── InvoicesController.cs
├── ProductsController.cs
├── Program.cs
│
├── SETUP_GUIDE.md
└── README.md
```

### Main Components

- **AppDbContext.cs:** Database context for managing application data.
- **Entities.cs:** Defines application entities and data models.
- **Dtos.cs:** Data Transfer Objects used for API communication.
- **CustomersController.cs:** Handles customer-related API operations.
- **InvoicesController.cs:** Handles invoice-related API operations.
- **ProductsController.cs:** Handles product-related API operations.
- **DashboardController.cs:** Provides dashboard-related functionality.
- **FbrService.cs:** Dedicated service component for FBR API integration.
- **Database_Schema.sql:** Contains the SQL database schema.
- **Program.cs:** Application configuration and startup.
- **appsettings.json:** Application configuration settings.

## ⚙️ Installation & Setup

### Prerequisites

Make sure you have the following installed:

- .NET SDK
- SQL Server
- SQL Server Management Studio (SSMS)
- Visual Studio or Visual Studio Code
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/abbas2562/InvoicePro.git
```

### 2. Navigate to the Project Directory

```bash
cd InvoicePro
```

### 3. Configure the Database

1. Open SQL Server Management Studio.
2. Create the required database.
3. Execute the `Database_Schema.sql` script.
4. Configure your database connection string in `appsettings.json`.

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Run the Application

```bash
dotnet run
```

The API will start on the configured application URL.

## 🇵🇰 FBR Digital Invoicing API Integration

InvoicePro includes a dedicated `FbrService.cs` component intended for integration with Pakistan's Federal Board of Revenue Digital Invoicing system.

The integration is designed to support the following capabilities:

- Connection with FBR Digital Invoicing APIs.
- Submission of electronic invoices.
- Invoice data validation.
- Processing API responses.
- Handling FBR invoice references and identifiers.
- Sandbox testing and production integration.

**Integration Note:** Actual FBR API functionality depends on the implementation within `FbrService.cs`, valid API credentials, and the applicable FBR onboarding and authorized integrator requirements. The presence of this service does not by itself establish a completed or production-ready integration.

## 🔮 Future Enhancements

- Complete FBR Digital Invoicing integration.
- Automated tax calculations.
- Invoice PDF generation and downloads.
- Advanced financial reporting.
- Payment tracking.
- User authentication and role-based access control.
- API documentation using Swagger/OpenAPI.
- Improved dashboard analytics.

## 🔐 Security Considerations

- Secure database connection management.
- Environment-based configuration.
- Protection of API credentials and access tokens.
- Input validation and error handling.
- Secure handling of customer and financial information.

Never commit passwords, API keys, or sensitive configuration files to GitHub.

## 👨‍💻 Developer

**Muhammad Abbas Zaidi**

- GitHub: [@abbas2562](https://github.com/abbas2562)
- Repository: [InvoicePro](https://github.com/abbas2562/InvoicePro)

## 📄 License

This project is currently intended for development and demonstration purposes. Licensing terms can be defined as the project evolves.

---

**InvoicePro – Simplifying Invoicing. Streamlining Business.**
