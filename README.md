# EquipTrack CMMS - Computerized Maintenance Management System

A comprehensive, production-ready CMMS built with .NET 9, following Clean Architecture principles and microservices architecture.

## 🏗️ Architecture

This project follows **Clean Architecture** with domain-driven design principles and microservices architecture:

```
src/
├── Core/
│   ├── EquipTrack.Core/           # Core business logic and interfaces
│   ├── EquipTrack.Domain/         # Domain entities, enums, interfaces
│   ├── EquipTrack.Application/    # Application services, DTOs, interfaces
│   └── EquipTrack.Utilities/      # Shared utilities and helpers
├── Infrastructure/
│   ├── EquipTrack.Infrastructure/ # Data access, external services
│   └── EquipTrack.RabbitMQ/      # Message queue implementation
├── EquipTrack.Dashboard.API/      # Web API controllers, configuration
└── EquipTrack.Query/             # Query services for data retrieval

tests/
├── EquipTrack.RabbitMQ.Tests/    # Unit tests for message queue
└── EquipTrack.Tests.Integration/ # Integration tests
```

## 🚀 Features

### Core Modules
- **Assets Management** - Register, track, and manage equipment
- **Work Orders Management** - Create, assign, and track maintenance work
- **Preventive Maintenance** - Schedule recurring maintenance tasks
- **Inventory & Spare Parts** - Manage spare parts and stock levels
- **Users & Roles** - Role-based access control (Admin, Manager, Technician)
- **Reports & Dashboards** - Asset performance and maintenance analytics

### Technical Features
- **Clean Architecture** with SOLID principles and DDD
- **Microservices Architecture** for scalability
- **Message Queue Integration** with RabbitMQ for async communication
- **JWT Authentication** with role-based authorization
- **Entity Framework Core** with SQL Server
- **Repository + Unit of Work** pattern
- **AutoMapper** for object mapping
- **Swagger/OpenAPI** documentation
- **Serilog** structured logging
- **xUnit** integration tests
- **Database migrations** and seeding

## 🛠️ Technology Stack

- **Backend**: .NET 9
- **Architecture**: Microservices
- **Message Queue**: RabbitMQ
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI
- **Logging**: Serilog
- **Testing**: xUnit, FluentAssertions
- **Mapping**: AutoMapper
- **Validation**: FluentValidation

## 📋 Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB for development)
- RabbitMQ Server
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd CMMS
```

### 2. Update Connection String
Update the connection string in `appsettings.json` and `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EquipTrackCMMS;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Run Database Migrations
```bash
cd src/EquipTrack.Dashboard.API
dotnet ef database update
```

### 4. Run the Application
```bash
dotnet run
```

The API will be available at:
- **Swagger UI**: `https://localhost:7000` (or your configured port)
- **API Base**: `https://localhost:7000/api/v1`

### 5. Default Users
The application seeds the following default users:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@equiptrack.com | Admin123! |
| Manager | manager@equiptrack.com | Manager123! |
| Technician | tech1@equiptrack.com | Tech123! |
| Technician | tech2@equiptrack.com | Tech123! |

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run integration tests only
dotnet test tests/EquipTrack.Tests.Integration/
```

## 📚 API Documentation

Once the application is running, visit the Swagger UI at the root URL to explore the API endpoints:

### Authentication Endpoints
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/logout` - User logout

### Core Endpoints (Planned)
- `/api/v1/assets` - Asset management
- `/api/v1/workorders` - Work order management
- `/api/v1/spareparts` - Spare parts inventory
- `/api/v1/preventive-maintenance` - Preventive maintenance schedules
- `/api/v1/users` - User management
- `/api/v1/reports` - Reports and analytics

## 🔧 Configuration

### JWT Settings
Configure JWT authentication in `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "EquipTrack.API",
    "Audience": "EquipTrack.Client",
    "ExpirationMinutes": 60
  }
}
```

### Database Configuration
The application uses SQL Server by default. For other databases, update the connection string and provider in `DependencyInjection.cs`.

### Logging Configuration
Serilog is configured to write to both console and file. Logs are stored in the `logs/` directory.

## 🏢 Domain Model

### Core Entities
- **User** - System users with roles
- **Asset** - Equipment and machinery
- **WorkOrder** - Maintenance work requests
- **SparePart** - Inventory items
- **WorkOrderSparePart** - Parts used in work orders
- **PreventiveMaintenance** - Scheduled maintenance tasks

### Enums
- **UserRole**: Admin, Manager, Technician
- **AssetStatus**: Active, Inactive, UnderMaintenance, OutOfService, Disposed
- **WorkOrderStatus**: Open, InProgress, OnHold, Completed, Cancelled
- **WorkOrderPriority**: Low, Medium, High, Critical
- **WorkOrderType**: Corrective, Preventive, Emergency, Inspection

## 🔮 Future Enhancements

- **Event Sourcing**: Implement event sourcing pattern for audit trails
- **Service Discovery**: Add service registry and discovery
- **API Gateway**: Implement API gateway for microservices
- **Frontend**: Blazor Server/WebAssembly UI
- **Mobile App**: .NET MAUI
- **Cloud Deployment**: Docker containers + Azure DevOps
- **Real-time Updates**: SignalR for live notifications
- **File Uploads**: Asset images and work order attachments
- **Advanced Reporting**: Charts and analytics dashboard
- **Email Notifications**: Automated alerts and reminders
- **Barcode/QR Scanning**: Asset identification
- **IoT Integration**: Sensor data and predictive maintenance

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support and questions:
- Email: support@equiptrack.com
- Documentation: [API Documentation](https://localhost:7000)
- Issues: GitHub Issues

---

**EquipTrack CMMS** - Streamlining maintenance management with modern technology.