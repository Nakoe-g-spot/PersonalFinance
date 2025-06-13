using Microsoft.EntityFrameworkCore;
using PersonalFinance.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PersonalFinance.API.Repositories;
using PersonalFinance.API.Services;
using PersonalFinance.API.Services.Interfaces;
using System.Text;
using PersonalFinance.API.Middleware;




var builder = WebApplication.CreateBuilder(args);
// Đăng ký EF Core DbContext 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//Đăng ký Generic Repository cho mọi entity
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//Đăng ký UserService
builder.Services.AddScoped<IUserService, UserService>();

// Đọc thông tin JWT từ cấu hình
var jwtKey = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("Missing configuration for Jwt:Key");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

// Cấu hình Authentication và đặt các tham số để xác thực token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Xác thực issuer
            ValidateAudience = true, // Xác thực audience
            ValidateLifetime = true,  // Kiểm tra thời gian sống của token
            ValidateIssuerSigningKey = true, // Kiểm tra chữ ký token
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

//Đăng ký Controllers và Swagger API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

// cấu hình pipeline HTTP, sử dụng Swagger khi môi trường development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Thiết lập endpoint cho Swagger JSON
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonalFinance API V1");
    });

}
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();

// Đăng ký Authentication và Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
