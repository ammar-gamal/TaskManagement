using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagement.Dtos.Common;
using TaskManagement.Dtos.Task;
using TaskManagement.Entities;
using TaskManagement.IntegrationTests.Utilities;
using TaskManagement.IntegrationTests.Utilities.Constants;
using TaskManagement.Persistence;

namespace TaskManagement.IntegrationTests.Services.TaskTests;

public class TaskSearchAndPaginationTests : IClassFixture<TaskManagementWebApplicationFactory>
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
    public TaskSearchAndPaginationTests(TaskManagementWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task SearchTasks_ByTitleAndDescription_IsCaseInsensitive()
    {
        var marker = Guid.NewGuid().ToString("N")[..8];
        await SeedTasksForSearchAsync(marker);


        var response = await _client.GetAsync($"/api/tasks?q={marker.ToUpperInvariant()}", CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<TaskDetailsDto>>(JsonOptions, CancellationToken.None);
        result.Should().NotBeNull();
        result.PageItems.Should().OnlyContain(t =>
             t.Title.Contains(marker, StringComparison.OrdinalIgnoreCase) ||
             (t.Description != null && t.Description.Contains(marker, StringComparison.OrdinalIgnoreCase)));
        result!.TotalCount.Should().Be(6);
    }

    [Fact]
    public async Task ListTasks_ReturnsCorrectPageMetadata()
    {
        var marker = Guid.NewGuid().ToString("N")[..8];
        await SeedTasksForSearchAsync(marker);


        var response = await _client.GetAsync($"/api/tasks?q={marker.ToUpperInvariant()}&pageindex=2&limit=2", CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<TaskDetailsDto>>(JsonOptions, CancellationToken.None);

        result!.PageIndex.Should().Be(2);
        result.TotalCount.Should().Be(6);
        result.PageItems.Should().HaveCount(2);
        result.TotalPages.Should().Be(3);
    }
    private async Task SeedTasksForSearchAsync(string marker)
    {

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Tasks.RemoveRange(dbContext.Tasks);
        dbContext.Projects.RemoveRange(dbContext.Projects);
        await dbContext.SaveChangesAsync();

        var project = new Project
        {
            Name = $"Project Search",
            Description = $"Description Search",
            UserId = TestAuthData.UserId
        };

        List<TaskItem> tasks = [
            new() { Title = $"Rotate {marker} credentials", Project = project },
            new() { Title = "Unrelated title", Description = "Unrelated desc", Project = project },
            new() { Title = "Unrelated", Description = $"mentions {marker} in the descr", Project = project },
            new() { Title = $"i am {marker}", Project = project },
            new() { Title = $"i am {marker}", Description = $"mentions {marker} in the body", Project = project },
            new() { Title = $"again {marker}", Project = project },
            new() { Title = "good", Description = $"{marker}", Project = project },
            new() { Title = "none", Project = project },
            new() { Title = "none", Project = project }
          ];

        dbContext.Projects.Add(project);
        dbContext.Tasks.AddRange(tasks);
        await dbContext.SaveChangesAsync();


    }
}