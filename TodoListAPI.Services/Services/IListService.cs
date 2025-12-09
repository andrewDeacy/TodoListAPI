using TodoListAPI.Core.DTOs;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service interface for TodoList operations.
/// Uses DTOs for data transfer between layers (never returns entities).
/// TODO (Task #9): Implement all methods in ListService.
/// </summary>
public interface IListService
{
    /// <summary>
    /// Gets all lists for a specific user.
    /// TODO (Task #9): Implement with repository calls, map entities to DTOs.
    /// </summary>
    Task<IEnumerable<TodoListDto>> GetListsForUserAsync(Guid userId);

    /// <summary>
    /// Gets a list by ID.
    /// TODO (Task #9): Implement with repository call, map entity to DTO.
    /// </summary>
    Task<TodoListDto?> GetListByIdAsync(Guid id);

    /// <summary>
    /// Creates a new list for a user.
    /// TODO (Task #5): Change parameter from object to CreateListDto when request models are completed.
    /// TODO (Task #9): Implement with repository call, map request to entity, save, return DTO.
    /// </summary>
    Task<TodoListDto> CreateListAsync(Guid userId, object request);

    /// <summary>
    /// Updates an existing list.
    /// TODO (Task #5): Change parameter from object to UpdateListDto when request models are completed.
    /// TODO (Task #9): Implement with repository call, update entity, return success.
    /// </summary>
    Task<bool> UpdateListAsync(Guid id, object request);

    /// <summary>
    /// Deletes a list.
    /// TODO (Task #9): Implement with repository call, return success.
    /// </summary>
    Task<bool> DeleteListAsync(Guid id);

    /// <summary>
    /// Adds an item to a list.
    /// TODO (Task #5): Change parameter from object to CreateListItemDto when request models are completed.
    /// TODO (Task #9): Implement with repository call, map request to entity, save, return DTO.
    /// </summary>
    Task<TodoItemDto> AddItemAsync(Guid listId, object request);

    /// <summary>
    /// Removes an item from a list.
    /// TODO (Task #9): Implement with repository call, return success.
    /// TODO (Task #19): Fix typo "RemoveItmemAsync" → "RemoveItemAsync".
    /// </summary>
    Task<bool> RemoveItmemAsync(Guid listId, Guid itemId);
}
