using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests.Integration;

public class TestProgram : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove EF Core 9 provider config registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(IDbContextOptionsConfiguration<TodoItemDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Register In-Memory database for tests
            services.AddDbContext<TodoItemDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));  // For testing only

            // Build provider and initialize database
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider
                               .GetRequiredService<TodoItemDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });
    }
}
