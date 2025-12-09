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
        // var userId = ... from auth/claims
        var lists = await _listService.GetListsForUserAsync(userId);
        // return Ok(lists.Select(MapToResponse));
        throw new NotImplementedException();
    }

    // GET /api/lists/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoListResponse>> GetListById(Guid id)
    {
        var list = await _listService.GetListByIdAsync(id);
        // if (list == null) return NotFound();
        // return Ok(MapToResponse(list));
        throw new NotImplementedException();
    }

    // POST /api/lists
    [HttpPost]
    public async Task<ActionResult<TodoListResponse>> CreateList(CreateListRequest request)
    {
        var userId = 0;//... from auth/claims
        var created = await _listService.CreateListAsync(userId, request);
        // var response = MapToResponse(created);
        //
        // return CreatedAtAction(
        //     nameof(GetListById),
        //     new { id = response.Id },
        //     response);
        throw new NotImplementedException();
    }

    // PUT /api/lists/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateList(Guid id, UpdateListRequest request)
    {
        var success = await _listService.UpdateListAsync(id, request);
        // if (!success) return NotFound();
        // return NoContent();
        throw new NotImplementedException();
    }

    // DELETE /api/lists/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteList(Guid id)
    {
        var success = await _listService.DeleteListAsync(id);
        // if (!success) return NotFound();
        // return NoContent();
        throw new NotImplementedException();
    }

    // POST /api/lists/{listId}/items
    [HttpPost("{listId:guid}/items")]
    public async Task<ActionResult<TodoItemResponse>> AddItemToList(
        Guid listId,
        CreateListItemRequest request)
    {
        var created = await _listService.AddItemAsync(listId, request);
        // var response = MapToItemResponse(created);
        //
        // return CreatedAtAction(
        //     nameof(GetListItemById),
        //     new { listId, itemId = response.Id },
        //     response);
        throw new NotImplementedException();
    }

    // DELETE /api/lists/{listId}/items/{itemId}
    [HttpDelete("{listId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItemFromList(Guid listId, Guid itemId)
    {
        var success = await _listService.RemoveItmemAsync(listId, itemId);
        // if (!success) return NotFound();
        // return NoContent();
        throw new NotImplementedException();
    }

    // (Optional) GET single item
    [HttpGet("{listId:guid}/items/{itemId:guid}")]
    public async Task<ActionResult<TodoItemResponse>> GetListItemById(Guid listId, Guid itemId)
    {
        // ...
        throw new NotImplementedException();
    }
}
