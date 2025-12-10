using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Api.Models.Responses;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Services.Services;

namespace TodoListAPI.Api.Controllers;

/// <summary>
/// Controller for managing TodoItems.
/// Provides endpoints for CRUD operations on todo items.
/// All endpoints require authentication via JWT token.
/// </summary>
[ApiController]
[Route("api/lists/{listId:guid}/items")]
[Authorize]
public class ListItemController : ControllerBase
{
    private readonly IListItemService _listItemService;

    /// <summary>
    /// Initializes a new instance of the ListItemController class.
    /// </summary>
    /// <param name="listItemService">Service for TodoItem operations.</param>
    public ListItemController(IListItemService listItemService)
    {
        _listItemService = listItemService ?? throw new ArgumentNullException(nameof(listItemService));
    }

    /// <summary>
    /// Gets all todo items for a specific list.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <returns>A collection of todo items.</returns>
    /// <response code="200">Returns the list of todo items.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemResponse>>> GetItemsByListId(Guid listId)
    {
        var items = await _listItemService.GetByListIdAsync(listId);
        return Ok(items.Select(MapToResponse));
    }

    /// <summary>
    /// Gets a specific todo item by its ID.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="itemId">The unique identifier of the todo item.</param>
    /// <returns>The todo item if found.</returns>
    /// <response code="200">Returns the todo item.</response>
    /// <response code="400">If the item does not belong to the list.</response>
    /// <response code="404">If the todo item is not found.</response>
    [HttpGet("{itemId:guid}")]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemResponse>> GetItemById(Guid listId, Guid itemId)
    {
        var item = await _listItemService.GetByIdAsync(itemId);
        if (item == null)
            return NotFound();
        
        // Validate item belongs to list
        if (item.ListId != listId)
            return BadRequest(new { error = $"Item {itemId} does not belong to list {listId}." });
        
        return Ok(MapToResponse(item));
    }

    /// <summary>
    /// Creates a new todo item in a list.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="request">The request containing item data.</param>
    /// <returns>The created todo item.</returns>
    /// <response code="201">Returns the newly created todo item.</response>
    /// <response code="400">If the request is invalid or the list does not exist.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemResponse>> CreateItem(Guid listId, CreateListItemRequest request)
    {
        // Validation is automatically handled by [ApiController] attribute
        // Invalid requests return ProblemDetails (RFC 7807) before this method is called

        try
        {
            var created = await _listItemService.CreateAsync(listId, request);
            var response = MapToResponse(created);
            
            return CreatedAtAction(
                nameof(GetItemById),
                new { listId, itemId = created.Id },
                response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing todo item.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="itemId">The unique identifier of the todo item to update.</param>
    /// <param name="request">The request containing updated item data.</param>
    /// <returns>The updated todo item if successful.</returns>
    /// <response code="200">Returns the updated todo item.</response>
    /// <response code="400">If the request is invalid or the item does not belong to the list.</response>
    /// <response code="404">If the todo item is not found.</response>
    [HttpPut("{itemId:guid}")]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemResponse>> UpdateItem(
        Guid listId, 
        Guid itemId, 
        CreateListItemRequest request)
    {
        // Validation is automatically handled by [ApiController] attribute
        // Invalid requests return ProblemDetails (RFC 7807) before this method is called

        // Validate item belongs to list
        var existingItem = await _listItemService.GetByIdAsync(itemId);
        if (existingItem == null)
            return NotFound();
        
        if (existingItem.ListId != listId)
            return BadRequest(new { error = $"Item {itemId} does not belong to list {listId}." });

        var updated = await _listItemService.UpdateAsync(itemId, request);
        if (updated == null)
            return NotFound();
        
        return Ok(MapToResponse(updated));
    }

    /// <summary>
    /// Deletes a todo item.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="itemId">The unique identifier of the todo item to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the item was deleted successfully.</response>
    /// <response code="400">If the item does not belong to the list.</response>
    /// <response code="404">If the item is not found.</response>
    [HttpDelete("{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid listId, Guid itemId)
    {
        // Validate item belongs to list
        var item = await _listItemService.GetByIdAsync(itemId);
        if (item == null)
            return NotFound();
        
        if (item.ListId != listId)
            return BadRequest(new { error = $"Item {itemId} does not belong to list {listId}." });

        var success = await _listItemService.DeleteAsync(itemId);
        if (!success)
            return NotFound();
        
        return NoContent();
    }

    /// <summary>
    /// Marks a todo item as completed or not completed.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="itemId">The unique identifier of the todo item.</param>
    /// <param name="isCompleted">True to mark as completed, false to mark as not completed.</param>
    /// <returns>The updated todo item if successful.</returns>
    /// <response code="200">Returns the updated todo item.</response>
    /// <response code="400">If the item does not belong to the list.</response>
    /// <response code="404">If the todo item is not found.</response>
    [HttpPatch("{itemId:guid}/complete")]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemResponse>> MarkItemComplete(
        Guid listId, 
        Guid itemId, 
        [FromQuery] bool isCompleted = true)
    {
        // Validate item belongs to list
        var existingItem = await _listItemService.GetByIdAsync(itemId);
        if (existingItem == null)
            return NotFound();
        
        if (existingItem.ListId != listId)
            return BadRequest(new { error = $"Item {itemId} does not belong to list {listId}." });

        var updated = await _listItemService.MarkCompleteAsync(itemId, isCompleted);
        if (updated == null)
            return NotFound();
        
        return Ok(MapToResponse(updated));
    }

    /// <summary>
    /// Reorders todo items within a list by updating their Order values.
    /// </summary>
    /// <param name="listId">The unique identifier of the todo list.</param>
    /// <param name="request">The request containing item ID to order position mappings.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the items were reordered successfully.</response>
    /// <response code="400">If the request is invalid, the list does not exist, or items don't belong to the list.</response>
    /// <remarks>
    /// This endpoint allows reordering items within a list. The request contains a dictionary mapping item IDs to their new order positions.
    /// Lower order values appear first in the list. Items can be reordered to any position, and gaps are allowed for efficiency.
    /// 
    /// Example request:
    /// {
    ///   "itemOrders": {
    ///     "item-id-1": 0,
    ///     "item-id-2": 1,
    ///     "item-id-3": 2
    ///   }
    /// }
    /// </remarks>
    [HttpPatch("reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderItems(Guid listId, ReorderItemsRequest request)
    {
        // Validation is automatically handled by [ApiController] attribute
        // Invalid requests return ProblemDetails (RFC 7807) before this method is called

        try
        {
            var success = await _listItemService.ReorderItemsAsync(listId, request);
            if (!success)
            {
                return BadRequest(new { error = "Reordering failed. Ensure all items belong to the specified list." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Maps a TodoItemDto to a TodoItemResponse.
    /// </summary>
    private TodoItemResponse MapToResponse(TodoListAPI.Core.DTOs.TodoItemDto dto)
    {
        return new TodoItemResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
            CreatedDate = dto.CreatedDate,
            UpdatedDate = dto.UpdatedDate,
            DueDate = dto.DueDate,
            Order = dto.Order
        };
    }
}