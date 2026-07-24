using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Task;
using TaskManagement.Entities;
using TaskManagement.Entities.Enums;
using TaskManagement.IntegrationTests.Utilities;
using TaskManagement.IntegrationTests.Utilities.Constants;
using TaskManagement.Persistence;

namespace TaskManagement.IntegrationTests.Services.TaskTests;

public class TaskFilteringTests : IClassFixture<TaskManagementWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TaskManagementWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
    {
        new JsonStringEnumConverter()
    }
    };
    public TaskFilteringTests(TaskManagementWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Theory]
    [InlineData(Status.Todo, Priority.Low)]
    [InlineData(Status.InProgress, Priority.Medium)]
    [InlineData(Status.Done, Priority.High)]
    [InlineData(null, Priority.Low)]
    [InlineData(Status.Done, null)]
    public async Task ListTasks_FiltersByStatusAndPriority(Status? status, Priority? priority)
    {
        await SeedTasksAsync();

        var query = new List<string>();
        if (status is not null) query.Add($"status={status}");
        if (priority is not null) query.Add($"priority={priority}");
        var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";

        var response = await _client.GetAsync($"/api/tasks{queryString}", CancellationToken.None);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<TaskDetailsDto>>(
            JsonOptions, CancellationToken.None);

        result.Should().NotBeNull();
        result!.PageItems.Should().NotBeEmpty();
        foreach (var item in result.PageItems)
        {
            if (status is not null) item.Status.Should().Be(status);
            if (priority is not null) item.Priority.Should().Be(priority);
        }
    }



    private async Task SeedTasksAsync()
    {

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Tasks.RemoveRange(dbContext.Tasks);
        dbContext.Projects.RemoveRange(dbContext.Projects);
        await dbContext.SaveChangesAsync();
        for (int i = 1; i <= 2; i++)
        {
            var project = new Project
            {
                Name = $"Project {i}",
                Description = $"Description {i}",
                UserId = TestAuthData.UserId
            };

            List<TaskItem> tasks = [
            new() { Title = $"Task 1 In Project {project.Name}",  Status = Status.Todo, Priority = Priority.Low, Project = project },
            new() { Title = $"Task 2 In Project {project.Name}",  Status = Status.Todo, Priority = Priority.Medium, Project = project },
            new() { Title = $"Task 3 In Project {project.Name}",  Status = Status.Todo, Priority = Priority.High, Project = project },
            new() { Title = $"Task 4 In Project {project.Name}",  Status = Status.InProgress, Priority = Priority.Low, Project = project },
            new() { Title = $"Task 5 In Project {project.Name}",  Status = Status.InProgress, Priority = Priority.Medium, Project = project },
            new() { Title = $"Task 6 In Project {project.Name}",  Status = Status.InProgress, Priority = Priority.High, Project = project },
            new() { Title = $"Task 7 In Project {project.Name}",  Status = Status.Done, Priority = Priority.Low, Project = project },
            new() { Title = $"Task 8 In Project {project.Name}",  Status = Status.Done, Priority = Priority.Medium, Project = project },
            new() { Title = $"Task 9 In Project {project.Name}",  Status = Status.Done, Priority = Priority.High, Project = project }
            ];
            dbContext.Projects.Add(project);

            dbContext.Tasks.AddRange(tasks);
            await dbContext.SaveChangesAsync();

        }

    }

}
