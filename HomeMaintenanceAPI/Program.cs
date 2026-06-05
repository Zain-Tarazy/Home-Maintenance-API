using Microsoft.AspNetCore.Authentication.Negotiate;
using HomeMaintenanceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeMaintenanceAPI.Application.Interfaces.Repositories;
using HomeMaintenanceAPI.Infrastructure.Repositories;
using HomeMaintenanceAPI.Application.Interfaces.Services;
using HomeMaintenanceAPI.Infrastructure.Services;
using HomeMaintenanceAPI.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using HomeMaintenanceAPI.Application.Mapping;
using AutoMapper;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
        Description = "Enter your JWT token like this: Bearer YOUR_TOKEN"
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
            Array.Empty<string>()
        }
    });
});




builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

builder.Services.AddAuthorization();



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));





builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IEmailService,SMTPEmailService>();
//builder.Services.AddScoped<IEmailService, FakeEmailService>();

builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ITokenService,TokenService>();

builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();

builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();

builder.Services.AddScoped<IProviderProfileRepository, ProviderProfileRepository>();
builder.Services.AddScoped<IProviderProfileService, ProviderProfileService>();

builder.Services.AddScoped<ISubscriptionPaymentRequestRepository, SubscriptionPaymentRequestRepository>();
builder.Services.AddScoped<ISubscriptionPaymentRequestService, SubscriptionPaymentRequestService>();

builder.Services.AddScoped<IProviderSubscriptionRepository, ProviderSubscriptionRepository>();
builder.Services.AddScoped<IProviderSubscriptionService, ProviderSubscriptionService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IProviderOfferRepository, ProviderOfferRepository>();
builder.Services.AddScoped<IProviderOfferService, ProviderOfferService>();

builder.Services.AddScoped<IOrderCompletionService, OrderCompletionService>();

builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();







builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MappingProfile>();
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
