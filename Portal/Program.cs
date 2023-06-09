using DomainServices.Interfaces.Repositories;
using DomainServices.Logger;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Portal.Controllers;
using System.Drawing.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationConnectionString")));
builder.Services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SecurityConnectionString")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<SecurityDbContext>().AddSignInManager<SignInManager<IdentityUser>>().AddDefaultTokenProviders();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllersWithViews();

// Build the configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.Build();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
string connectionString = configuration.GetConnectionString("MyDatabase");
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ILoggerProvider>(sp =>
{
    var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
    return new DatabaseLoggerProvider(connectionString, httpContextAccessor);
});

//builder.Logging.AddProvider(new DatabaseLoggerProvider(connectionString, "System"));

//Satoshi
builder.Services.AddScoped<ISatoshiCompensation, SatoshiCompensation>();

//Repositories
builder.Services.AddScoped<IStreamRepository>(sp => new StreamRepository(connectionString));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();