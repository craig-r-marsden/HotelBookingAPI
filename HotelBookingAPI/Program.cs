using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Data;
using HotelBookingAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Register EF Core DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register service layer
builder.Services.AddScoped<SeedService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IHotelService, HotelService>();

var app = builder.Build();

// Apply migrations automatically if in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// DEBUG: log environment and the URLs the app is listening on (fallback to ASPNETCORE_URLS)
app.Logger.LogInformation("Environment: {env}", app.Environment.EnvironmentName);
var listeningUrls = app.Urls.Any()
    ? string.Join(',', app.Urls)
    : (builder.Configuration["ASPNETCORE_URLS"] ?? "no urls configured");
app.Logger.LogInformation("Listening on: {urls}", listeningUrls);

// Maps the OpenAPI spec document, view at /openapi/v1.json
app.MapOpenApi();

// Add Swagger UI for testing endpoints
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi/v1.json", "HotelBooking API v1");
    c.RoutePrefix = "swagger"; // UI available at /swagger or /swagger/index.html
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
