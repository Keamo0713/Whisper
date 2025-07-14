// Whisper/Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Whisper.Services; // Make sure to include this namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register GeminiApiService for dependency injection
// HttpClient is also registered here for use in GeminiApiService
builder.Services.AddHttpClient<GeminiApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configure the default route to ConfessionController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Confession}/{action=Create}/{id?}"); // THIS LINE IS CRUCIAL: Changed default controller to Confession

app.Run();
