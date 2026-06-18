using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;

var builder = WebApplication.CreateBuilder(args);

// ----- Если переменная PORT задана (облачная среда) - переопределяем порт -----
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(int.Parse(port)); // Слушаем на всех интерфейсах
    });
}
// Если PORT не задан — используем стандартные настройки (launchSettings.json)

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// SQLite строка подключения (можно оставить как есть)
var connectionString = "Data Source=servicehub.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Инициализация БД
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    DbInitializer.Initialize(dbContext);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Перенаправление на HTTPS только в разработке
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();