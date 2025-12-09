using TodoListAPI.Repository.Models;

namespace TodoListAPI.Repository.Repositories;

/// <summary>
/// Repository interface for TodoList operations.
/// Provides data access methods for TodoList entities.
/// </summary>
public interface IListRepository
{
    /// <summary>
    /// Gets a todo list by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list.</param>
    /// <param name="includeItems">Whether to include TodoItems in the result.</param>
    /// <returns>The TodoList entity if found, null otherwise.</returns>
    Task<TodoList?> GetByIdAsync(Guid id, bool includeItems = false);

    /// <summary>
    /// Gets all todo lists for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="includeItems">Whether to include TodoItems in the results.</param>
    /// <returns>A collection of TodoList entities for the user.</returns>
    Task<IEnumerable<TodoList>> GetAllForUserAsync(Guid userId, bool includeItems = false);

    /// <summary>
    /// Creates a new todo list.
    /// </summary>
    /// <param name="todoList">The TodoList entity to create.</param>
    /// <returns>The created TodoList entity with generated Id and timestamps.</returns>
    Task<TodoList> CreateAsync(TodoList todoList);

    /// <summary>
    /// Updates an existing todo list.
    /// </summary>
    /// <param name="todoList">The TodoList entity with updated properties.</param>
    /// <returns>The updated TodoList entity.</returns>
    Task<TodoList> UpdateAsync(TodoList todoList);

    /// <summary>
    /// Deletes a todo list by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the todo list to delete.</param>
    /// <returns>True if the list was deleted, false if it was not found.</returns>
    Task<bool> DeleteAsync(Guid id);
}

