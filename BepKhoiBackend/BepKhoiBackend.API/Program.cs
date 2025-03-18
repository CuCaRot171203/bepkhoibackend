using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.BusinessObject.Services;
using BepKhoiBackend.BusinessObject.Services.CustomerService;
using BepKhoiBackend.BusinessObject.Services.LoginService;
using BepKhoiBackend.BusinessObject.Services.UserService.CashierService;
using BepKhoiBackend.BusinessObject.Services.UserService.ShipperService;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.CustomerRepository;
using BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository;
using BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
//connect db context
builder.Services.AddDbContext<bepkhoiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký dịch vụ liên quan đến cashier

builder.Services.AddScoped<ICashierRepository, CashierRepository>();
builder.Services.AddScoped<ICashierService, CashierService>();
// Đăng ký dịch vụ liên quan đến shipper

builder.Services.AddScoped<IShipperRepository, ShipperRepository>();
builder.Services.AddScoped<IShipperService, ShipperService>();


// Đăng ký dịch vụ liên quan đến khách hàng
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>(); // Repository cho Customer
builder.Services.AddScoped<ICustomerService, CustomerService>(); // Service cho Customer

//interface service and repository
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

//session
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
