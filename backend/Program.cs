using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Google AI Service
builder.Services.AddScoped<GoogleAIService>();
builder.Services.AddHttpClient<GoogleAIService>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "frontend");
if (Directory.Exists(frontendPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(frontendPath),
        RequestPath = ""
    });
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers(); // ← Quan trọng!

// Không dùng MapFallbackToFile để tránh conflict với API
// app.MapFallbackToFile("index.html");

Console.WriteLine("====================================");
Console.WriteLine("🚀 Server đang chạy tại: http://localhost:5000");
Console.WriteLine("📖 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("🌐 Frontend: http://localhost:5000");
Console.WriteLine("📁 Frontend path: " + frontendPath);
Console.WriteLine("====================================");

app.Run();
