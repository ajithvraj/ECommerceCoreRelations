using ECommerceCore.Api.MIddleware;
using ECommerceCore.Application.Common;
using ECommerceCore.Application.Interfaces.CustomerInterface;
using ECommerceCore.Application.Services.CustomerServices.Interfaces;
using ECommerceCore.Application.Services.CustomerServices.Services;
using ECommerceCore.Application.Validators;
using ECommerceCore.Infrastructure.Data;
using ECommerceCore.Infrastructure.Persistance.Data;
using ECommerceCore.Infrastructure.Repository.CustomerRepository;
using ECommerceCore.Infrastructure.Repository.ProductRepository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;
using ECommerceCore.Application.Interfaces.ProductInterface;
using ECommerceCore.Application.Services.ProductServices.Interfaces;
using ECommerceCore.Application.Services.ProductServices.Services;
using Microsoft.OpenApi.Models;
using ECommerceCore.Application.Interfaces.CartInterface;
using ECommerceCore.Infrastructure.Repository.CartRepository;
using ECommerceCore.Application.Services.CartServices.Interfaces;
using ECommerceCore.Application.Services.CartServices.Services;
using ECommerceCore.Application.Interfaces.OrderIterface;
using ECommerceCore.Infrastructure.Repository.OrderRepository;
using ECommerceCore.Application.Services.Orderservice.Interfaces;
using ECommerceCore.Application.Services.Orderservice.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace ECommerceCore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));

            builder.Services.AddScoped<ICustomerRepository, CustomerRepositoryServices>();
            builder.Services.AddScoped<ICustomerServices, CustomerServices>();
            builder.Services.AddScoped<IProductRepository, ProductRepositoryServices>();
            builder.Services.AddScoped<IProductService, ProductServices>();
            builder.Services.AddScoped<ICartRepository,CartRepositoryServices>();
            builder.Services.AddScoped<ICartServices,CartServices>();
            builder.Services.AddScoped<IOrderRepository, OrderRepositoryService>();
            builder.Services.AddScoped<IOrderServices,OrderService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<CloudinaryService>();

            builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token here"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    var config = builder.Configuration;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["JwtSettings:Issuer"],
                        ValidAudience = config["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config["JwtSettings:Key"]
                                ?? throw new InvalidOperationException("JWT Key is not configured"))),

                       
                        RoleClaimType = "role",
                        NameClaimType = "name"
                    };
                });

            builder.Services.AddRateLimiter(options =>
            {
                // fixed window — 5 requests per minute for login
                options.AddFixedWindowLimiter("login", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                //  fixed window — 30 requests per minute for search
                options.AddFixedWindowLimiter("search", opt =>
                {
                    opt.PermitLimit = 30;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                // global — 100 requests per minute for all endpoints
                options.AddFixedWindowLimiter("global", opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                //  return 429 Too Many Requests
                options.RejectionStatusCode = 429;
            });

           

            builder.Services.AddAuthorization();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DataSeeder.SeedAdminAsync(context).GetAwaiter().GetResult();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}