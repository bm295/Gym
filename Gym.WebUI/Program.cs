using Gym.Application;
using Gym.Infrastructure;
using Gym.WebUI.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddSingleton<ITodoItemService, TodoItemService>();
builder.Services.AddSingleton<ThreadSafeCounterDemo>();
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();

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
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost(
        "/api/tenants/{tenantId}/branches/{branchId}/payments/{paymentId:guid}/void",
        IResult (
            string tenantId,
            string branchId,
            Guid paymentId,
            VoidPaymentRequest request,
            IPaymentService payments,
            ILogger<Program> logger) =>
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Reason)] = ["A reason is required to void a payment."]
                });
            }

            var result = payments.Void(tenantId, branchId, paymentId, request.Reason.Trim());
            if (result == VoidPaymentResult.Success)
            {
                logger.LogInformation(
                    "Payment {PaymentId} was voided for tenant {TenantId} and branch {BranchId}",
                    paymentId,
                    tenantId,
                    branchId);
            }

            return result switch
            {
                VoidPaymentResult.Success => Results.NoContent(),
                VoidPaymentResult.NotFound => Results.NotFound(),
                VoidPaymentResult.AlreadyVoided => Results.Conflict(new { message = "Payment has already been voided." }),
                _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
            };
        })
    .RequireAuthorization(AuthorizationPolicies.CanVoidPayment);

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public sealed record VoidPaymentRequest(string Reason);
