using Microsoft.EntityFrameworkCore;
using TodoListAPI.Repository.Models;

namespace TodoListAPI.Repository.Repositories;

/// <summary>
/// Repository implementation for TodoList operations.
/// Provides data access methods for TodoList entities using Entity Framework Core.
/// </summary>
public class ListRepository : IListRepository
{
    private readonly TodoListDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ListRepository class.
    /// </summary>
    /// <param name="context">The database context for TodoList operations.</param>
    public ListRepository(TodoListDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a todo list by its unique identifier.
    /// </summary>
    public async Task<TodoList?> GetByIdAsync(Guid id, bool includeItems = false)
    {
        var query = _context.TodoLists.AsQueryable();

        if (includeItems)
        {
            query = query.Include(tl => tl.TodoItems);
        }

        return await query.FirstOrDefaultAsync(tl => tl.Id == id);
    }

    /// <summary>
    /// Gets all todo lists for a specific user.
    /// </summary>
    public async Task<IEnumerable<TodoList>> GetAllForUserAsync(Guid userId, bool includeItems = false)
    {
        var query = _context.TodoLists
            .Where(tl => tl.UserId == userId)
            .AsQueryable();

        if (includeItems)
        {
            query = query.Include(tl => tl.TodoItems);
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    public async Task<TodoList> CreateAsync(TodoList todoList)
    {
        if (todoList == null)
            throw new ArgumentNullException(nameof(todoList));

        // Set timestamps
        todoList.CreatedDate = DateTime.UtcNow;
        todoList.UpdatedDate = DateTime.UtcNow;

        // Generate Id if not set
        if (todoList.Id == Guid.Empty)
        {
            todoList.Id = Guid.NewGuid();
        }

        _context.TodoLists.Add(todoList);
        await _context.SaveChangesAsync();

        return todoList;
    }

    /// <summary>
    /// Updates an existing todo list.
    /// </summary>
    public async Task<TodoList> UpdateAsync(TodoList todoList)
    {
        if (todoList == null)
            throw new ArgumentNullException(nameof(todoList));

        // Update timestamp
        todoList.UpdatedDate = DateTime.UtcNow;

        _context.TodoLists.Update(todoList);
        await _context.SaveChangesAsync();

        return todoList;
    }

    /// <summary>
    /// Deletes a todo list by its unique identifier.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var todoList = await _context.TodoLists.FindAsync(id);
        if (todoList == null)
            return false;

        _context.TodoLists.Remove(todoList);
        await _context.SaveChangesAsync();

        return true;
    }
}

