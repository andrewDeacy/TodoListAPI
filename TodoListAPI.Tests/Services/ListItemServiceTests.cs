using Moq;
using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Repository.Models;
using TodoListAPI.Repository.Repositories;
using TodoListAPI.Services.Services;

namespace TodoListAPI.Tests.Services;

[TestClass]
public class ListItemServiceTests
{
    private Mock<IListItemRepository> _mockListItemRepository = null!;
    private Mock<IListRepository> _mockListRepository = null!;
    private ListItemService _listItemService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockListItemRepository = new Mock<IListItemRepository>();
        _mockListRepository = new Mock<IListRepository>();
        _listItemService = new ListItemService(_mockListItemRepository.Object, _mockListRepository.Object);
    }

    #region GetByIdAsync Tests

    [TestMethod]
    public async Task GetByIdAsync_ReturnsNull_WhenItemNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockListItemRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _listItemService.GetByIdAsync(itemId);

        // Assert
        Assert.IsNull(result);
        _mockListItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsItem_WhenItemExists()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var item = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = "Test Item",
            Description = "Test Description",
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _mockListItemRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync(item);

        // Act
        var result = await _listItemService.GetByIdAsync(itemId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(itemId, result.Id);
        Assert.AreEqual("Test Item", result.Title);
        Assert.AreEqual("Test Description", result.Description);
        Assert.AreEqual(listId, result.ListId);
    }

    #endregion

    #region GetByListIdAsync Tests

    [TestMethod]
    public async Task GetByListIdAsync_ReturnsEmptyList_WhenNoItemsExist()
    {
        // Arrange
        var listId = Guid.NewGuid();
        _mockListItemRepository.Setup(r => r.GetByListIdAsync(listId))
            .ReturnsAsync(new List<TodoItem>());

        // Act
        var result = await _listItemService.GetByListIdAsync(listId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
        _mockListItemRepository.Verify(r => r.GetByListIdAsync(listId), Times.Once);
    }

    [TestMethod]
    public async Task GetByListIdAsync_ReturnsItems_WhenItemsExist()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var items = new List<TodoItem>
        {
            new TodoItem
            {
                Id = Guid.NewGuid(),
                ListId = listId,
                Title = "Item 1",
                IsCompleted = false,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            },
            new TodoItem
            {
                Id = Guid.NewGuid(),
                ListId = listId,
                Title = "Item 2",
                IsCompleted = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }
        };

        _mockListItemRepository.Setup(r => r.GetByListIdAsync(listId))
            .ReturnsAsync(items);

        // Act
        var result = await _listItemService.GetByListIdAsync(listId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        var resultList = result.ToList();
        Assert.AreEqual(items[0].Title, resultList[0].Title);
        Assert.AreEqual(items[1].Title, resultList[1].Title);
    }

    #endregion

    #region CreateAsync Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task CreateAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var listId = Guid.NewGuid();

        // Act
        await _listItemService.CreateAsync(listId, null!);

        // Assert - Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task CreateAsync_ThrowsInvalidOperationException_WhenListNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var request = new CreateListItemRequest
        {
            Title = "New Item",
            Description = "Item Description"
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync((TodoList?)null);

        // Act
        await _listItemService.CreateAsync(listId, request);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task CreateAsync_CreatesItem_WhenListExists()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var list = new TodoList
        {
            Id = listId,
            Name = "Test List",
            UserId = userId,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var request = new CreateListItemRequest
        {
            Title = "New Item",
            Description = "Item Description",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createdItem = new TodoItem
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

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync(list);
        _mockListItemRepository.Setup(r => r.CreateAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync(createdItem);

        // Act
        var result = await _listItemService.CreateAsync(listId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(request.Title, result.Title);
        Assert.AreEqual(request.Description, result.Description);
        Assert.AreEqual(request.DueDate, result.DueDate);
        _mockListRepository.Verify(r => r.GetByIdAsync(listId, false), Times.Once);
        _mockListItemRepository.Verify(r => r.CreateAsync(It.Is<TodoItem>(i => 
            i.ListId == listId && 
            i.Title == request.Title)), Times.Once);
    }

    #endregion

    #region UpdateAsync Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task UpdateAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        // Act
        await _listItemService.UpdateAsync(itemId, null!);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task UpdateAsync_ReturnsNull_WhenItemNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var request = new CreateListItemRequest
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        _mockListItemRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _listItemService.UpdateAsync(itemId, request);

        // Assert
        Assert.IsNull(result);
        _mockListItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockListItemRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesItem_WhenItemExists()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var existingItem = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = "Old Title",
            Description = "Old Description",
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow.AddDays(-1),
            UpdatedDate = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        var request = new CreateListItemRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        var updatedItem = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            IsCompleted = existingItem.IsCompleted,
            CreatedDate = existingItem.CreatedDate,
            UpdatedDate = DateTime.UtcNow
        };

        _mockListItemRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockListItemRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _listItemService.UpdateAsync(itemId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(request.Title, result.Title);
        Assert.AreEqual(request.Description, result.Description);
        Assert.AreEqual(request.DueDate, result.DueDate);
        _mockListItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockListItemRepository.Verify(r => r.UpdateAsync(It.Is<TodoItem>(i => 
            i.Title == request.Title && 
            i.Description == request.Description)), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [TestMethod]
    public async Task DeleteAsync_ReturnsFalse_WhenItemNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockListItemRepository.Setup(r => r.DeleteAsync(itemId))
            .ReturnsAsync(false);

        // Act
        var result = await _listItemService.DeleteAsync(itemId);

        // Assert
        Assert.IsFalse(result);
        _mockListItemRepository.Verify(r => r.DeleteAsync(itemId), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_ReturnsTrue_WhenItemDeleted()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockListItemRepository.Setup(r => r.DeleteAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _listItemService.DeleteAsync(itemId);

        // Assert
        Assert.IsTrue(result);
        _mockListItemRepository.Verify(r => r.DeleteAsync(itemId), Times.Once);
    }

    #endregion

    #region MarkCompleteAsync Tests

    [TestMethod]
    public async Task MarkCompleteAsync_ReturnsNull_WhenItemNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockListItemRepository.Setup(r => r.MarkCompleteAsync(itemId, true))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _listItemService.MarkCompleteAsync(itemId, true);

        // Assert
        Assert.IsNull(result);
        _mockListItemRepository.Verify(r => r.MarkCompleteAsync(itemId, true), Times.Once);
    }

    [TestMethod]
    public async Task MarkCompleteAsync_MarksItemComplete_WhenItemExists()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var item = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = "Test Item",
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var completedItem = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = "Test Item",
            IsCompleted = true,
            CreatedDate = item.CreatedDate,
            UpdatedDate = DateTime.UtcNow
        };

        _mockListItemRepository.Setup(r => r.MarkCompleteAsync(itemId, true))
            .ReturnsAsync(completedItem);

        // Act
        var result = await _listItemService.MarkCompleteAsync(itemId, true);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsCompleted);
        _mockListItemRepository.Verify(r => r.MarkCompleteAsync(itemId, true), Times.Once);
    }

    [TestMethod]
    public async Task MarkCompleteAsync_MarksItemIncomplete_WhenItemExists()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var item = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = "Test Item",
            IsCompleted = true,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var incompleteItem = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = "Test Item",
            IsCompleted = false,
            CreatedDate = item.CreatedDate,
            UpdatedDate = DateTime.UtcNow
        };

        _mockListItemRepository.Setup(r => r.MarkCompleteAsync(itemId, false))
            .ReturnsAsync(incompleteItem);

        // Act
        var result = await _listItemService.MarkCompleteAsync(itemId, false);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsCompleted);
        _mockListItemRepository.Verify(r => r.MarkCompleteAsync(itemId, false), Times.Once);
    }

    #endregion

    #region ReorderItemsAsync Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task ReorderItemsAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var listId = Guid.NewGuid();

        // Act
        await _listItemService.ReorderItemsAsync(listId, null!);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task ReorderItemsAsync_ReturnsTrue_WhenRequestIsEmpty()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var request = new ReorderItemsRequest
        {
            ItemOrders = new Dictionary<Guid, int>()
        };

        // Act
        var result = await _listItemService.ReorderItemsAsync(listId, request);

        // Assert
        Assert.IsTrue(result);
        // When request is empty, service returns early without checking list or calling repository
        _mockListRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
        _mockListItemRepository.Verify(r => r.ReorderItemsAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<Guid, int>>()), Times.Never);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task ReorderItemsAsync_ThrowsInvalidOperationException_WhenListNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var request = new ReorderItemsRequest
        {
            ItemOrders = new Dictionary<Guid, int>
            {
                { Guid.NewGuid(), 0 }
            }
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync((TodoList?)null);

        // Act
        await _listItemService.ReorderItemsAsync(listId, request);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task ReorderItemsAsync_ReturnsTrue_WhenReorderingSucceeds()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();
        var itemId3 = Guid.NewGuid();

        var list = new TodoList
        {
            Id = listId,
            Name = "Test List",
            UserId = Guid.NewGuid(),
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var request = new ReorderItemsRequest
        {
            ItemOrders = new Dictionary<Guid, int>
            {
                { itemId1, 2 },
                { itemId2, 0 },
                { itemId3, 1 }
            }
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync(list);
        _mockListItemRepository.Setup(r => r.ReorderItemsAsync(listId, request.ItemOrders))
            .ReturnsAsync(true);

        // Act
        var result = await _listItemService.ReorderItemsAsync(listId, request);

        // Assert
        Assert.IsTrue(result);
        _mockListRepository.Verify(r => r.GetByIdAsync(listId, false), Times.Once);
        _mockListItemRepository.Verify(r => r.ReorderItemsAsync(listId, request.ItemOrders), Times.Once);
    }

    [TestMethod]
    public async Task ReorderItemsAsync_ReturnsFalse_WhenRepositoryValidationFails()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();

        var list = new TodoList
        {
            Id = listId,
            Name = "Test List",
            UserId = Guid.NewGuid(),
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var request = new ReorderItemsRequest
        {
            ItemOrders = new Dictionary<Guid, int>
            {
                { itemId1, 0 },
                { itemId2, 1 }
            }
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync(list);
        _mockListItemRepository.Setup(r => r.ReorderItemsAsync(listId, request.ItemOrders))
            .ReturnsAsync(false); // Repository validation fails (items don't belong to list)

        // Act
        var result = await _listItemService.ReorderItemsAsync(listId, request);

        // Assert
        Assert.IsFalse(result);
        _mockListRepository.Verify(r => r.GetByIdAsync(listId, false), Times.Once);
        _mockListItemRepository.Verify(r => r.ReorderItemsAsync(listId, request.ItemOrders), Times.Once);
    }

    [TestMethod]
    public async Task ReorderItemsAsync_HandlesSingleItemReorder()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var list = new TodoList
        {
            Id = listId,
            Name = "Test List",
            UserId = Guid.NewGuid(),
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var request = new ReorderItemsRequest
        {
            ItemOrders = new Dictionary<Guid, int>
            {
                { itemId, 0 }
            }
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync(list);
        _mockListItemRepository.Setup(r => r.ReorderItemsAsync(listId, request.ItemOrders))
            .ReturnsAsync(true);

        // Act
        var result = await _listItemService.ReorderItemsAsync(listId, request);

        // Assert
        Assert.IsTrue(result);
        _mockListItemRepository.Verify(r => r.ReorderItemsAsync(listId, request.ItemOrders), Times.Once);
    }

    #endregion
}

