using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    // Deklarerar att denna klass är en API-kontroller och specificerar routen för API-anrop
    [Route("api/Todoitems")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        // Fält för TodoItemService som används för att utföra CRUD-operationer
        private readonly TodoItemService _todoItemService;

        // Konstruktor för controller-klassen där tjänsten injiceras
        public TodoItemController(TodoItemService _todoItemService)
        {
            this._todoItemService = _todoItemService;
        }

        // GET: api/TodoItems
        // Hämtar en lista av alla Todo-items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            var todoItems = await _todoItemService.GetAllTodoItemsAsync();
            return Ok(todoItems);
        }

        // GET: api/TodoItems/5
        // Hämtar en specifik Todo-item baserat på det angivna id:t
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItemDto = await _todoItemService.GetTodoItemByIdAsync(id);

            if (todoItemDto == null)
            {
                return NotFound(); // Returnerar 404 Not Found om itemet inte hittas
            }

            return todoItemDto;
        }

        // PUT: api/TodoItems/5
        // Uppdaterar en befintlig Todo-item
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoDTO)
        {
            var result = await _todoItemService.UpdateTodoItemAsync(id, todoDTO);

            if (!result)
            {
                if (id != todoDTO.Id)
                {
                    return BadRequest(); // Returnerar 400 Bad Request om id inte matchar
                }

                return NotFound(); // Returnerar 404 om itemet inte finns att uppdatera
            }

            return NoContent(); // Returnerar 204 No Content vid lyckad uppdatering
        }

        // POST: api/TodoItems
        // Skapar en ny Todo-item
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO)
        {
            var createdTodoItemDTO = await _todoItemService.CreateTodoItemAsync(todoDTO);

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = createdTodoItemDTO.Id },
                createdTodoItemDTO); // Returnerar 201 Created, och platsen för den nya resursen
        }

        // DELETE: api/TodoItems/5
        // Tar bort en Todo-item baserat på det givna id:t
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var result = await _todoItemService.DeleteTodoItemAsync(id);

            if (!result)
            {
                return NotFound(); // Returnerar 404 om itemet inte finns att ta bort
            }

            return NoContent(); // Returnerar 204 No Content vid lyckad borttagning
        }

        // En privat metod för att konvertera TodoItem till TodoItemDTO
        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
