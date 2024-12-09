using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Lägg till tjänster i DI-containern

builder.Services.AddControllers(); // Registrerar controller-tjänster för MVC mönstret

// Konfigurerar en In-Memory-databas för TodoContext
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

// Registrerar TodoItemService som en scoped-tjänst, vilket innebär att en ny instans 
// skapas för varje HTTP-anrop
builder.Services.AddScoped<TodoItemService>();

// Konfigurerar Swagger-verktyg för att automatiskt generera API-dokumentation
// och användargränssnitt för att testa API:et
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Konfiguration av HTTP-förfrågningspipelinje
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Aktiverar middleware för att generera Swagger JSON endpoint
    app.UseSwaggerUI(); // Aktiverar SwaggerUI, ett interaktivt interface för API-dokumentation
}

app.UseHttpsRedirection(); // Tvingar omdirigering av inkommande HTTP-begäran till HTTPS

app.UseAuthorization(); // Lägg till middleware för att hantera auktorisering

app.MapControllers(); // Mappar controllers till slutpunkter

app.RunAsync(); // Startar applikationen