using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;

var builder = WebApplication.CreateBuilder(args);

// ====== ВАЖНО: Порт из переменной окружения, либо 5000 по умолчанию ======
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port)); // Слушаем на всех IP
});

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Подключение SQLite (для облака лучше использовать /tmp)
var connectionString = "Data Source=/tmp/servicehub.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Создание БД и инициализация
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

// В продакшене не перенаправляем на HTTPS (внешний балансировщик сам это делает)
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