using DomainServices.Interfaces.Repositories;
using DomainServices.Interfaces.Services;
using DomainServices.Logger;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Portal.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationConnectionString")));
builder.Services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SecurityConnectionString")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<SecurityDbContext>().AddSignInManager<SignInManager<IdentityUser>>().AddDefaultTokenProviders();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISatoshiCompensation, SatoshiCompensation>();
builder.Services.AddScoped<IloggerService, LoggerService>();


builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "AuthorizationCookieTC";
        config.LoginPath = "/User/Login";
    });

builder.Services.AddControllersWithViews();

// Build the configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddSingleton(configuration.GetConnectionString("MyDatabase"));
string connectionString = configuration.GetConnectionString("MyDatabase");

builder.Services.AddScoped<DatabaseLogger>();
builder.Services.AddScoped<IStreamRepository>(sp => new StreamRepository(connectionString));


//Logging configurations
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpContextAccessor();

// Register the logger provider
builder.Services.AddSingleton<ILoggerProvider>(sp =>
{
    var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
    return new DatabaseLoggerProvider(connectionString, httpContextAccessor);
});

// Session configuration
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Error/Error");
    app.UseDeveloperExceptionPage();
}

void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseMiddleware<PreventAccessFilterAttribute>();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
