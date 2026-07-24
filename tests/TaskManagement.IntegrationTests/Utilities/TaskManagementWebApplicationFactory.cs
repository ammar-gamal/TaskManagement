using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Entities;
using TaskManagement.IntegrationTests.Utilities.Constants;
using TaskManagement.Persistence;
using Testcontainers.MsSql;

namespace TaskManagement.IntegrationTests.Utilities;

public class TaskManagementWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
            .Build();
    public async ValueTask InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
        var user = new User
        {
            Username = "FirstUser",
            Password = "FirstPassword"
        };
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        TestAuthData.UserId = user.Id;

    }


    public new async ValueTask DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var descriptor = services
                  .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_sqlContainer.GetConnectionString());
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthData.Scheme;
                options.DefaultChallengeScheme = TestAuthData.Scheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthData.Scheme, options => { });
        });
    }
}
