using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Api.Models.Requests;
using TodoListAPI.Api.Models.Responses;
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
        
        // TODO (Task #4, #9): Service will return DTOs in Task #4, implementation in Task #9
        // For now, service returns object - will be properly typed in Task #4
        var lists = await _listService.GetListsForUserAsync(userId);
        return Ok(lists.Cast<TodoListResponse>());
    }

    // GET /api/lists/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoListResponse>> GetListById(Guid id)
    {
        // TODO (Task #4, #9): Service will return DTOs in Task #4, implementation in Task #9
        // For now, service returns object? - will be properly typed in Task #4
        var list = await _listService.GetListByIdAsync(id);
        if (list == null) return NotFound();
        return Ok((TodoListResponse)list);
    }

    // POST /api/lists
    [HttpPost]
    public async Task<ActionResult<TodoListResponse>> CreateList(CreateListRequest request)
    {
        // TODO (Task #20): Extract userId from JWT claims when authentication is implemented
        // For now, use placeholder userId as specified in Backend-Todo-List.md Task #10
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        
        // TODO (Task #4, #9): Service will accept/return DTOs in Task #4, implementation in Task #9
        // For now, service uses object - will be properly typed in Task #4
        var created = await _listService.CreateListAsync(userId, request);
        var response = (TodoListResponse)created;
        
        // TODO (Task #6): TodoListResponse.Id will be added in Task #6
        // For now, use Guid.Empty as placeholder
        return CreatedAtAction(
            nameof(GetListById),
            new { id = Guid.Empty },
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
        // TODO (Task #4, #9): Service will accept/return DTOs in Task #4, implementation in Task #9
        // For now, service uses object - will be properly typed in Task #4
        var created = await _listService.AddItemAsync(listId, request);
        var response = (TodoItemResponse)created;
        
        // TODO (Task #6): TodoItemResponse.Id will be added in Task #6
        // For now, use Guid.Empty as placeholder
        return CreatedAtAction(
            nameof(GetListItemById),
            new { listId, itemId = Guid.Empty },
            response);
    }

    // DELETE /api/lists/{listId}/items/{itemId}
    [HttpDelete("{listId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItemFromList(Guid listId, Guid itemId)
    {
        // TODO (Task #9): Service implementation will be completed in Task #9
        // TODO (Task #19): Fix typo in service method name "RemoveItmemAsync" → "RemoveItemAsync"
        var success = await _listService.RemoveItmemAsync(listId, itemId);
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
}
