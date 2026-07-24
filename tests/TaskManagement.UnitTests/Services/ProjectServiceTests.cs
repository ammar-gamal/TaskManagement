using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using TaskManagement.Dtos.Project;
using TaskManagement.Entities;
using TaskManagement.Persistence.Interfaces;
using TaskManagement.Services;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utilites;

namespace TaskManagement.UnitTests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IAppDbContext> _contextMock;
    private readonly Mock<ILogger<ProjectService>> _loggerMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly ProjectService _sut;
    private readonly DateTime _fixedUtcNow = new(2026, 7, 22, 12, 0, 0);

    private readonly int _userId = 100;

    public ProjectServiceTests()
    {
        _contextMock = new Mock<IAppDbContext>();
        _loggerMock = new Mock<ILogger<ProjectService>>();
        _currentUserMock = new Mock<ICurrentUserService>();

        _currentUserMock.Setup(cu => cu.Id).Returns(_userId);

        _sut = new ProjectService(
            _contextMock.Object,
            _currentUserMock.Object,
            _loggerMock.Object);
    }

    #region CreateAsync
    [Fact]
    public async Task CreateAsync_WhenProjectNameAlreadyExists_ReturnsConflictError()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "N", Description = "D" };
        var existingProjects = new List<Project> { new() { Id = 1, Name = "N", UserId = _userId } };

        _contextMock.Setup(c => c.Projects).ReturnsDbSet(existingProjects);

        // Act
        var result = await _sut.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);

        _contextMock.Verify(c => c.Projects.Add(It.IsAny<Project>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_AllValid_CreatesProjectSuccessfully()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "N", Description = "D" };
        var existingProjects = new List<Project>();

        _contextMock.Setup(c => c.Projects).ReturnsDbSet(existingProjects);

        // Act
        var result = await _sut.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _contextMock.Verify(c => c.Projects.Add(It.Is<Project>(p =>
                    p.Name == dto.Name &&
                    p.Description == dto.Description &&
                    p.UserId == _userId
                )), Times.Once);

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WhenProjectDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        int projectId = 99;
        var dto = new UpdateProjectDto { Name = "New Name" };
        var projects = new List<Project>();

        _contextMock.Setup(c => c.Projects).ReturnsDbSet(projects);

        // Act
        var result = await _sut.UpdateAsync(projectId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _contextMock.Verify(c => c.Projects.Update(It.IsAny<Project>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectNameIsTaken_ReturnsConflictError()
    {
        // Arrange
        int projectId = 1;
        var existingProject = new Project { Id = projectId, Name = "My Project", UserId = _userId };
        var conflictingProject = new Project { Id = 2, Name = "Taken Name", UserId = _userId };

        var projects = new List<Project> { existingProject, conflictingProject };
        _contextMock.Setup(c => c.Projects).ReturnsDbSet(projects);

        var dto = new UpdateProjectDto { Name = "Taken Name", Description = "Updated Desc" };

        // Act
        var result = await _sut.UpdateAsync(projectId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        _contextMock.Verify(c => c.Projects.Update(It.IsAny<Project>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_AllValid_UpdatesProjectSuccessfully()
    {
        // Arrange
        int projectId = 1;
        var existingProject = new Project { Id = projectId, Name = "Old Name", Description = "Old Desc", UserId = _userId };
        var projects = new List<Project> { existingProject };

        _contextMock.Setup(c => c.Projects).ReturnsDbSet(projects);

        var dto = new UpdateProjectDto { Name = "New Name", Description = "New Desc" };

        // Act
        var result = await _sut.UpdateAsync(projectId, dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _contextMock.Verify(c => c.Projects.Update(It.Is<Project>(p =>
                             p.Id == projectId &&
                             p.Name == dto.Name &&
                             p.Description == dto.Description
                             )), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }
    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        int projectId = 99;
        var projects = new List<Project>();
        _contextMock.Setup(c => c.Projects).ReturnsDbSet(projects);

        // Act
        var result = await _sut.DeleteAsync(projectId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _contextMock.Verify(c => c.Projects.Remove(It.IsAny<Project>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectExists_DeletesProjectSuccessfully()
    {
        // Arrange
        int projectId = 1;
        var existingProject = new Project { Id = projectId, Name = "N", UserId = _userId };
        var projects = new List<Project> { existingProject };

        _contextMock.Setup(c => c.Projects).ReturnsDbSet(projects);

        // Act
        var result = await _sut.DeleteAsync(projectId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _contextMock.Verify(c => c.Projects.Remove(It.Is<Project>(p => p.Id == projectId)), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_AllValid_ReturnsProject()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            UserId = _userId,
            Name = "Name",
            Description = "Description",
            CreatedAt = _fixedUtcNow.AddDays(-2),
            UpdatedAt = _fixedUtcNow.AddDays(-1)
        };

        _contextMock
            .Setup(c => c.Projects)
            .ReturnsDbSet([project]);

        // Act
        var result = await _sut.GetByIdAsync(project.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var data = result.Data;
        data.Should().NotBeNull();
        data.Id.Should().Be(project.Id);
        data.Name.Should().Be(project.Name);
        data.Description.Should().Be(project.Description);
        data.CreatedAt.Should().Be(project.CreatedAt);
        data.UpdatedAt.Should().Be(project.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        _contextMock.Setup(c => c.Projects)
            .ReturnsDbSet([]);

        // Act
        var result = await _sut.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
    #endregion

}
