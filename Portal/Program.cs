using Microsoft.Extensions.Options;
using Portal.Hubs;
using SignalRChat.Hubs;

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

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "AuthorizationCookieTC";
        config.LoginPath = "/User/Login";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = null;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<StreamHub>("/streamHub");
app.MapHub<ChatHub>("/chatHub");

app.Run();