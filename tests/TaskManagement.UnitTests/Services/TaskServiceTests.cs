using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Moq.EntityFrameworkCore;
using TaskManagement.Dtos.Task;
using TaskManagement.Entities;
using TaskManagement.Entities.Enums;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Services;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utilites;

namespace TaskManagement.UnitTests.Services;

public class TaskServiceTests
{
    private readonly TaskService _sut;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IAppDbContext> _contextMock;
    private readonly Mock<ILogger<TaskService>> _loggerMock;
    private readonly FakeTimeProvider _fakeTimeProvider;
    private readonly DateTime _fixedUtcNow = new(2026, 7, 22, 12, 0, 0);
    private readonly int _userId = 100;
    public TaskServiceTests()
    {
        _fakeTimeProvider = new(_fixedUtcNow);
        _contextMock = new();
        _currentUserMock = new();
        _loggerMock = new();
        _currentUserMock.Setup(cu => cu.Id).Returns(_userId);


        _sut = new(
        _loggerMock.Object,
        _fakeTimeProvider,
        _contextMock.Object,
        _currentUserMock.Object);

    }

    #region CreateAsync
    [Fact]
    public async Task CreateAsync_WhenProjectDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        int projectId = 1;
        var dto = new CreateTaskDto
        {
            Title = "Test Task",
            DueDate = DateOnly.FromDateTime(_fixedUtcNow)
        };

        _contextMock.Setup(c => c.Projects)
                    .ReturnsDbSet([]);

