using Gym.Api.Authorization;
using Gym.Application;
using Gym.Application.CheckIns;
using Gym.Application.Repositories;
using Gym.Infrastructure;
using Gym.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IUtcClock, UtcClock>();
builder.Services.AddSingleton<InMemoryGymDataStore>();
builder.Services.AddSingleton<IBranchRepository, InMemoryBranchRepository>();
builder.Services.AddSingleton<IMemberRepository, InMemoryMemberRepository>();
builder.Services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();
builder.Services.AddSingleton<ICheckInRepository, InMemoryCheckInRepository>();
builder.Services.AddSingleton<ICheckInOperationLogger, TraceCheckInOperationLogger>();
builder.Services.AddSingleton<ICheckInService, CheckInService>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection["SigningKey"]
    ?? throw new InvalidOperationException("Jwt:SigningKey must be configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.CanCheckIn, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(
            StaffRoles.TenantAdmin,
            StaffRoles.BranchManager,
            StaffRoles.Receptionist);
        policy.AddRequirements(new TenantAndBranchAccessRequirement());
    });
    options.AddPolicy(AuthorizationPolicies.CanVoidPayment, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(StaffRoles.TenantAdmin, StaffRoles.BranchManager);
        policy.AddRequirements(new TenantAndBranchAccessRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, TenantAndBranchAccessHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

namespace Gym.Api
{
    public partial class Program;
}
