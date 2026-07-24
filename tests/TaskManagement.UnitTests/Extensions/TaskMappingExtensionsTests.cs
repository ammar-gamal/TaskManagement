using FluentAssertions;
using TaskManagement.Dtos.Task;
using TaskManagement.Entities;
using TaskManagement.Entities.Enums;
using TaskManagement.ExtensionMethods.Mapping;

namespace TaskManagement.UnitTests.Extensions;

public class TaskMappingExtensionsTests
{
    [Fact]
    public void ToEntity_WhenStatusAndPriorityAreNull_AppliesDefaultValues()
    {
        // Arrange
        int projectId = 10;
        var dto = new CreateTaskDto
        {
            Title = "1",
            Description = "2",
            Status = null,
            Priority = null,
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
        };

        // Act
        var result = dto.ToEntity(projectId);

        // Assert
        result.Status.Should().Be(Status.Todo);
        result.Priority.Should().Be(Priority.Medium);
        result.Should().NotBeNull();
        result.ProjectId.Should().Be(projectId);
        result.Title.Should().Be(dto.Title);
        result.Description.Should().Be(dto.Description);
        result.DueDate.Should().Be(dto.DueDate);
    }

    [Fact]
    public void ToEntity_WhenStatusAndPriorityAreProvided_MapsCustomValues()
    {
        // Arrange
        int projectId = 10;
        var dto = new CreateTaskDto
        {
            Title = "Amazing",
            Status = Status.InProgress,
            Priority = Priority.High,
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = dto.ToEntity(projectId);

        // Assert
        result.Status.Should().Be(dto.Status);
        result.Priority.Should().Be(dto.Priority);
    }

    [Fact]
    public void ToDto_MapsEntityToDtoCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = new TaskItem
        {
            Id = 5,
            ProjectId = 10,
            Title = "Sample Task",
            Description = "Sample Description",
            Status = Status.Done,
            Priority = Priority.High,
            DueDate = DateOnly.FromDateTime(now.AddDays(5)),
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = entity.ToDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.ProjectId.Should().Be(entity.ProjectId);
        result.Title.Should().Be(entity.Title);
        result.Description.Should().Be(entity.Description);
        result.Status.Should().Be(entity.Status);
        result.Priority.Should().Be(entity.Priority);
        result.DueDate.Should().Be(entity.DueDate);
        result.CreatedAt.Should().Be(entity.CreatedAt);
        result.UpdatedAt.Should().Be(entity.UpdatedAt);
    }
}
