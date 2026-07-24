using Microsoft.EntityFrameworkCore;
using TaskManagement.Entities;
using TaskManagement.Entities.Enums;

namespace TaskManagement.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var user = new User
        {
            Username = "testuser",
            Password = "12345"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var projects = new[]
         {
            new Project
            {
                UserId = user.Id,
                Name = "Backend API",
                Description = "Build Task Management API"
            },
            new Project
            {
                UserId = user.Id,
                Name = "Frontend Application",
                Description = "Build frontend client"
            }
        };

        context.Projects.AddRange(projects);
        await context.SaveChangesAsync();


        var priorities = Enum.GetValues<Priority>();
        var statuses = Enum.GetValues<Status>();

        var tasks = new List<TaskItem>();
        var now = DateTime.UtcNow;

        foreach (var project in projects)
        {
            for (int i = 1; i <= 5; i++)
            {
                tasks.Add(new()
                {
                    ProjectId = project.Id,
                    Title = $"Task {i} - {project.Name}",
                    Description = $"Description for task {i}",
                    Priority = priorities[(i - 1) % priorities.Length],
                    Status = statuses[(i - 1) % statuses.Length],
                    DueDate = DateOnly.FromDateTime(now.AddDays(i))
                });
            }
        }

        context.Tasks.AddRange(tasks);

        await context.SaveChangesAsync();
    }
}
