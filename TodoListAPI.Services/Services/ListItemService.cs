using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service implementation for TodoItem operations.
/// Uses DTOs for data transfer (never returns entities).
/// TODO (Task #9): Implement all methods with repository calls and business logic.
/// </summary>
public class ListItemService : IListItemService
{
    /// <summary>
    /// TODO (Task #13): Inject IListItemRepository via constructor.
    /// </summary>
    public ListItemService()
    {
        // TODO (Task #13): Add constructor injection for repository
    }

    /// <summary>
    /// Gets a todo item by its unique identifier.
    /// TODO (Task #9): Call _listItemRepository.GetByIdAsync(id), map entity to DTO, return DTO.
    /// </summary>
    public Task<TodoItemDto?> GetByIdAsync(Guid id)
    {
        // TODO (Task #9): Call _listItemRepository.GetByIdAsync(id)
        // TODO (Task #9): Map entity to DTO (or null if not found)
        // TODO (Task #9): Return DTO
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Gets all todo items for a specific list.
    /// TODO (Task #9): Call _listItemRepository.GetByListIdAsync(listId), map entities to DTOs, return DTOs.
    /// </summary>
    public Task<IEnumerable<TodoItemDto>> GetByListIdAsync(Guid listId)
    {
        // TODO (Task #9): Call _listItemRepository.GetByListIdAsync(listId)
        // TODO (Task #9): Map entities to DTOs
        // TODO (Task #9): Return DTOs
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Creates a new todo item.
    /// TODO (Task #9): Validate list exists, map request to entity, call _listItemRepository.CreateAsync(entity), map entity to DTO, return DTO.
    /// </summary>
    public Task<TodoItemDto> CreateAsync(Guid listId, CreateListItemRequest request)
    {
        // TODO (Task #9): Validate list exists
        // TODO (Task #9): Map request to entity
        // TODO (Task #9): Call _listItemRepository.CreateAsync(entity)
        // TODO (Task #9): Map entity to DTO
        // TODO (Task #9): Return DTO
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Updates an existing todo item.
    /// TODO (Task #9): Get existing entity via _listItemRepository.GetByIdAsync(id), if not found return null,
    /// update entity properties from request, call _listItemRepository.UpdateAsync(entity), map to DTO, return DTO.
    /// </summary>
    public Task<TodoItemDto?> UpdateAsync(Guid id, CreateListItemRequest request)
    {
        // TODO (Task #9): Get existing entity via _listItemRepository.GetByIdAsync(id)
        // TODO (Task #9): If not found, return null
        // TODO (Task #9): Update entity properties from request
        // TODO (Task #9): Call _listItemRepository.UpdateAsync(entity)
        // TODO (Task #9): Map entity to DTO
        // TODO (Task #9): Return DTO
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Deletes a todo item.
    /// TODO (Task #9): Call _listItemRepository.DeleteAsync(id), return true if deleted, false if not found.
    /// </summary>
    public Task<bool> DeleteAsync(Guid id)
    {
        // TODO (Task #9): Call _listItemRepository.DeleteAsync(id)
        // TODO (Task #9): Return true if deleted, false if not found
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Marks a todo item as completed or not completed.
    /// TODO (Task #9): Call _listItemRepository.MarkCompleteAsync(id, isCompleted), map entity to DTO, return DTO.
    /// </summary>
    public Task<TodoItemDto?> MarkCompleteAsync(Guid id, bool isCompleted)
    {
        // TODO (Task #9): Call _listItemRepository.MarkCompleteAsync(id, isCompleted)
        // TODO (Task #9): Map entity to DTO (or null if not found)
        // TODO (Task #9): Return DTO
        throw new NotImplementedException("To be implemented in Task #9");
    }
}
