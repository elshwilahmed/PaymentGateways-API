using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaymentGateway.IntegrationAPI.Data;
using PaymentGateway.IntegrationAPI.Infrastructure;
using PaymentGateway.IntegrationAPI.Infrastructure.interfaces;
using PaymentGateway.IntegrationAPI.Settings;
using SarahaAPI.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/Payment_Log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Configration
builder.Services.Configure<PaymobSettings>(
    builder.Configuration.GetSection("PaymentGateways:Paymob"));

builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("PaymentGateways:Stripe"));

// Services
builder.Services.AddHttpClient<IPaymentGateway, PaymobGateway>();

builder.Services.AddScoped<IPaymentGateway, PaymobGateway>();

builder.Services.AddScoped<IPaymentGateway, StripeGateway>();

builder.Services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();

//var conn = builder.Configuration.GetConnectionString("DbAddress");
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(conn)
//);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DbAddress")));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
