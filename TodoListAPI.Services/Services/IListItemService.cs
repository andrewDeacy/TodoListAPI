using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service interface for TodoItem operations.
/// Uses DTOs for data transfer between layers (never returns entities).
/// All methods return DTOs as specified in Task #8.
/// </summary>
public interface IListItemService
{
    /// <summary>
    /// Gets a todo item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item.</param>
    /// <returns>The TodoItemDto if found, null otherwise.</returns>
    Task<TodoItemDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all todo items for a specific list.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <returns>A collection of TodoItemDto objects for the list.</returns>
    Task<IEnumerable<TodoItemDto>> GetByListIdAsync(Guid listId);

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="request">The request containing item data.</param>
    /// <returns>The created TodoItemDto.</returns>
    Task<TodoItemDto> CreateAsync(Guid listId, CreateListItemRequest request);

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="id">The unique identifier of the todo item to update.</param>
    /// <param name="request">The request containing updated item data.</param>
    /// <returns>The updated TodoItemDto if found, null otherwise.</returns>
    Task<TodoItemDto?> UpdateAsync(Guid id, CreateListItemRequest request);

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
    /// <returns>The updated TodoItemDto if found, null otherwise.</returns>
    Task<TodoItemDto?> MarkCompleteAsync(Guid id, bool isCompleted);

    /// <summary>
    /// Reorders todo items within a list by updating their Order values.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="request">The request containing item ID to order position mappings.</param>
    /// <returns>True if reordering was successful, false if validation failed.</returns>
    Task<bool> ReorderItemsAsync(Guid listId, ReorderItemsRequest request);
}