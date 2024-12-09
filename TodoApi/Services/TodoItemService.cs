using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Services
{
    // Tjänstklass för att hantera affärslogik och databasoperationer för TodoItem
    public class TodoItemService
    {
        private readonly TodoContext _context;

        // Konstruktor som injicerar TodoContext för att få tillgång till databasen
        public TodoItemService(TodoContext context)
        {
            _context = context;
        }

        // Hämtar alla TodoItems och konverterar dem till DTO-objekt
        public async Task<IEnumerable<TodoItemDTO>> GetAllTodoItemsAsync()
        {
            var todoItems = await _context.TodoItems.ToListAsync();
            return todoItems.Select(item => ItemToDTO(item));
        }

        // Hämtar en enstaka TodoItem baserat på ID och konverterar till DTO
        public async Task<TodoItemDTO?> GetTodoItemByIdAsync(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            return todoItem != null ? ItemToDTO(todoItem) : null;
        }

        // Uppdaterar existerande TodoItem och hanterar diferentes scenarier
        public async Task<bool> UpdateTodoItemAsync(long id, TodoItemDTO todoDTO)
        {
            if (id != todoDTO.Id)
            {
                return false; // BadRequest scenario om ID:n inte matchar
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return false; // NotFound scenario om item ej hittas
            }

            // Uppdatera värden och spara dem
            todoItem.Name = todoDTO.Name;
            todoItem.IsComplete = todoDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return false; // Om ett konkurensproblem uppstår
            }
        }

        // Skapar en ny TodoItem
        public async Task<TodoItemDTO> CreateTodoItemAsync(TodoItemDTO todoDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoDTO.IsComplete,
                Name = todoDTO.Name
            };

            _context.TodoItems.Add(todoItem); // Lägger till i DbContext
            await _context.SaveChangesAsync(); // Sparar förändringar

            return ItemToDTO(todoItem); // Returnerar det skapade objektet som DTO
        }

        // Tar bort ett TodoItem baserat på ID
        public async Task<bool> DeleteTodoItemAsync(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return false; // NotFound scenario om item ej hittas
            }

            _context.TodoItems.Remove(todoItem); // Tar bort från DbContext
            await _context.SaveChangesAsync(); // Sparar förändringar

            return true; // Återkomst om borttagning lyckades
        }

        // Hjälpmetod för att kontrollera om ett TodoItem existerar
        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        // Hjälpmetod för att konvertera TodoItem till DTO
        private TodoItemDTO ItemToDTO(TodoItem todoItem)
        {
            return new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
        }
    }
}