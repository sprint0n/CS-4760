using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using University_Grant_Application_System.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<University_Grant_Application_SystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("University_Grant_Application_SystemContext") ?? throw new InvalidOperationException("Connection string 'University_Grant_Application_SystemContext' not found.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
