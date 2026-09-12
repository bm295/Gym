var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Gym.WebUI.Services.Ui.IToastService, Gym.WebUI.Services.Ui.ToastService>();
builder.Services.AddScoped<Gym.WebUI.Services.Ui.IAppShellState, Gym.WebUI.Services.Ui.AppShellState>();
builder.Services.AddScoped<Gym.WebUI.Services.Ui.IUiLocalizer, Gym.WebUI.Services.Ui.UiLocalizer>();
builder.Services.AddScoped<Gym.WebUI.Services.Prototype.IPrototypeGymState, Gym.WebUI.Services.Prototype.PrototypeGymState>();
builder.Services.AddScoped<Gym.WebUI.Services.Authentication.IAccessTokenProvider, Gym.WebUI.Services.Authentication.HttpContextAccessTokenProvider>();
builder.Services.AddHttpClient<Gym.WebUI.Services.CheckIns.ICheckInApiClient, Gym.WebUI.Services.CheckIns.AuthenticatedCheckInApiClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException("Api:BaseUrl must be configured.");
    client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
