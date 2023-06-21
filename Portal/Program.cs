using DomainServices.Interfaces;
using DomainServices.Interfaces.Repositories;
using Infrastructure.Repositories;
using Portal.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationConnectionString")));
builder.Services.AddDbContext<SecurityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecurityConnectionString")));

builder.Services.AddIdentity<UserIdentity, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<SecurityDbContext>().AddSignInManager<SignInManager<UserIdentity>>().AddDefaultTokenProviders();


builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ILoggerRepository, LoggerRepository>();
builder.Services.AddScoped<IStreamRepository,StreamRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IStreamService, StreamService>();

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


// Logging configurations
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpContextAccessor();

// Session configuration
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}

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
app.MapHub<WatcherHub>("/watcherHub");

app.Run();
