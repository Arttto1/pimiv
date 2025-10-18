using KanbanWeb.Components;
using KanbanWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configurar HttpClient com BaseAddress
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";
Console.WriteLine($"[PROGRAM] API Base URL: {apiBaseUrl}");

builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<TicketService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SessionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine("Aplicação web iniciada!");
app.Run();
