using Microsoft.Extensions.Logging;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;
using EquipTrack.Infrastructure.Services;

namespace EquipTrack.Infrastructure.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<DataSeeder> _logger;

    // Store seeded entities for reference
    private List<User> _users = new();
    private List<Asset> _assets = new();
    private List<SparePart> _spareParts = new();

    public DataSeeder(
        ApplicationDbContext context,
        IPasswordService passwordService,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Check if data already exists
            if (_context.Users.Any())
            {
                _logger.LogInformation("Database already contains data. Skipping seed.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Seed in order of dependencies
            await SeedUsersAsync();
            await SeedAssetsAsync();
            await SeedSparePartsAsync();
            await SeedWorkOrdersAsync();
            await SeedPreventiveMaintenanceAsync();

            // Save all changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        _users = new List<User>
        {
            new User
            {
                Email = "admin@equiptrack.com",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = _passwordService.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
            },
            new User
            {
                Email = "manager@equiptrack.com",
                FirstName = "Maintenance",
                LastName = "Manager",
                PasswordHash = _passwordService.HashPassword("Manager123!"),
                Role = UserRole.Manager,
                IsActive = true,
            },
            new User
            {
                Email = "tech@equiptrack.com",
                FirstName = "John",
                LastName = "Technician",
                PasswordHash = _passwordService.HashPassword("Tech123!"),
                Role = UserRole.Technician,
                IsActive = true,
            },
            new User
            {
                Email = "tech2@equiptrack.com",
                FirstName = "Sarah",
                LastName = "Smith",
                PasswordHash = _passwordService.HashPassword("Tech123!"),
                Role = UserRole.Technician,
                IsActive = true,
            }
        };

        await _context.Users.AddRangeAsync(_users);
        _logger.LogInformation("Seeded {Count} users", _users.Count);
    }

    private async Task SeedAssetsAsync()
    {
        var baseDate = DateTime.UtcNow.AddYears(-2);

        _assets = new List<Asset>
        {
            Machine.Create(
                name: "CNC Milling Machine #1",
                description: "High-precision CNC milling machine for metal parts",
                serialNumber: "CNC-2022-001",
                model: "DMG MORI NHX 5000",
                manufacturer: "DMG MORI",
                assetTag: "ASSET-001",
                location: "Production Floor - Zone A",
                purchaseDate: baseDate,
                purchasePrice: 250000m,
                criticality: AssetCriticality.Critical,
                machineTypeRef: "CNC_MILL",
                powerRating: 45.5m,
                voltageRequirement: "380V 3-phase"
            ),
            Machine.Create(
                name: "Hydraulic Press #2",
                description: "200-ton hydraulic press for metal forming",
                serialNumber: "HP-2022-002",
                model: "Schuler PHW 200",
                manufacturer: "Schuler",
                assetTag: "ASSET-002",
                location: "Production Floor - Zone B",
                purchaseDate: baseDate.AddMonths(3),
                purchasePrice: 180000m,
                criticality: AssetCriticality.High,
                machineTypeRef: "HYDRAULIC_PRESS",
                powerRating: 75m,
                voltageRequirement: "380V 3-phase"
            ),
            Machine.Create(
                name: "Conveyor Belt System",
                description: "Automated conveyor system for material handling",
                serialNumber: "CONV-2023-001",
                model: "FlexLink X85",
                manufacturer: "FlexLink",
                assetTag: "ASSET-003",
                location: "Production Floor - Main Line",
                purchaseDate: baseDate.AddMonths(8),
                purchasePrice: 45000m,
                criticality: AssetCriticality.Medium,
                machineTypeRef: "CONVEYOR",
                powerRating: 5.5m,
                voltageRequirement: "220V"
            ),
            Robot.Create(
                name: "Welding Robot #1",
                description: "6-axis industrial welding robot",
                serialNumber: "ROB-2023-001",
                model: "KUKA KR 16 R2010",
                manufacturer: "KUKA",
                assetTag: "ASSET-004",
                location: "Welding Station - Zone C",
                purchaseDate: baseDate.AddMonths(10),
                purchasePrice: 95000m,
                robotType: RobotType.Industrial,
                maxPayloadKg: 16,
                reachMeters: 2.01m,
                criticality: AssetCriticality.High,
                firmwareVersion: "v2.5.1"
            ),
            Robot.Create(
                name: "Assembly Robot #2",
                description: "Collaborative robot for assembly operations",
                serialNumber: "ROB-2023-002",
                model: "Universal Robots UR10e",
                manufacturer: "Universal Robots",
                assetTag: "ASSET-005",
                location: "Assembly Line - Zone D",
                purchaseDate: baseDate.AddMonths(12),
                purchasePrice: 55000m,
                robotType: RobotType.Collaborative,
                maxPayloadKg: 10,
                reachMeters: 1.3m,
                criticality: AssetCriticality.Medium,
                firmwareVersion: "v3.12.0"
            )
        };

        await _context.Assets.AddRangeAsync(_assets);
        _logger.LogInformation("Seeded {Count} assets", _assets.Count);
    }

    private async Task SeedSparePartsAsync()
    {
        _spareParts = new List<SparePart>
        {
            new SparePart
            {
                Name = "Hydraulic Oil Filter",
                Description = "High-performance hydraulic oil filter",
                PartNumber = "HF-2024-001",
                Category = "Hydraulic",
                Supplier = "Parker Hannifin",
                UnitPrice = 45.50m,
                QuantityInStock = 25,
                MinimumStock = 10,
                MinimumStockLevel = 10,
                Unit = "pieces",
                Location = "Warehouse - Shelf A12",
            },
            new SparePart
            {
                Name = "CNC Cutting Tool Set",
                Description = "Carbide cutting tool set for CNC machines",
                PartNumber = "CT-2024-002",
                Category = "Mechanical",
                Supplier = "Sandvik Coromant",
                UnitPrice = 320.00m,
                QuantityInStock = 8,
                MinimumStock = 5,
                MinimumStockLevel = 5,
                Unit = "sets",
                Location = "Tool Crib - Section B",
            },
            new SparePart
            {
                Name = "Conveyor Belt",
                Description = "Replacement belt for conveyor system",
                PartNumber = "CB-2024-003",
                Category = "Mechanical",
                Supplier = "FlexLink",
                UnitPrice = 850.00m,
                QuantityInStock = 3,
                MinimumStock = 2,
                MinimumStockLevel = 2,
                Unit = "pieces",
                Location = "Warehouse - Shelf C5",
            },
            new SparePart
            {
                Name = "Robot Welding Tip",
                Description = "Copper welding tip for KUKA robot",
                PartNumber = "WT-2024-004",
                Category = "Consumable",
                Supplier = "KUKA",
                UnitPrice = 12.50m,
                QuantityInStock = 50,
                MinimumStock = 20,
                MinimumStockLevel = 20,
                Unit = "pieces",
                Location = "Warehouse - Shelf D8",
            },
            new SparePart
            {
                Name = "Servo Motor",
                Description = "Replacement servo motor for robots",
                PartNumber = "SM-2024-005",
                Category = "Electrical",
                Supplier = "Siemens",
                UnitPrice = 1250.00m,
                QuantityInStock = 4,
                MinimumStock = 2,
                MinimumStockLevel = 2,
                Unit = "pieces",
                Location = "Warehouse - Shelf E3",
            },
            new SparePart
            {
                Name = "Lubricating Oil",
                Description = "Industrial lubricating oil for machines",
                PartNumber = "LO-2024-006",
                Category = "Consumable",
                Supplier = "Shell",
                UnitPrice = 85.00m,
                QuantityInStock = 15,
                MinimumStock = 8,
                MinimumStockLevel = 8,
                Unit = "liters",
                Location = "Warehouse - Shelf F1",
            }
        };

        await _context.SpareParts.AddRangeAsync(_spareParts);
        _logger.LogInformation("Seeded {Count} spare parts", _spareParts.Count);
    }

    private async Task SeedWorkOrdersAsync()
    {
        var techUser = _users.FirstOrDefault(u => u.Role == UserRole.Technician);
        var managerUser = _users.FirstOrDefault(u => u.Role == UserRole.Manager);

        var workOrders = new List<WorkOrder>
        {
            WorkOrder.Create(
                title: "Routine maintenance - CNC Machine",
                requestedDate: DateTime.UtcNow.AddDays(-5),
                assetRef: _assets[0].Id
            ),
            WorkOrder.Create(
                title: "Hydraulic leak repair",
                requestedDate: DateTime.UtcNow.AddDays(-3),
                assetRef: _assets[1].Id
            ),
            WorkOrder.Create(
                title: "Conveyor belt alignment",
                requestedDate: DateTime.UtcNow.AddDays(-1),
                assetRef: _assets[2].Id
            )
        };

        await _context.WorkOrders.AddRangeAsync(workOrders);
        _logger.LogInformation("Seeded {Count} work orders", workOrders.Count);
    }

    private async Task SeedPreventiveMaintenanceAsync()
    {
        User? techUser = _users.FirstOrDefault(u => u.Role == UserRole.Technician);

        var preventiveMaintenance = new List<PreventiveMaintenance>
        {
            new PreventiveMaintenance
            {
                Name = "Monthly CNC Calibration",
                Description = "Monthly calibration and precision check for CNC machine",
                Frequency = MaintenanceFrequency.Monthly,
                FrequencyValue = 30,
                NextDueDate = DateTime.UtcNow.AddDays(15),
                LastCompletedDate = DateTime.UtcNow.AddDays(-15),
                IsActive = true,
                EstimatedHours = 4,
                EstimatedCost = 500m,
                Instructions = "1. Check axis alignment\n2. Calibrate tool offsets\n3. Verify spindle accuracy\n4. Update maintenance log",
                AssetRef = _assets[0].Id,
                AssignedToUserRef = techUser?.Id,
            },
            new PreventiveMaintenance
            {
                Name = "Quarterly Hydraulic System Check",
                Description = "Quarterly inspection of hydraulic press system",
                Frequency = MaintenanceFrequency.Quarterly,
                FrequencyValue = 90,
                NextDueDate = DateTime.UtcNow.AddDays(30),
                LastCompletedDate = DateTime.UtcNow.AddDays(-60),
                IsActive = true,
                EstimatedHours = 6,
                EstimatedCost = 800m,
                Instructions = "1. Check hydraulic fluid levels\n2. Inspect hoses and seals\n3. Test pressure settings\n4. Replace filters if needed",
                AssetRef = _assets[1].Id,
                AssignedToUserRef = techUser?.Id,
            },
            new PreventiveMaintenance
            {
                Name = "Weekly Conveyor Inspection",
                Description = "Weekly visual inspection and lubrication of conveyor system",
                Frequency = MaintenanceFrequency.Weekly,
                FrequencyValue = 7,
                NextDueDate = DateTime.UtcNow.AddDays(3),
                LastCompletedDate = DateTime.UtcNow.AddDays(-4),
                IsActive = true,
                EstimatedHours = 2,
                EstimatedCost = 150m,
                Instructions = "1. Visual inspection of belt condition\n2. Check belt tension\n3. Lubricate bearings\n4. Clean sensors",
                AssetRef = _assets[2].Id,
                AssignedToUserRef = techUser?.Id,
            },
            new PreventiveMaintenance
            {
                Name = "Monthly Robot Maintenance",
                Description = "Monthly maintenance for welding robot",
                Frequency = MaintenanceFrequency.Monthly,
                FrequencyValue = 30,
                NextDueDate = DateTime.UtcNow.AddDays(20),
                LastCompletedDate = DateTime.UtcNow.AddDays(-10),
                IsActive = true,
                EstimatedHours = 3,
                EstimatedCost = 400m,
                Instructions = "1. Check robot arm movement\n2. Inspect welding torch\n3. Replace welding tips\n4. Update robot software if needed",
                AssetRef = _assets[3].Id,
                AssignedToUserRef = techUser?.Id,
            }
        };

        await _context.PreventiveMaintenances.AddRangeAsync(preventiveMaintenance);
        _logger.LogInformation("Seeded {Count} preventive maintenance schedules", preventiveMaintenance.Count);
    }
}