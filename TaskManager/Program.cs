using TaskManager;
using TaskManager.Components;
using TaskManager.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // interakce pro client hosting model?

builder.Services.AddSingleton<DataMediator>();

builder.Services.AddSignalR(); // added for ChatRoom

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHub<ChatHub>("/chat"); // added for ChatRoom

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
