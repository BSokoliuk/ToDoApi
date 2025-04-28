using Application.DTOs;
using Application.Enums;
using Application.Services;
using Application.Services.Interfaces;
using Application.UoW;
using Domain.Entities;
using Moq;

namespace Application.Tests.Unit;
//unit test for TodoItemService
public class TodoItemServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ITodoItemService _service;

    public TodoItemServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new TodoItemService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenItemsExist_ReturnsAllItems()
    {
        // Arrange
        var items = new List<TodoItem> { new() { Id = 1, Title = "Test", Description = "Test" } };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetAllAsync()).ReturnsAsync(items);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoItemsExist_ReturnsEmpty()
    {
        // Arrange
        var items = new List<TodoItem>();
        _unitOfWorkMock.Setup(u => u.TodoItem.GetAllAsync()).ReturnsAsync(items);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var item = new TodoItem { Id = 1, Title = "Test", Description = "Test" };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync(item);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Id, result?.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetIncomingItemsAsync_WhenItemsExist_ReturnsItems()
    {
        // Arrange
        var items = new List<TodoItem> { new() { Id = 1, Title = "Test", Description = "Test", ExpiryDateTime = DateTime.UtcNow.AddHours(1) } };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetIncomingItemsAsync(It.IsAny<DateTime>())).ReturnsAsync(items);

        // Act
        var result = await _service.GetIncomingItemsAsync(DateTime.UtcNow.AddHours(2));

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetIncomingItemsAsync_WhenNoItemsExist_ReturnsEmpty()
    {
        // Arrange
        var items = new List<TodoItem>();
        _unitOfWorkMock.Setup(u => u.TodoItem.GetIncomingItemsAsync(It.IsAny<DateTime>())).ReturnsAsync(items);

        // Act
        var result = await _service.GetIncomingItemsAsync(DateTime.UtcNow.AddHours(2));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetIncomingTodayAsync_WhenExists_ReturnsItems()
    {
        // Arrange
        var items = new List<TodoItem> { new() { Id = 1, Title = "Test", Description = "Test", ExpiryDateTime = DateTime.UtcNow.AddHours(1) } };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetIncomingItemsAsync(It.IsAny<DateTime>())).ReturnsAsync(items);

        // Act
        var result = await _service.GetIncomingTodayAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetIncomingNextDayAsync_WhenExists_ReturnsItems()
    {
        // Arrange
        var items = new List<TodoItem> { new() { Id = 1, Title = "Test", Description = "Test", ExpiryDateTime = DateTime.UtcNow.AddDays(1) } };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetIncomingItemsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(items);

        // Act
        var result = await _service.GetIncomingNextDayAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetIncomingThisWeekAsync_WhenNoItems_ReturnsEmpty()
    {
        // Arrange
        var items = new List<TodoItem>();
        _unitOfWorkMock.Setup(u => u.TodoItem.GetIncomingItemsAsync(It.IsAny<DateTime>())).ReturnsAsync(items);

        // Act
        var result = await _service.GetIncomingThisWeekAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_WhenItemIsValid_ShouldAddItem()
    {
        // Arrange
        var item = new TodoItemDto { Title = "Test", Description = "Test" };
        var addedItem = new TodoItem { Id = 1, Title = "Test", Description = "Test" };
        _unitOfWorkMock.Setup(u => u.TodoItem.AddAsync(It.IsAny<TodoItem>())).ReturnsAsync(addedItem);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _service.AddAsync(item);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(addedItem.Id, result.Id);
    }

    [Fact]
    public async Task AddAsync_WhenItemIsInvalid_ShouldThrowException()
    {
        // Arrange
        var item = new TodoItemDto { 
            Title = "Test",
            Description = "Test",
            PercentComplete = 100, // IsComplete is false by default
        }; // Invalid item

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddAsync(item));
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_ShouldUpdateItem()
    {
        // Arrange
        var item = new TodoItemDto { Id = 1, Title = "Updated", Description = "Updated" };
        var existingItem = new TodoItem { Id = 1, Title = "Test", Description = "Test" };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync(existingItem);

        // Act
        UpdateResult result = await _service.UpdateAsync(item.Id, item);

        // Assert
        Assert.Equal(UpdateResult.Success, result);
        _unitOfWorkMock.Verify(u => u.TodoItem.Update(It.IsAny<TodoItem>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var item = new TodoItemDto { Id = 1, Title = "Updated", Description = "Updated" };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        UpdateResult result = await _service.UpdateAsync(item.Id, item);

        // Assert
        Assert.Equal(UpdateResult.NotFound, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenIdDoesNotMatch_ReturnsBadRequest()
    {
        // Arrange
        var item = new TodoItemDto { Id = 1, Title = "Updated", Description = "Updated" };
        var existingItem = new TodoItem { Id = 2, Title = "Test", Description = "Test" };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync(existingItem);

        // Act
        UpdateResult result = await _service.UpdateAsync(2, item);

        // Assert
        Assert.Equal(UpdateResult.BadRequest, result);
    }

    [Fact]
    public async Task UpdatePercentCompleteAsync_WhenItemExists_ReturnsTrue()
    {
        // Arrange
        var item = new TodoItemDto { Id = 1, PercentComplete = 50 };
        var existingItem = new TodoItem { Id = 1, Title = "Test", Description = "Test" };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync(existingItem);
    
        // Act
        var result = await _service.UpdatePercentCompleteAsync(item.Id, item.PercentComplete);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(u => u.TodoItem.Update(It.IsAny<TodoItem>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdatePercentCompleteAsync_WhenItemDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var item = new TodoItemDto { Id = 1, PercentComplete = 50 };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _service.UpdatePercentCompleteAsync(item.Id, item.PercentComplete);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenItemExists_ReturnsTrue()
    {
        // Arrange
        var item = new TodoItemDto { Id = 1, IsCompleted = true };
        var existingItem = new TodoItem { Id = 1, Title = "Test", Description = "Test" };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync(existingItem);

        // Act
        var result = await _service.UpdateStatusAsync(item.Id, item.IsCompleted);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(u => u.TodoItem.Update(It.IsAny<TodoItem>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenItemDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var item = new TodoItemDto { Id = 1, IsCompleted = true };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _service.UpdateStatusAsync(item.Id, item.IsCompleted);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsTrue()
    {
        // Arrange
        var item = new TodoItem { Id = 1, Title = "Test", Description = "Test" };
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync(item);

        // Act
        var result = await _service.DeleteAsync(item.Id);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(u => u.TodoItem.Delete(It.IsAny<TodoItem>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.TodoItem.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.False(result);
    }
}