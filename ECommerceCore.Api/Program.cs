using ECommerceCore.Api.MIddleware;
using ECommerceCore.Application.Common;
using ECommerceCore.Application.Interfaces.CartInterface;
using ECommerceCore.Application.Interfaces.CustomerInterface;
using ECommerceCore.Application.Interfaces.OrderIterface;
using ECommerceCore.Application.Interfaces.ProductInterface;
using ECommerceCore.Application.Services.CartServices.Interfaces;
using ECommerceCore.Application.Services.CartServices.Services;
using ECommerceCore.Application.Services.CustomerServices.Interfaces;
using ECommerceCore.Application.Services.CustomerServices.Services;
using ECommerceCore.Application.Services.Orderservice.Interfaces;
using ECommerceCore.Application.Services.Orderservice.Services;
using ECommerceCore.Application.Services.ProductServices.Interfaces;
using ECommerceCore.Application.Services.ProductServices.Services;
using ECommerceCore.Application.Validators;
using ECommerceCore.Infrastructure.Data;
using ECommerceCore.Infrastructure.Persistance.Data;
using ECommerceCore.Infrastructure.Repository.CartRepository;
using ECommerceCore.Infrastructure.Repository.CustomerRepository;
using ECommerceCore.Infrastructure.Repository.OrderRepository;
using ECommerceCore.Infrastructure.Repository.ProductRepository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;

namespace ECommerceCore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //  Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            //  Settings
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));
            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));

            // Repositories
            builder.Services.AddScoped<ICustomerRepository, CustomerRepositoryServices>();
            builder.Services.AddScoped<IProductRepository, ProductRepositoryServices>();
            builder.Services.AddScoped<ICartRepository, CartRepositoryServices>();
            builder.Services.AddScoped<IOrderRepository, OrderRepositoryService>();

            //  Services
            builder.Services.AddScoped<ICustomerServices, CustomerServices>();
            builder.Services.AddScoped<IProductService, ProductServices>();
            builder.Services.AddScoped<ICartServices, CartServices>();
            builder.Services.AddScoped<IOrderServices, OrderService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<CloudinaryService>();

            //  Validation
            builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
            builder.Services.AddFluentValidationAutoValidation();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Swagger
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

            //  Authentication — only once
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
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
                    RoleClaimType = "role",  // ✅ lowercase
                    NameClaimType = "name"   // ✅ lowercase
                };
            });
           
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            //  Authorization — only once
            builder.Services.AddAuthorization();

            // Controllers
            builder.Services.AddControllers();

            // Rate Limiter
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("login", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("search", opt =>
                {
                    opt.PermitLimit = 30;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("global", opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.RejectionStatusCode = 429;
            });

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

            // Middleware pipeline 
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}