using TodoListAPI.Repository.Models;

namespace TodoListAPI.Repository.Repositories;

/// <summary>
/// Repository interface for TodoItem operations.
/// Provides data access methods for TodoItem entities.
/// </summary>
public interface IListItemRepository
{
    /// <summary>
    /// Gets a todo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item.</param>
    /// <returns>The TodoItem entity if found, null otherwise.</returns>
    Task<TodoItem?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all todo items for a specific list.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <returns>A collection of TodoItem entities for the list.</returns>
    Task<IEnumerable<TodoItem>> GetByListIdAsync(Guid listId);

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="todoItem">The TodoItem entity to create.</param>
    /// <returns>The created TodoItem entity with generated Id and timestamps.</returns>
    Task<TodoItem> CreateAsync(TodoItem todoItem);

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="todoItem">The TodoItem entity with updated properties.</param>
    /// <returns>The updated TodoItem entity.</returns>
    Task<TodoItem> UpdateAsync(TodoItem todoItem);

    /// <summary>
    /// Deletes a todo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item to delete.</param>
    /// <returns>True if the item was deleted, false if it was not found.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Marks a todo item as completed or not completed.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item.</param>
    /// <param name="isCompleted">True to mark as completed, false to mark as not completed.</param>
    /// <returns>The updated TodoItem entity if found, null otherwise.</returns>
    Task<TodoItem?> MarkCompleteAsync(Guid id, bool isCompleted);
}

