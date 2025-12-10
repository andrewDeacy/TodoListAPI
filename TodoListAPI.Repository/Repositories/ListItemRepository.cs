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
    /// Gets all todo items for a specific list, ordered by their Order property.
    /// </summary>
    public async Task<IEnumerable<TodoItem>> GetByListIdAsync(Guid listId)
    {
        return await _context.TodoItems
            .Where(ti => ti.ListId == listId)
            .OrderBy(ti => ti.Order)
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

        // Assign order automatically: get max order for the list and add 1
        // If list is empty, order will be 0
        var maxOrder = await _context.TodoItems
            .Where(ti => ti.ListId == todoItem.ListId)
            .Select(ti => (int?)ti.Order)
            .MaxAsync();

        todoItem.Order = (maxOrder ?? -1) + 1; // If maxOrder is null, start at 0

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

    /// <summary>
    /// Reorders todo items within a list by updating their Order values.
    /// </summary>
    public async Task<bool> ReorderItemsAsync(Guid listId, Dictionary<Guid, int> itemOrders)
    {
        if (itemOrders == null || itemOrders.Count == 0)
            return true; // Nothing to reorder

        // Validate all items belong to the specified list
        var itemIds = itemOrders.Keys.ToList();
        var items = await _context.TodoItems
            .Where(ti => itemIds.Contains(ti.Id))
            .ToListAsync();

        // Check if all items exist and belong to the list
        if (items.Count != itemIds.Count)
            return false; // Some items not found

        if (items.Any(ti => ti.ListId != listId))
            return false; // Some items don't belong to the list

        // Update order values
        foreach (var item in items)
        {
            if (itemOrders.TryGetValue(item.Id, out var newOrder))
            {
                item.Order = newOrder;
                item.UpdatedDate = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}

