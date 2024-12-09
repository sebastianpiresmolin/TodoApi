using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

// Definierar kontextklass som hanterar anslutning och interaktion med databasen
public class TodoContext : DbContext
{
    // Konstruktor som tar DbContextOptions av typen TodoContext.
    // Dessa options konfigurerar databaskontexten (exempelvis vilken databas som ska användas).
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    // DbSet<TodoItem> representerar en samling av TodoItems i databasen
    // och används för att utföra CRUD-operationer på TodoItems-tabellen.
    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}