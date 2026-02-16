using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using University_Grant_Application_System.Data;
var builder = WebApplication.CreateBuilder(args);

//Make it so the connection string autofills the right directory
string projectRoot = builder.Environment.ContentRootPath;
string appDataPath = Path.Combine(projectRoot, "App_Data");
AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/AdminDashboard", "AdminOnly");
    options.Conventions.AuthorizePage("/CommitteeDashboard", "CommitteeChairOnly");
    options.Conventions.AuthorizePage("/ComMemberDashboard", "ComMemberOnly");
    options.Conventions.AuthorizePage("/DeptChairDashboard", "ChairOnly");
    //Authorize Page for pages, authorize folder for folders, make sure the Policy matches in Program.cs and Index.cshtml.cs, just gotta lock the DepartmentDash the same way
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
    options.AddPolicy("CommitteeChairOnly", policy => policy.RequireClaim("CommitteeStatus", "chair"));
    options.AddPolicy("ComMemberOnly", policy => policy.RequireClaim("CommitteeStatus", "member"));
    options.AddPolicy("ChairOnly", policy => policy.RequireClaim("UserType", "chair"));
    //You should be able to add a "chair only" policy that checks userType to see if they're a chair
});

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "UserAuthCookie";
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/AccessDenied";
    });


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<University_Grant_Application_SystemContext>(options =>
    options.UseSqlServer(connectionString));

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();


app.Run();