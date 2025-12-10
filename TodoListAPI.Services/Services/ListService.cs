using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Repository.Models;
using TodoListAPI.Repository.Repositories;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service implementation for TodoList operations.
/// Uses DTOs for data transfer (never returns entities).
/// </summary>
public class ListService : IListService
{
    private readonly IListRepository _listRepository;
    private readonly IListItemRepository _listItemRepository;

    /// <summary>
    /// Initializes a new instance of the ListService class.
    /// </summary>
    /// <param name="listRepository">Repository for TodoList operations.</param>
    /// <param name="listItemRepository">Repository for TodoItem operations.</param>
    public ListService(IListRepository listRepository, IListItemRepository listItemRepository)
    {
        _listRepository = listRepository ?? throw new ArgumentNullException(nameof(listRepository));
        _listItemRepository = listItemRepository ?? throw new ArgumentNullException(nameof(listItemRepository));
    }

    /// <summary>
    /// Gets all lists for a specific user.
    /// </summary>
    public async Task<IEnumerable<TodoListDto>> GetListsForUserAsync(Guid userId)
    {
        var entities = await _listRepository.GetAllForUserAsync(userId, includeItems: true);
        return entities.Select(MapToDto);
    }

    /// <summary>
    /// Gets a list by ID.
    /// </summary>
    public async Task<TodoListDto?> GetListByIdAsync(Guid id)
    {
        var entity = await _listRepository.GetByIdAsync(id, includeItems: true);
        return entity == null ? null : MapToDto(entity);
    }

    /// <summary>
    /// Creates a new list for a user.
    /// </summary>
    public async Task<TodoListDto> CreateListAsync(Guid userId, CreateListRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Map request to entity
        var entity = new TodoList
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = userId,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        // Create via repository
        var createdEntity = await _listRepository.CreateAsync(entity);
        return MapToDto(createdEntity);
    }

    /// <summary>
    /// Updates an existing list.
    /// </summary>
    public async Task<bool> UpdateListAsync(Guid id, UpdateListRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var entity = await _listRepository.GetByIdAsync(id, includeItems: false);
        if (entity == null)
            return false;

        // Update entity properties
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.UpdatedDate = DateTime.UtcNow;

        await _listRepository.UpdateAsync(entity);
        return true;
    }

    /// <summary>
    /// Deletes a list.
    /// </summary>
    public async Task<bool> DeleteListAsync(Guid id)
    {
        return await _listRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Adds an item to a list.
    /// </summary>
    public async Task<TodoItemDto> AddItemAsync(Guid listId, CreateListItemRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Validate list exists
        var list = await _listRepository.GetByIdAsync(listId, includeItems: false);
        if (list == null)
            throw new InvalidOperationException($"Todo list with ID {listId} not found.");

        // Map request to entity
        var entity = new TodoItem
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        // Create via repository
        var createdEntity = await _listItemRepository.CreateAsync(entity);
        return MapItemToDto(createdEntity);
    }

    /// <summary>
    /// Removes an item from a list.
    /// </summary>
    public async Task<bool> RemoveItemAsync(Guid listId, Guid itemId)
    {
        // Validate item belongs to list
        var item = await _listItemRepository.GetByIdAsync(itemId);
        if (item == null)
            return false;

        if (item.ListId != listId)
            throw new InvalidOperationException($"Item {itemId} does not belong to list {listId}.");

        return await _listItemRepository.DeleteAsync(itemId);
    }

    /// <summary>
    /// Maps a TodoList entity to a TodoListDto.
    /// </summary>
    private TodoListDto MapToDto(TodoList entity)
    {
        return new TodoListDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate,
            UserId = entity.UserId,
            Items = entity.TodoItems?.Select(MapItemToDto).ToList()
        };
    }

    /// <summary>
    /// Maps a TodoItem entity to a TodoItemDto.
    /// </summary>
    private TodoItemDto MapItemToDto(TodoItem entity)
    {
        return new TodoItemDto
        {
            Id = entity.Id,
            ListId = entity.ListId,
            Title = entity.Title,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate,
            DueDate = entity.DueDate
        };
    }
}
