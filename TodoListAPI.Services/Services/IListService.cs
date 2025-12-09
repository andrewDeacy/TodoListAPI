namespace TodoListAPI.Services.Services;

/// <summary>
/// Service interface for TodoList operations.
/// TODO (Task #4): Replace object types with proper DTOs from Core/DTOs.
/// TODO (Task #8): Complete interface definition with proper DTOs (not entities).
/// TODO (Task #9): Implement all methods in ListService.
/// 
/// NOTE: Currently using object as placeholder. In Task #4, DTOs will be created in Core/DTOs,
/// and these method signatures will be updated to use proper DTO types instead of object.
/// </summary>
public interface IListService
{
    /// <summary>
    /// Gets all lists for a specific user.
    /// TODO (Task #4): Change return type from object to TodoListDto.
    /// TODO (Task #9): Implement with repository calls, map entities to DTOs.
    /// </summary>
    Task<IEnumerable<object>> GetListsForUserAsync(Guid userId);

    /// <summary>
    /// Gets a list by ID.
    /// TODO (Task #4): Change return type from object? to TodoListDto?.
    /// TODO (Task #9): Implement with repository call, map entity to DTO.
    /// </summary>
    Task<object?> GetListByIdAsync(Guid id);

    /// <summary>
    /// Creates a new list for a user.
    /// TODO (Task #4): Change parameter from object to CreateListDto, return type to TodoListDto.
    /// TODO (Task #9): Implement with repository call, map request to entity, save, return DTO.
    /// </summary>
    Task<object> CreateListAsync(Guid userId, object request);

    /// <summary>
    /// Updates an existing list.
    /// TODO (Task #4): Change parameter from object to UpdateListDto.
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
    /// TODO (Task #4): Change parameter from object to CreateListItemDto, return type to TodoItemDto.
    /// TODO (Task #9): Implement with repository call, map request to entity, save, return DTO.
    /// </summary>
    Task<object> AddItemAsync(Guid listId, object request);

    /// <summary>
    /// Removes an item from a list.
    /// TODO (Task #9): Implement with repository call, return success.
    /// TODO (Task #19): Fix typo "RemoveItmemAsync" → "RemoveItemAsync".
    /// </summary>
    Task<bool> RemoveItmemAsync(Guid listId, Guid itemId);
}
