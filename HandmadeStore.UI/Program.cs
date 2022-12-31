using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository;
using HandmadeStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using HandmadeStore.Utility;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using HandmadeStore.UI.Hubs;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization(v1 => v1.ResourcesPath = "Resources");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<EmailJetSettings>(builder.Configuration.GetSection("MailJetSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});
builder.Services.AddRazorPages();
builder.Services.AddLocalization();
builder.Services.AddSignalR();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbIntializer, DbIntializer>();

builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromSeconds(100);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

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
SeedData();
app.UseAuthentication(); ;
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
var supportedCultures = new[] { "ar-EG", "en-Us" };
app.UseRequestLocalization(r =>
{
    r.AddSupportedUICultures(supportedCultures);
    r.AddSupportedCultures(supportedCultures);
    r.SetDefaultCulture("en-Us");
});
app.MapHub<ReviewsHub>("/Reviews");
app.MapHub<MassagesHub>("/Massage");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

RotativaConfiguration.Setup(builder.Environment.WebRootPath);

app.Run();

async void SeedData()
{
    using var scope = app.Services.CreateScope();
    var dbIntializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>();
    dbIntializer.Intializer().Wait();
}
