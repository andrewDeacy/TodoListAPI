using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Core.Models.Requests;

/// <summary>
/// Request model for reordering todo items within a list.
/// Maps item IDs to their new order positions.
/// </summary>
public class ReorderItemsRequest
{
    /// <summary>
    /// Dictionary mapping item IDs to their new order positions.
    /// Key: Item ID (Guid)
    /// Value: New order position (int)
    /// Lower order values appear first in the list.
    /// </summary>
    [Required(ErrorMessage = "Item orders are required.")]
    public Dictionary<Guid, int> ItemOrders { get; set; } = new Dictionary<Guid, int>();
}

