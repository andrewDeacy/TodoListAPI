using Moq;
using TodoListAPI.Core.DTOs;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Repository.Models;
using TodoListAPI.Repository.Repositories;
using TodoListAPI.Services.Services;

namespace TodoListAPI.Tests.Services;

[TestClass]
public class ListServiceTests
{
    private Mock<IListRepository> _mockListRepository = null!;
    private Mock<IListItemRepository> _mockListItemRepository = null!;
    private ListService _listService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockListRepository = new Mock<IListRepository>();
        _mockListItemRepository = new Mock<IListItemRepository>();
        _listService = new ListService(_mockListRepository.Object, _mockListItemRepository.Object);
    }

    #region GetListsForUserAsync Tests

    [TestMethod]
    public async Task GetListsForUserAsync_ReturnsEmptyList_WhenNoListsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockListRepository.Setup(r => r.GetAllForUserAsync(userId, false))
            .ReturnsAsync(new List<TodoList>());

        // Act
        var result = await _listService.GetListsForUserAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
        _mockListRepository.Verify(r => r.GetAllForUserAsync(userId, false), Times.Once);
    }

    [TestMethod]
    public async Task GetListsForUserAsync_ReturnsLists_WhenListsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var lists = new List<TodoList>
        {
            new TodoList
            {
                Id = Guid.NewGuid(),
                Name = "Test List 1",
                Description = "Description 1",
                UserId = userId,
                IsCompleted = false,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            },
            new TodoList
            {
                Id = Guid.NewGuid(),
                Name = "Test List 2",
                Description = "Description 2",
                UserId = userId,
                IsCompleted = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }
        };

        _mockListRepository.Setup(r => r.GetAllForUserAsync(userId, false))
            .ReturnsAsync(lists);

        // Act
        var result = await _listService.GetListsForUserAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        var resultList = result.ToList();
        Assert.AreEqual(lists[0].Name, resultList[0].Name);
        Assert.AreEqual(lists[1].Name, resultList[1].Name);
    }

    #endregion

    #region GetListByIdAsync Tests

    [TestMethod]
    public async Task GetListByIdAsync_ReturnsNull_WhenListNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        _mockListRepository.Setup(r => r.GetByIdAsync(listId, true))
            .ReturnsAsync((TodoList?)null);

        // Act
        var result = await _listService.GetListByIdAsync(listId);

        // Assert
        Assert.IsNull(result);
        _mockListRepository.Verify(r => r.GetByIdAsync(listId, true), Times.Once);
    }

    [TestMethod]
    public async Task GetListByIdAsync_ReturnsList_WhenListExists()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var list = new TodoList
        {
            Id = listId,
            Name = "Test List",
            Description = "Test Description",
            UserId = userId,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            TodoItems = new List<TodoItem>()
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, true))
            .ReturnsAsync(list);

        // Act
        var result = await _listService.GetListByIdAsync(listId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(listId, result.Id);
        Assert.AreEqual("Test List", result.Name);
        Assert.AreEqual("Test Description", result.Description);
    }

    #endregion

    #region CreateListAsync Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task CreateListAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _listService.CreateListAsync(userId, null!);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task CreateListAsync_CreatesList_WhenRequestIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateListRequest
        {
            Name = "New List",
            Description = "New Description"
        };

        var createdList = new TodoList
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = userId,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _mockListRepository.Setup(r => r.CreateAsync(It.IsAny<TodoList>()))
            .ReturnsAsync(createdList);

        // Act
        var result = await _listService.CreateListAsync(userId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(request.Name, result.Name);
        Assert.AreEqual(request.Description, result.Description);
        Assert.AreEqual(userId, result.UserId);
        _mockListRepository.Verify(r => r.CreateAsync(It.Is<TodoList>(l => 
            l.Name == request.Name && 
            l.Description == request.Description && 
            l.UserId == userId)), Times.Once);
    }

    #endregion

    #region UpdateListAsync Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task UpdateListAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var listId = Guid.NewGuid();

        // Act
        await _listService.UpdateListAsync(listId, null!);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task UpdateListAsync_ReturnsFalse_WhenListNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var request = new UpdateListRequest
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync((TodoList?)null);

        // Act
        var result = await _listService.UpdateListAsync(listId, request);

        // Assert
        Assert.IsFalse(result);
        _mockListRepository.Verify(r => r.GetByIdAsync(listId, false), Times.Once);
        _mockListRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoList>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateListAsync_UpdatesList_WhenListExists()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingList = new TodoList
        {
            Id = listId,
            Name = "Old Name",
            Description = "Old Description",
            UserId = userId,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow.AddDays(-1),
            UpdatedDate = DateTime.UtcNow.AddDays(-1)
        };

        var request = new UpdateListRequest
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        _mockListRepository.Setup(r => r.GetByIdAsync(listId, false))
            .ReturnsAsync(existingList);
        _mockListRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoList>()))
            .ReturnsAsync(existingList);

        // Act
        var result = await _listService.UpdateListAsync(listId, request);

        // Assert
        Assert.IsTrue(result);
        _mockListRepository.Verify(r => r.GetByIdAsync(listId, false), Times.Once);
        _mockListRepository.Verify(r => r.UpdateAsync(It.Is<TodoList>(l => 
            l.Name == request.Name && 
            l.Description == request.Description)), Times.Once);
    }

    #endregion

    #region DeleteListAsync Tests

    [TestMethod]
    public async Task DeleteListAsync_ReturnsFalse_WhenListNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        _mockListRepository.Setup(r => r.DeleteAsync(listId))
            .ReturnsAsync(false);

        // Act
        var result = await _listService.DeleteListAsync(listId);

        // Assert
        Assert.IsFalse(result);
        _mockListRepository.Verify(r => r.DeleteAsync(listId), Times.Once);
    }

    [TestMethod]
    public async Task DeleteListAsync_ReturnsTrue_WhenListDeleted()
    {
        // Arrange
        var listId = Guid.NewGuid();
        _mockListRepository.Setup(r => r.DeleteAsync(listId))
            .ReturnsAsync(true);

        // Act
        var result = await _listService.DeleteListAsync(listId);

        // Assert
        Assert.IsTrue(result);
        _mockListRepository.Verify(r => r.DeleteAsync(listId), Times.Once);
    }

    #endregion

    #region AddItemAsync Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task AddItemAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var listId = Guid.NewGuid();

        // Act
        await _listService.AddItemAsync(listId, null!);

        // Assert - Exception expected
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task AddItemAsync_ThrowsInvalidOperationException_WhenListNotFound()
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
        await _listService.AddItemAsync(listId, request);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task AddItemAsync_CreatesItem_WhenListExists()
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
        var result = await _listService.AddItemAsync(listId, request);

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

    #region RemoveItemAsync Tests

    [TestMethod]
    public async Task RemoveItemAsync_ReturnsFalse_WhenItemNotFound()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _mockListItemRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _listService.RemoveItemAsync(listId, itemId);

        // Assert
        Assert.IsFalse(result);
        _mockListItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockListItemRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task RemoveItemAsync_ThrowsInvalidOperationException_WhenItemDoesNotBelongToList()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var differentListId = Guid.NewGuid();

        var item = new TodoItem
        {
            Id = itemId,
            ListId = differentListId,
            Title = "Test Item",
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _mockListItemRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync(item);

        // Act
        await _listService.RemoveItemAsync(listId, itemId);

        // Assert - Exception expected
    }

    [TestMethod]
    public async Task RemoveItemAsync_ReturnsTrue_WhenItemRemoved()
    {
        // Arrange
        var listId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var item = new TodoItem
        {
            Id = itemId,
            ListId = listId,
            Title = "Test Item",
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _mockListItemRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync(item);
        _mockListItemRepository.Setup(r => r.DeleteAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _listService.RemoveItemAsync(listId, itemId);

        // Assert
        Assert.IsTrue(result);
        _mockListItemRepository.Verify(r => r.GetByIdAsync(itemId), Times.Once);
        _mockListItemRepository.Verify(r => r.DeleteAsync(itemId), Times.Once);
    }

    #endregion
}

