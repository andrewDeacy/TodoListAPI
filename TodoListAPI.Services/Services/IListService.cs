using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service interface for TodoList operations.
/// Uses DTOs for data transfer between layers (never returns entities).
/// All methods return DTOs as specified in Task #8.
/// </summary>
public interface IListService
{
    /// <summary>
    /// Gets all lists for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of TodoListDto objects for the user.</returns>
    Task<IEnumerable<TodoListDto>> GetListsForUserAsync(Guid userId);

    /// <summary>
    /// Gets a list by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list.</param>
    /// <returns>The TodoListDto if found, null otherwise.</returns>
    Task<TodoListDto?> GetListByIdAsync(Guid id);

    /// <summary>
    /// Creates a new list for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">The request containing list data.</param>
    /// <returns>The created TodoListDto.</returns>
    Task<TodoListDto> CreateListAsync(Guid userId, CreateListRequest request);

    /// <summary>
    /// Updates an existing list.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list to update.</param>
    /// <param name="request">The request containing updated list data.</param>
    /// <returns>True if the list was updated, false if it was not found.</returns>
    Task<bool> UpdateListAsync(Guid id, UpdateListRequest request);

    /// <summary>
    /// Deletes a list.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list to delete.</param>
    /// <returns>True if the list was deleted, false if it was not found.</returns>
    Task<bool> DeleteListAsync(Guid id);

    /// <summary>
    /// Adds an item to a list.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="request">The request containing item data.</param>
    /// <returns>The created TodoItemDto.</returns>
    Task<TodoItemDto> AddItemAsync(Guid listId, CreateListItemRequest request);

    /// <summary>
    /// Removes an item from a list.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="itemId">The unique identifier of the todo item to remove.</param>
    /// <returns>True if the item was removed, false if it was not found.</returns>
    Task<bool> RemoveItemAsync(Guid listId, Guid itemId);
}
