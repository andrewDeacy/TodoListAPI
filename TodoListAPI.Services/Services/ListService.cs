using TodoListAPI.Core.DTOs;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service implementation for TodoList operations.
/// Uses DTOs for data transfer (never returns entities).
/// TODO (Task #9): Implement all methods with repository calls and business logic.
/// </summary>
public class ListService : IListService
{
    /// <summary>
    /// TODO (Task #13): Inject IListRepository and IListItemRepository via constructor.
    /// </summary>
    public ListService()
    {
        // TODO (Task #13): Add constructor injection for repositories
        // Example: public ListService(IListRepository listRepository, IListItemRepository listItemRepository)
    }

    /// <summary>
    /// Gets all lists for a specific user.
    /// TODO (Task #9): Call _listRepository.GetAllForUserAsync(userId), map entities to DTOs, return DTOs.
    /// </summary>
    public Task<IEnumerable<TodoListDto>> GetListsForUserAsync(Guid userId)
    {
        // TODO (Task #9): Call _listRepository.GetAllForUserAsync(userId)
        // TODO (Task #9): Map entities to DTOs
        // TODO (Task #9): Return DTOs
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Gets a list by ID.
    /// TODO (Task #9): Call _listRepository.GetByIdAsync(id), map entity to DTO (or null if not found), return DTO.
    /// </summary>
    public Task<TodoListDto?> GetListByIdAsync(Guid id)
    {
        // TODO (Task #9): Call _listRepository.GetByIdAsync(id)
        // TODO (Task #9): Map entity to DTO (or null if not found)
        // TODO (Task #9): Return DTO
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Creates a new list for a user.
    /// TODO (Task #5): Change parameter from object to CreateListDto when request models are completed.
    /// TODO (Task #9): Validate request, map request to entity, call _listRepository.CreateAsync(entity), map entity to DTO, return DTO.
    /// </summary>
    public Task<TodoListDto> CreateListAsync(Guid userId, object request)
    {
        // TODO (Task #9): Validate request
        // TODO (Task #9): Map request to entity
        // TODO (Task #9): Call _listRepository.CreateAsync(entity)
        // TODO (Task #9): Map entity to DTO
        // TODO (Task #9): Return DTO
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Updates an existing list.
    /// TODO (Task #5): Change parameter from object to UpdateListDto when request models are completed.
    /// TODO (Task #9): Get existing entity via _listRepository.GetByIdAsync(id), if not found return false,
    /// update entity properties from request, call _listRepository.UpdateAsync(entity), return true.
    /// </summary>
    public Task<bool> UpdateListAsync(Guid id, object request)
    {
        // TODO (Task #9): Get existing entity via _listRepository.GetByIdAsync(id)
        // TODO (Task #9): If not found, return false
        // TODO (Task #9): Update entity properties from request
        // TODO (Task #9): Call _listRepository.UpdateAsync(entity)
        // TODO (Task #9): Return true
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Deletes a list.
    /// TODO (Task #9): Call _listRepository.DeleteAsync(id), return true if deleted, false if not found.
    /// </summary>
    public Task<bool> DeleteListAsync(Guid id)
    {
        // TODO (Task #9): Call _listRepository.DeleteAsync(id)
        // TODO (Task #9): Return true if deleted, false if not found
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Adds an item to a list.
    /// TODO (Task #5): Change parameter from object to CreateListItemDto when request models are completed.
    /// TODO (Task #9): Validate list exists via _listRepository.GetByIdAsync(listId), map request to entity,
    /// call _listItemRepository.CreateAsync(entity), map entity to DTO, return DTO.
    /// </summary>
    public Task<TodoItemDto> AddItemAsync(Guid listId, object request)
    {
        // TODO (Task #9): Validate list exists via _listRepository.GetByIdAsync(listId)
        // TODO (Task #9): Map request to entity
        // TODO (Task #9): Call _listItemRepository.CreateAsync(entity)
        // TODO (Task #9): Map entity to DTO
        // TODO (Task #9): Return DTO
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Removes an item from a list.
    /// TODO (Task #9): Call _listItemRepository.DeleteAsync(itemId), return true if deleted, false if not found.
    /// TODO (Task #19): Fix method name typo "RemoveItmemAsync" → "RemoveItemAsync".
    /// </summary>
    public Task<bool> RemoveItmemAsync(Guid listId, Guid itemId)
    {
        // TODO (Task #9): Call _listItemRepository.DeleteAsync(itemId)
        // TODO (Task #9): Return true if deleted, false if not found
        throw new NotImplementedException("To be implemented in Task #9");
    }
}
