using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagement.Dtos.Project;
using TaskManagement.Dtos.Task;
using TaskManagement.Entities.Enums;
using TaskManagement.IntegrationTests.Utilities;

namespace TaskManagement.IntegrationTests.Services.ProjectTests;

public class ProjectLifeCycleTests : IClassFixture<TaskManagementWebApplicationFactory>
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
    public ProjectLifeCycleTests(TaskManagementWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    [Fact]
    public async Task CreateProject_AddTask_MarkDone_DeleteProjectAndCascadeTasks()
    {
        // 1.Create project
        var newProject = new CreateProjectDto
        {
            Name = $"Project {Guid.NewGuid()}",
            Description = "Description"
        };

        var projectResponse = await _client.PostAsJsonAsync("/api/projects", newProject, CancellationToken.None);
        projectResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectDto>(CancellationToken.None);
        project.Should().NotBeNull();

        // 2.Add a task
        var newTask = new CreateTaskDto
        {
            Title = "TaskTitle",
            Status = Status.InProgress
        };

        var taskResponse = await _client.PostAsJsonAsync($"/api/projects/{project.Id}/tasks", newTask, JsonOptions, CancellationToken.None);
        taskResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var task = await taskResponse.Content.ReadFromJsonAsync<TaskDto>(JsonOptions, CancellationToken.None);
        task.Should().NotBeNull();
        task.ProjectId.Should().Be(project.Id);

        // 3.Mark task as done
        var updateTask = new UpdateTaskDto
        {
            Description = task.Description,
            DueDate = task.DueDate,
            Status = Status.Done,
            Priority = task.Priority,
            Title = task.Title
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{task.Id}", updateTask, JsonOptions, CancellationToken.None);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskDto>(JsonOptions, CancellationToken.None);
        updatedTask.Should().NotBeNull();
        updatedTask.Status.Should().Be(Status.Done);

        // 4.Delete the project
        var deleteResponse = await _client.DeleteAsync($"/api/projects/{project.Id}", CancellationToken.None);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);


        var getProjectResponse = await _client.GetAsync($"/api/projects/{project.Id}", CancellationToken.None);
        getProjectResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var getTaskResponse = await _client.GetAsync($"/api/tasks/{task.Id}", CancellationToken.None);
        getTaskResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
