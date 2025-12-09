using Microsoft.EntityFrameworkCore;
using TodoListAPI.Repository.Models;

namespace TodoListAPI.Repository.Repositories;

/// <summary>
/// Repository implementation for TodoItem operations.
/// Provides data access methods for TodoItem entities using Entity Framework Core.
/// </summary>
public class ListItemRepository : IListItemRepository
{
    private readonly TodoListDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ListItemRepository class.
    /// </summary>
    /// <param name="context">The database context for TodoItem operations.</param>
    public ListItemRepository(TodoListDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a todo item by its unique identifier.
    /// </summary>
    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        return await _context.TodoItems
            .Include(ti => ti.TodoList)
            .FirstOrDefaultAsync(ti => ti.Id == id);
    }

    /// <summary>
    /// Gets all todo items for a specific list.
    /// </summary>
    public async Task<IEnumerable<TodoItem>> GetByListIdAsync(Guid listId)
    {
        return await _context.TodoItems
            .Where(ti => ti.ListId == listId)
            .OrderBy(ti => ti.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        if (todoItem == null)
            throw new ArgumentNullException(nameof(todoItem));

        // Set timestamps
        todoItem.CreatedDate = DateTime.UtcNow;
        todoItem.UpdatedDate = DateTime.UtcNow;

        // Generate Id if not set
        if (todoItem.Id == Guid.Empty)
        {
            todoItem.Id = Guid.NewGuid();
        }

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return todoItem;
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    public async Task<TodoItem> UpdateAsync(TodoItem todoItem)
    {
        if (todoItem == null)
            throw new ArgumentNullException(nameof(todoItem));

        // Update timestamp
        todoItem.UpdatedDate = DateTime.UtcNow;

        _context.TodoItems.Update(todoItem);
        await _context.SaveChangesAsync();

        return todoItem;
    }

    /// <summary>
    /// Deletes a todo item by its unique identifier.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
            return false;

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Marks a todo item as completed or not completed.
    /// </summary>
    public async Task<TodoItem?> MarkCompleteAsync(Guid id, bool isCompleted)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
            return null;

        todoItem.IsCompleted = isCompleted;
        todoItem.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return todoItem;
    }
}

