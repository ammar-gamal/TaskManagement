using FluentAssertions;
using TaskManagement.Dtos.Project;
using TaskManagement.Entities;
using TaskManagement.ExtensionMethods.Mapping;
namespace TaskManagement.UnitTests.Extensions;

public class ProjectMappingExtensionsTests
{
    [Fact]
    public void ToEntity_MapsCreateProjectDtoToProjectCorrectly()
    {
        // Arrange
        var userId = 100;
        var dto = new CreateProjectDto
        {
            Name = "Name",
            Description = "Desc"
        };

        // Act
        var result = dto.ToEntity(userId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Description.Should().Be(dto.Description);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public void ToDto_MapsProjectEntityToProjectDtoCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var userId = 100;
        var entity = new Project
        {
            Id = 5,
            Name = "Name",
            Description = "Desc",
            UserId = userId,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act
        var result = entity.ToDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be(entity.Name);
        result.Description.Should().Be(entity.Description);
        result.CreatedAt.Should().Be(entity.CreatedAt);
        result.UpdatedAt.Should().Be(entity.UpdatedAt);
    }
}