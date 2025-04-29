using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.DbContexts;

namespace API.Tests.Integration;

public class TodoItemControllerTests : IClassFixture<TestProgram>
{
    private readonly HttpClient _client;
    private readonly TestProgram _factory;

    public TodoItemControllerTests(TestProgram factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Seed the database before each test
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoItemDbContext>();

        // Clear existing data
        context.TodoItems.RemoveRange(context.TodoItems);
        context.SaveChanges();

        // Seed test data
        context.TodoItems.Add(new Domain.Entities.TodoItem
        {
            Id = 1,
            Title = "Test Item",
            Description = "Test Description",
            ExpiryDateTime = DateTime.UtcNow.AddDays(1)
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithTodoItems()
    {
        // Act
        var response = await _client.GetAsync("/api/TodoItem");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoItems = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        todoItems.Should().NotBeNull();
        todoItems.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetById_ReturnsOkWithTodoItem()
    {
        // Act
        var response = await _client.GetAsync("/api/TodoItem/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoItem = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        todoItem.Should().NotBeNull();
        todoItem.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/TodoItem/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetIncomingItems_ReturnsOkWithTodoItems()
    {
        // Arrange
        var dateTime = DateTime.UtcNow.AddDays(2);

        // Act
        var response = await _client.GetAsync($"/api/TodoItem/incoming/{dateTime:yyyy-MM-ddTHH:mm:ssZ}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoItems = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        todoItems.Should().NotBeNull();
        todoItems.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetIncomingToday_ReturnsOkWithEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/TodoItem/incoming/today");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoItems = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        todoItems.Should().NotBeNull();
        todoItems.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetIncomingNextDay_ReturnsOkWithTodoItems()
    {
        // Act
        var response = await _client.GetAsync("/api/TodoItem/incoming/nextday");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoItems = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        todoItems.Should().NotBeNull();
        todoItems.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetIncomingWeek_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/TodoItem/incoming/week");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoItems = await response.Content.ReadFromJsonAsync<List<TodoItemDto>>();
        todoItems.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_ReturnsCreated()
    {
        // Arrange
        var newTodoItem = new TodoItemDto
        {
            Title = "New Item",
            Description = "New Description",
            ExpiryDateTime = DateTime.UtcNow.AddDays(2),
            IsCompleted = false,
            PercentComplete = 33
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/TodoItem", newTodoItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTodoItem = await response.Content.ReadFromJsonAsync<TodoItemDto>();
        createdTodoItem.Should().NotBeNull();
        createdTodoItem.Should().BeEquivalentTo(newTodoItem, options => options.ExcludingMissingMembers().Excluding(x => x.Id));
    }

    [Fact]
    public async Task Post_WhenIdIsProvided_ReturnsBadRequest()
    {
        // Arrange
        var newTodoItem = new TodoItemDto
        {
            Id = 1,
            Title = "New Item",
            Description = "New Description",
            ExpiryDateTime = DateTime.UtcNow.AddDays(2),
            IsCompleted = false,
            PercentComplete = 33
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/TodoItem", newTodoItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WhenModelIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var newTodoItem = new TodoItemDto
        {
            Id = 1,
            Title = "New Item",
            Description = "New Description",
            ExpiryDateTime = DateTime.UtcNow.AddDays(2),
            IsCompleted = false,
            PercentComplete = 102 // Invalid percent complete
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/TodoItem", newTodoItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Contain("PercentComplete must be between 0 and 100.");
    }

    [Fact]
    public async Task Put_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var updatedTodoItem = new TodoItemDto
        {
            Id = 1,
            Title = "Updated Item",
            Description = "Updated Description",
            ExpiryDateTime = DateTime.UtcNow.AddDays(3),
            IsCompleted = true,
            PercentComplete = 100
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/1", updatedTodoItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Put_WhenItemDoesNotExist_ReturnsNotFound_()
    {
        // Arrange
        var updatedTodoItem = new TodoItemDto
        {
            Id = 2,
            Title = "Updated Item",
            Description = "Updated Description",
            ExpiryDateTime = DateTime.UtcNow.AddDays(3),
            IsCompleted = true,
            PercentComplete = 100
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/2", updatedTodoItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_WhenIdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var updatedTodoItem = new TodoItemDto
        {
            Id = 2,
            Title = "Updated Item",
            Description = "Updated Description",
            ExpiryDateTime = DateTime.UtcNow.AddDays(3),
            IsCompleted = true,
            PercentComplete = 100
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/1", updatedTodoItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Contain("Id mismatch.");
    }

    [Fact]
    public async Task UpdatePercentComplete_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var percentComplete = 50;

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/1/percent-complete", percentComplete);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdatePercentComplete_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var percentComplete = 50;

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/5/percent-complete", percentComplete);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePercentComplete_WhenPercentCompleteIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var invalidPercentComplete = 102; // range is 0-100

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/1/percent-complete", invalidPercentComplete);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorMessage = await response.Content.ReadAsStringAsync();
        errorMessage.Should().Contain("Percent complete must be between 0 and 100.");
    }

    [Fact]
    public async Task UpdateStatus_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var isCompleted = true;

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/1/status", isCompleted);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateStatus_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var isCompleted = true;

        // Act
        var response = await _client.PutAsJsonAsync("/api/TodoItem/2/status", isCompleted);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenItemExists_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/api/TodoItem/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/TodoItem/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}