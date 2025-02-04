using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using MessageProject.DAL;
using MessageProject.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Настройка CORS (разрешаем WebSocket соединения для SignalR)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Message API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message API v1");
    c.RoutePrefix = "swagger"; // Swagger будет доступен по /swagger/
});

app.UseStaticFiles();

app.UseRouting();

// Подключение CORS
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Настраиваем маршруты MVC
app.MapControllers(); //API-контроллеры
app.MapHub<MessageHub>("/messagehub"); //WebSocket (SignalR)
app.MapControllerRoute( // MVC (Главная страница)
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Запускаем приложение
app.Run();
