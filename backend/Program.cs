using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS - Cho phép tất cả các origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve static files from frontend folder
var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "frontend");
if (Directory.Exists(frontendPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(frontendPath),
        RequestPath = ""
    });
}

// CORS phải đứng trước UseAuthorization
app.UseMiddleware<CorsMiddleware>();
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

// Default route to serve index.html
app.MapFallbackToFile("index.html");

Console.WriteLine("====================================");
Console.WriteLine("🚀 Server đang chạy tại: http://localhost:5000");
Console.WriteLine("📖 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("🌐 Frontend: http://localhost:5000");
Console.WriteLine("📁 Frontend path: " + frontendPath);
Console.WriteLine("====================================");

app.Run();
