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
// Railway: Read directly from Environment Variables
Console.WriteLine("🔍 Checking connection string sources...");

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

Console.WriteLine($"Environment Variable: {(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")) ? "NOT FOUND" : "FOUND")}");
Console.WriteLine($"Configuration: {(string.IsNullOrEmpty(builder.Configuration.GetConnectionString("DefaultConnection")) ? "NOT FOUND" : "FOUND")}");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("❌ Connection string not found in any source!");
    Console.WriteLine("Available environment variables:");
    foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
    {
        if (env.Key.ToString().Contains("MYSQL") || env.Key.ToString().Contains("Connection"))
        {
            Console.WriteLine($"  - {env.Key}");
        }
    }
    
    throw new InvalidOperationException(
        "Connection string not found!\n" +
        "Please check Railway Variables:\n" +
        "- ConnectionStrings__DefaultConnection should be set\n" +
        "- Example: Server=mysql.railway.internal;Port=3306;Database=railway;Uid=root;Pwd=yourpassword;"
    );
}

Console.WriteLine($"✅ Connection string loaded successfully!");

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