        // Act
        var result = await _sut.CreateAsync(projectId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _contextMock.Verify(c => c.Tasks.Add(It.IsAny<TaskItem>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task CreateAsync_WhenDueDateIsInThePast_ReturnsBadRequestError()
    {
        // Arrange/
        int projectId = 1;
        var pastDate = DateOnly.FromDateTime(_fixedUtcNow).AddDays(-1);
        var dto = new CreateTaskDto { Title = "Test Task", DueDate = pastDate };

        _contextMock.Setup(c => c.Projects)
                    .ReturnsDbSet([new() { Id = projectId, UserId = _userId }]);

        // Act
        var result = await _sut.CreateAsync(projectId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.BadRequest);
        _contextMock.Verify(c => c.Tasks.Add(It.IsAny<TaskItem>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task CreateAsync_AllValid_CreatesTaskSuccessfullyAndReturnsDto()
    {
        // Arrange
        int projectId = 1;
        var validDate = DateOnly.FromDateTime(_fixedUtcNow).AddDays(2);
        var dto = new CreateTaskDto
        {
            Title = "Almentor1",
            Description = "Solve Task",
            DueDate = validDate
        };

        _contextMock.Setup(c => c.Projects)
                    .ReturnsDbSet([new() { Id = projectId, UserId = _userId }]);

        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([]);

        // Act
        var result = await _sut.CreateAsync(projectId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        _contextMock.Verify(c => c.Tasks.Add(It.Is<TaskItem>(t =>
            t.ProjectId == projectId &&
            t.Title == dto.Title &&
            t.DueDate == dto.DueDate
        )), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync
    [Fact]
    public async Task DeleteAsync_WhenTaskDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        int taskId = 99;
        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([]);

        // Act
        var result = await _sut.DeleteAsync(taskId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _contextMock.Verify(c => c.Tasks.Remove(It.IsAny<TaskItem>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_AllValid_DeletesTaskSuccessfully()
    {
        // Arrange
        int taskId = 1;
        var project = new Project() { Id = 1, UserId = _userId };
        var existingTask = new TaskItem { Id = taskId, Title = "Task to delete", Project = project };
        var tasks = new List<TaskItem> { existingTask };

        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([existingTask]);

        // Act
        var result = await _sut.DeleteAsync(taskId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _contextMock.Verify(c => c.Tasks.Remove(It.Is<TaskItem>(t => t.Id == taskId)), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WhenTaskDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        int taskId = 99;
        var dto = new UpdateTaskDto { Title = "Updated Title" };
        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([]);

        // Act
        var result = await _sut.UpdateAsync(taskId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenDueDateIsInThePast_ReturnsBadRequestError()
    {
        // Arrange
        int taskId = 1;
        var project = new Project() { Id = 1, UserId = _userId };
        var existingTask = new TaskItem { Id = taskId, Title = "Original Title", Project = project };
        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([existingTask]);

        var pastDate = DateOnly.FromDateTime(_fixedUtcNow).AddDays(-1);
        var dto = new UpdateTaskDto { Title = "Updated Title", DueDate = pastDate };

        // Act
        var result = await _sut.UpdateAsync(taskId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.BadRequest);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_AllValid_UpdatesTaskSuccessfully()
    {
        // Arrange
        int taskId = 1;
        var project = new Project() { Id = 1, UserId = _userId };
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Title",
            Description = "Descritipion",
            DueDate = DateOnly.FromDateTime(_fixedUtcNow).AddDays(1),
            Status = Status.InProgress,
            Priority = Priority.Low,
            Project = project
        };
        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([existingTask]);

        var futureDate = DateOnly.FromDateTime(_fixedUtcNow).AddDays(5);
        var dto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Desc",
            Status = Status.Todo,
            Priority = Priority.High,
            DueDate = futureDate
        };

        // Act
        var result = await _sut.UpdateAsync(taskId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(c => c.Tasks.Update(It.Is<TaskItem>(t =>
                     t.Id == taskId &&
                     t.Title == dto.Title &&
                     t.Description == dto.Description &&
                     t.DueDate == dto.DueDate &&
                     t.Priority == dto.Priority &&
                     t.Status == dto.Status
                 )), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpadtingFromDoneToTodo_UpdatesSuccessfullyAndLogsWarning()
    {
        // Arrange
        int taskId = 1;
        var project = new Project() { Id = 1, UserId = _userId };

        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task",
            Status = Status.Done,
            Priority = Priority.Medium,
            Project = project
        };
        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([existingTask]);

        var dto = new UpdateTaskDto
        {
            Title = "Task",
            Status = Status.Todo,
            Priority = Priority.Medium
        };

        // Act
        var result = await _sut.UpdateAsync(taskId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingTask.Status.Should().Be(dto.Status);

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(c => c.Tasks.Update(It.Is<TaskItem>(t =>
                             t.Id == taskId &&
                             t.Title == dto.Title &&
                             t.Description == dto.Description &&
                             t.DueDate == dto.DueDate &&
                             t.Priority == dto.Priority &&
                             t.Status == dto.Status
                         )), Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_AllValid_ReturnsTaskDetailsDto()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "Project One",
            UserId = _userId
        };

        var task = new TaskItem
        {
            Id = 10,
            ProjectId = project.Id,
            Project = project,
            Title = "feature",
            Description = "Description",
            Status = Status.Todo,
            Priority = Priority.High,
            DueDate = DateOnly.FromDateTime(_fixedUtcNow.AddDays(2)),
            CreatedAt = _fixedUtcNow.AddDays(-1),
            UpdatedAt = _fixedUtcNow
        };

        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([task]);

        // Act
        var result = await _sut.GetByIdAsync(task.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var data = result.Data;
        data.Should().NotBeNull();
        data.Id.Should().Be(task.Id);
        data.ProjectId.Should().Be(project.Id);
        data.ProjectName.Should().Be(project.Name);
        data.Title.Should().Be(task.Title);
        data.Description.Should().Be(task.Description);
        data.Status.Should().Be(task.Status);
        data.Priority.Should().Be(task.Priority);
        data.DueDate.Should().Be(task.DueDate);
        data.CreatedAt.Should().Be(task.CreatedAt);
        data.UpdatedAt.Should().Be(task.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        _contextMock
            .Setup(c => c.Tasks)
            .ReturnsDbSet([]);

        // Act
        var result = await _sut.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
    #endregion

    #region ListForProjectAsync
    [Fact]
    public async Task ListForProjectAsync_WhenProjectDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        _contextMock
            .Setup(c => c.Projects)
            .ReturnsDbSet([]);

        var query = new TaskQueryParameters();

        // Act
        var result = await _sut.ListForProjectAsync(999, query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ListForProjectAsync_AlLValid_ReturnsPaginatedTaskDetailsDto()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "Project",
            UserId = _userId
        };
        var task = new TaskItem()
        {
            Id = 1,
            ProjectId = project.Id,
            Project = project,
            Title = "Task 1",
            Description = "Description 1",
            Status = Status.Todo,
            Priority = Priority.High,
            DueDate = DateOnly.FromDateTime(_fixedUtcNow.AddDays(2)),
            CreatedAt = _fixedUtcNow.AddDays(-2),
            UpdatedAt = _fixedUtcNow.AddDays(-1)
        };

        _contextMock.Setup(c => c.Projects)
                    .ReturnsDbSet([project]);

        _contextMock.Setup(c => c.Tasks)
                    .ReturnsDbSet([task]);

        var query = new TaskQueryParameters();

        // Act
        var result = await _sut.ListForProjectAsync(project.Id, query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var dto = result.Data.PageItems.First();

        dto.Id.Should().Be(task.Id);
        dto.ProjectId.Should().Be(project.Id);
        dto.ProjectName.Should().Be(project.Name);
        dto.Description.Should().Be(task.Description);
        dto.Title.Should().Be(task.Title);
        dto.Priority.Should().Be(task.Priority);
        dto.Status.Should().Be(task.Status);
        dto.DueDate.Should().Be(task.DueDate);
        dto.CreatedAt.Should().Be(task.CreatedAt);
        dto.UpdatedAt.Should().Be(task.UpdatedAt);


    }
    #endregion
}
