using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Api.Extensions;
using TodoListAPI.Api.Models.Responses;
using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Services.Services;

namespace TodoListAPI.Api.Controllers;

/// <summary>
/// Controller for managing TodoLists.
/// Provides endpoints for CRUD operations on todo lists.
/// All endpoints require authentication via JWT token.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ListController : ControllerBase
{
    private readonly IListService _listService;

    /// <summary>
    /// Initializes a new instance of the ListController class.
    /// </summary>
    /// <param name="listService">Service for TodoList operations.</param>
    public ListController(IListService listService)
    {
        _listService = listService ?? throw new ArgumentNullException(nameof(listService));
    }
    
    /// <summary>
    /// Gets all todo lists for the current user.
    /// </summary>
    /// <returns>A collection of todo lists.</returns>
    /// <response code="200">Returns the list of todo lists.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoListResponse>>> GetListsForCurrentUser()
    {
        var userId = User.GetUserIdRequired();
        
        var lists = await _listService.GetListsForUserAsync(userId);
        return Ok(lists.Select(MapToResponse));
    }

    /// <summary>
    /// Gets a specific todo list by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list.</param>
    /// <returns>The todo list if found.</returns>
    /// <response code="200">Returns the todo list.</response>
    /// <response code="404">If the todo list is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoListResponse>> GetListById(Guid id)
    {
        var list = await _listService.GetListByIdAsync(id);
        if (list == null) 
            return NotFound();
        
        return Ok(MapToResponse(list));
    }

    /// <summary>
    /// Creates a new todo list for the current user.
    /// </summary>
    /// <param name="request">The request containing list data.</param>
    /// <returns>The created todo list.</returns>
    /// <response code="201">Returns the newly created todo list.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TodoListResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoListResponse>> CreateList(CreateListRequest request)
    {
        // Validation is automatically handled by [ApiController] attribute
        // Invalid requests return ProblemDetails (RFC 7807) before this method is called

        var userId = User.GetUserIdRequired();
        
        try
        {
            var created = await _listService.CreateListAsync(userId, request);
            var response = MapToResponse(created);
            
            return CreatedAtAction(
                nameof(GetListById),
                new { id = created.Id },
                response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing todo list.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list to update.</param>
    /// <param name="request">The request containing updated list data.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the todo list was updated successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the todo list is not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateList(Guid id, UpdateListRequest request)
    {
        // Validation is automatically handled by [ApiController] attribute
        // Invalid requests return ProblemDetails (RFC 7807) before this method is called

        var success = await _listService.UpdateListAsync(id, request);
        if (!success) 
            return NotFound();
        
        return NoContent();
    }

    /// <summary>
    /// Deletes a todo list.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the todo list was deleted successfully.</response>
    /// <response code="404">If the todo list is not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteList(Guid id)
    {
        var success = await _listService.DeleteListAsync(id);
        if (!success) 
            return NotFound();
        
        return NoContent();
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
            Items = dto.Items?.Select(item => new TodoItemResponse
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted,
                CreatedDate = item.CreatedDate,
                UpdatedDate = item.UpdatedDate,
                DueDate = item.DueDate,
                Order = item.Order
            }).ToList() ?? new List<TodoItemResponse>()
        };
    }
}
