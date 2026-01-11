using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Application.Services;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using EbikeRental.Infrastructure.Identity;
using EbikeRental.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Railway: Ensure environment variables override configuration
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorPages();

// Database - Support both SQL Server and MySQL
// Railway: Try MYSQL_URL first (Railway's standard)
Console.WriteLine("🔍 Checking connection string sources...");

var connectionString = Environment.GetEnvironmentVariable("MYSQL_URL")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// If not found, build from MYSQL* variables
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("📦 Building connection string from MYSQL* variables...");
    
    var host = Environment.GetEnvironmentVariable("MYSQLHOST");
    var port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";
    var database = Environment.GetEnvironmentVariable("MYSQLDATABASE");
    var user = Environment.GetEnvironmentVariable("MYSQLUSER");
    var password = Environment.GetEnvironmentVariable("MYSQLPASSWORD");

    Console.WriteLine($"  MYSQLHOST: {(string.IsNullOrEmpty(host) ? "NOT FOUND" : host)}");
    Console.WriteLine($"  MYSQLPORT: {(string.IsNullOrEmpty(port) ? "NOT FOUND" : port)}");
    Console.WriteLine($"  MYSQLDATABASE: {(string.IsNullOrEmpty(database) ? "NOT FOUND" : database)}");
    Console.WriteLine($"  MYSQLUSER: {(string.IsNullOrEmpty(user) ? "NOT FOUND" : user)}");
    Console.WriteLine($"  MYSQLPASSWORD: {(string.IsNullOrEmpty(password) ? "NOT FOUND" : "***HIDDEN***")}");

    if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(password))
    {
        connectionString = $"Server={host};Port={port};Database={database};Uid={user};Pwd={password};";
        Console.WriteLine($"✅ Connection string built successfully: Server={host}:{port}");
    }
    else
    {
        Console.WriteLine("❌ MYSQL* variables not found!");
        
        // Last resort: try to find any MySQL-related variables
        Console.WriteLine("🔍 Searching all environment variables...");
        var allVars = Environment.GetEnvironmentVariables();
        foreach (System.Collections.DictionaryEntry entry in allVars)
        {
            var key = entry.Key.ToString();
            if (key.Contains("MYSQL", StringComparison.OrdinalIgnoreCase) || 
                key.Contains("DATABASE", StringComparison.OrdinalIgnoreCase) ||
                key.Contains("Connection", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"  Found: {key}");
            }
        }
    }
}
else
{
    Console.WriteLine($"✅ Connection string loaded from MYSQL_URL or DATABASE_URL");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string not found!\n" +
        "Please set either:\n" +
        "1. MYSQL_URL or DATABASE_URL, OR\n" +
        "2. ConnectionStrings__DefaultConnection, OR\n" +
        "3. MYSQL* environment variables (MYSQLHOST, MYSQLPORT, MYSQLDATABASE, MYSQLUSER, MYSQLPASSWORD)"
    );
}

var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") 
    ?? Environment.GetEnvironmentVariable("DatabaseProvider") 
    ?? "MySQL";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider == "MySQL")
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
        options.UseMySql(connectionString, serverVersion);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Identity
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IRepairRepository, RepairRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPurchaseRequisitionRepository, PurchaseRequisitionRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IGoodsReceiptRepository, GoodsReceiptRepository>();
builder.Services.AddScoped<IBomRepository, BomRepository>();
builder.Services.AddScoped<IMaterialIssueRepository, MaterialIssueRepository>();
builder.Services.AddScoped<IQualityCheckRepository, QualityCheckRepository>();
builder.Services.AddScoped<IProductionReceiptRepository, ProductionReceiptRepository>();

// ? NEW: Inventory Transaction Repositories
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
builder.Services.AddScoped<IStockBalanceRepository, StockBalanceRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IRepairService, RepairService>();
builder.Services.AddScoped<IPurchasingService, PurchasingService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IBomService, BomService>();
builder.Services.AddScoped<IProductionService, ProductionService>();
builder.Services.AddScoped<IPurchaseRequisitionService, PurchaseRequisitionService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IMaterialIssueService, MaterialIssueService>();
builder.Services.AddScoped<IQualityCheckService, QualityCheckService>();

// ? NEW: Inventory Service (Must be BEFORE GoodsReceiptService and ProductionReceiptService)
builder.Services.AddScoped<IInventoryService, InventoryService>();

// ? MOVED: GoodsReceiptService and ProductionReceiptService must be registered AFTER InventoryService
builder.Services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
builder.Services.AddScoped<IProductionReceiptService, ProductionReceiptService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Seed Data and Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Starting database migration and seeding...");

        var context = services.GetRequiredService<AppDbContext>();

        // Test connection
        logger.LogInformation("Testing database connection...");
        logger.LogInformation($"Connection String (masked): {connectionString?.Substring(0, Math.Min(60, connectionString.Length))}...");
        
        try
        {
            var canConnect = await context.Database.CanConnectAsync();
            logger.LogInformation($"Database connection test: {(canConnect ? "SUCCESS" : "FAILED")}");

            if (!canConnect)
            {
                logger.LogError("Cannot connect to database. Please check connection string.");
                throw new Exception("Database connection failed");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Connection error details: {ex.Message}");
            logger.LogError($"Error type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }

        // Apply migrations
        logger.LogInformation("Applying pending migrations...");
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        logger.LogInformation($"Pending migrations count: {pendingMigrations.Count()}");

        if (pendingMigrations.Any())
        {
            foreach (var migration in pendingMigrations)
            {
                logger.LogInformation($"  - {migration}");
            }
        }

        context.Database.Migrate(); // Auto-migrate
        logger.LogInformation("✅ Database migrations applied successfully!");

        // Seed roles and admin
        logger.LogInformation("Seeding roles and admin user...");
        await IdentitySeed.SeedRolesAndAdminAsync(services);
        logger.LogInformation("✅ Database seeding completed successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ CRITICAL ERROR during database migration/seeding:");
        logger.LogError($"Error Type: {ex.GetType().Name}");
        logger.LogError($"Error Message: {ex.Message}");
        logger.LogError($"Stack Trace: {ex.StackTrace}");

        if (ex.InnerException != null)
        {
            logger.LogError($"Inner Exception: {ex.InnerException.Message}");
            logger.LogError($"Inner Stack Trace: {ex.InnerException.StackTrace}");
        }

        // Re-throw to prevent app from starting with broken database
        throw;
    }
}

app.Run();
