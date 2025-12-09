using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Api.Models.Responses;
using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Services.Services;

namespace TodoListAPI.Api.Controllers;

public class ListController : ControllerBase
{
    private IListService _listService;

    public ListController(IListService listService)
    {
        _listService = listService;
    }
    
    // get lists by user id
    // GET /api/lists  (lists for current user)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListResponse>>> GetListsForCurrentUser()
    {
        // TODO (Task #20): Extract userId from JWT claims when authentication is implemented
        // For now, use placeholder userId as specified in Backend-Todo-List.md Task #10
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        
        // TODO (Task #9): Service implementation will be completed in Task #9
        var lists = await _listService.GetListsForUserAsync(userId);
        
        // TODO (Task #6): Map DTOs to Response models when Response models are completed
        // For now, return DTOs directly (will be mapped in Task #6)
        return Ok(lists.Select(MapToResponse));
    }

    // GET /api/lists/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoListResponse>> GetListById(Guid id)
    {
        // TODO (Task #9): Service implementation will be completed in Task #9
        var list = await _listService.GetListByIdAsync(id);
        if (list == null) return NotFound();
        
        // TODO (Task #6): Map DTO to Response model when Response models are completed
        return Ok(MapToResponse(list));
    }

    // POST /api/lists
    [HttpPost]
    public async Task<ActionResult<TodoListResponse>> CreateList(CreateListRequest request)
    {
        // TODO (Task #20): Extract userId from JWT claims when authentication is implemented
        // For now, use placeholder userId as specified in Backend-Todo-List.md Task #10
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        
        // TODO (Task #9): Service implementation will be completed in Task #9
        var created = await _listService.CreateListAsync(userId, request);
        var response = MapToResponse(created);
        
        // TODO (Task #6): Use response.Id when Response models are completed
        // For now, use DTO's Id property
        return CreatedAtAction(
            nameof(GetListById),
            new { id = created.Id },
            response);
    }

    // PUT /api/lists/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateList(Guid id, UpdateListRequest request)
    {
        // TODO (Task #9): Service implementation will be completed in Task #9
        var success = await _listService.UpdateListAsync(id, request);
        if (!success) return NotFound();
        return NoContent();
    }

    // DELETE /api/lists/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteList(Guid id)
    {
        // TODO (Task #9): Service implementation will be completed in Task #9
        var success = await _listService.DeleteListAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    // POST /api/lists/{listId}/items
    [HttpPost("{listId:guid}/items")]
    public async Task<ActionResult<TodoItemResponse>> AddItemToList(
        Guid listId,
        CreateListItemRequest request)
    {
        // TODO (Task #9): Service implementation will be completed in Task #9
        var created = await _listService.AddItemAsync(listId, request);
        var response = MapToItemResponse(created);
        
        // TODO (Task #6): Use response.Id when Response models are completed
        // For now, use DTO's Id property
        return CreatedAtAction(
            nameof(GetListItemById),
            new { listId, itemId = created.Id },
            response);
    }

    // DELETE /api/lists/{listId}/items/{itemId}
    [HttpDelete("{listId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItemFromList(Guid listId, Guid itemId)
    {
        // TODO (Task #9): Service implementation will be completed in Task #9
        var success = await _listService.RemoveItemAsync(listId, itemId);
        if (!success) return NotFound();
        return NoContent();
    }

    // (Optional) GET single item
    [HttpGet("{listId:guid}/items/{itemId:guid}")]
    public async Task<ActionResult<TodoItemResponse>> GetListItemById(Guid listId, Guid itemId)
    {
        // TODO (Task #9): Add GetItemByIdAsync method to IListService and implement
        // For now, this endpoint is not fully implemented
        throw new NotImplementedException("To be implemented in Task #9");
    }

    /// <summary>
    /// Maps a TodoListDto to a TodoListResponse.
    /// Includes mapping of nested TodoItems if present.
    /// </summary>
    private TodoListResponse MapToResponse(TodoListDto dto)
    {
        return new TodoListResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            CreatedDate = dto.CreatedDate,
            UpdatedDate = dto.UpdatedDate,
            Items = dto.Items?.Select(MapToItemResponse).ToList() ?? new List<TodoItemResponse>()
        };
    }

    /// <summary>
    /// Maps a TodoItemDto to a TodoItemResponse.
    /// </summary>
    private TodoItemResponse MapToItemResponse(TodoItemDto dto)
    {
        return new TodoItemResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
            CreatedDate = dto.CreatedDate,
            UpdatedDate = dto.UpdatedDate,
            DueDate = dto.DueDate
        };
    }
}
