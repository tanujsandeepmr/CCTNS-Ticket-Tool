using logindemo.Data;
using logindemo.Models;
using logindemo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;



var builder = WebApplication.CreateBuilder(args);

// ✅ Set EPPlus license context (for version 5–7)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// ✅ Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null
        )
    )
);

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// ✅ Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// ✅ Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add MVC and Razor Pages
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();


app.UseExceptionHandler("/Home/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UsePathBase("/tickets");
// ✅ Fix request paths when behind a base path
app.Use((context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/tickets", out var remainder))
    {
        context.Request.Path = remainder;
    }
    return next();
});



app.UseStaticFiles(); // default static

// map wwwroot/uploads → /uploads
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "uploads")
    ),
    RequestPath = "/uploads"
});


app.UseRouting();           // 1️⃣ Routing
app.UseAuthentication();    // 2️⃣ Authentication middleware
app.UseAuthorization();     // 3️⃣ Authorization middleware - IMPORTANT ✅

app.UseSession();           // 4️⃣ Session
app.UseCookiePolicy();      // 5️⃣ Cookies

// ✅ Cache-control headers
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

// ✅ Request logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request for {context.Request.Path} received");
    await next();
});

// ✅ Status Code error handler
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

// ✅ Routing endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();



// ✅ Redirect root URL to /Home/Index
app.MapGet("/", context =>
{
    context.Response.Redirect("/tickets/Home/Index");
    return Task.CompletedTask;
});



// ✅ Seed users/roles
await SeedDatabaseAsync(app);

static async Task SeedDatabaseAsync(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ApplicationDbContextSeed.SeedUsersAndRolesAsync(userManager, roleManager);
        Console.WriteLine("✅ Admin users and roles seeded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Seeding failed:");
        Console.WriteLine(ex.ToString());
    }
}

app.Run();
