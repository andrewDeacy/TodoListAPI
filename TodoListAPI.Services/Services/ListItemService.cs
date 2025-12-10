using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Repository.Models;
using TodoListAPI.Repository.Repositories;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service implementation for TodoItem operations.
/// Uses DTOs for data transfer (never returns entities).
/// </summary>
public class ListItemService : IListItemService
{
    private readonly IListItemRepository _listItemRepository;
    private readonly IListRepository _listRepository;

    /// <summary>
    /// Initializes a new instance of the ListItemService class.
    /// </summary>
    /// <param name="listItemRepository">Repository for TodoItem operations.</param>
    /// <param name="listRepository">Repository for TodoList operations (for validation).</param>
    public ListItemService(IListItemRepository listItemRepository, IListRepository listRepository)
    {
        _listItemRepository = listItemRepository ?? throw new ArgumentNullException(nameof(listItemRepository));
        _listRepository = listRepository ?? throw new ArgumentNullException(nameof(listRepository));
    }

    /// <summary>
    /// Gets a todo item by its unique identifier.
    /// </summary>
    public async Task<TodoItemDto?> GetByIdAsync(Guid id)
    {
        var entity = await _listItemRepository.GetByIdAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    /// <summary>
    /// Gets all todo items for a specific list.
    /// </summary>
    public async Task<IEnumerable<TodoItemDto>> GetByListIdAsync(Guid listId)
    {
        var entities = await _listItemRepository.GetByListIdAsync(listId);
        return entities.Select(MapToDto);
    }

    /// <summary>
    /// Creates a new todo item.
    /// </summary>
    public async Task<TodoItemDto> CreateAsync(Guid listId, CreateListItemRequest request)
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
        return MapToDto(createdEntity);
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    public async Task<TodoItemDto?> UpdateAsync(Guid id, CreateListItemRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var entity = await _listItemRepository.GetByIdAsync(id);
        if (entity == null)
            return null;

        // Update entity properties
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.DueDate = request.DueDate;
        entity.UpdatedDate = DateTime.UtcNow;

        var updatedEntity = await _listItemRepository.UpdateAsync(entity);
        return MapToDto(updatedEntity);
    }

    /// <summary>
    /// Deletes a todo item.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _listItemRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Marks a todo item as completed or not completed.
    /// </summary>
    public async Task<TodoItemDto?> MarkCompleteAsync(Guid id, bool isCompleted)
    {
        var entity = await _listItemRepository.MarkCompleteAsync(id, isCompleted);
        return entity == null ? null : MapToDto(entity);
    }

    /// <summary>
    /// Reorders todo items within a list by updating their Order values.
    /// </summary>
    public async Task<bool> ReorderItemsAsync(Guid listId, ReorderItemsRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.ItemOrders == null || request.ItemOrders.Count == 0)
            return true; // Nothing to reorder

        // Validate list exists
        var list = await _listRepository.GetByIdAsync(listId, includeItems: false);
        if (list == null)
            throw new InvalidOperationException($"Todo list with ID {listId} not found.");

        // Call repository method to perform reordering
        return await _listItemRepository.ReorderItemsAsync(listId, request.ItemOrders);
    }

    /// <summary>
    /// Maps a TodoItem entity to a TodoItemDto.
    /// </summary>
    private TodoItemDto MapToDto(TodoItem entity)
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
            DueDate = entity.DueDate,
            Order = entity.Order
        };
    }
}
