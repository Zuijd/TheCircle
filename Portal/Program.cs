using DomainServices.Interfaces.Repositories;
using DomainServices.Interfaces.Services;
using DomainServices.Logger;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portal.Controllers;
using Portal.Hubs;
using SignalRChat.Hubs;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationConnectionString")));
builder.Services.AddDbContext<SecurityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecurityConnectionString")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<SecurityDbContext>()
    .AddSignInManager<SignInManager<IdentityUser>>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Services 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISatoshiCompensation, SatoshiCompensation>();
builder.Services.AddScoped<IloggerService, LoggerService>();

builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "AuthorizationCookieTC";
        config.LoginPath = "/User/Login";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = null;
});

builder.Services.AddScoped<IStreamRepository>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("ApplicationConnectionString");
    return new StreamRepository(connectionString);
});

// Logging configurations
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpContextAccessor();

// Register the logger provider
builder.Services.AddSingleton<ILoggerProvider>(sp =>
{
    var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
    var applicationConnectionString = builder.Configuration.GetConnectionString("ApplicationConnectionString");
    return new DatabaseLoggerProvider(applicationConnectionString, httpContextAccessor);
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

app.MapHub<StreamHub>("/streamHub");
app.MapHub<ChatHub>("/chatHub");

app.Run();
