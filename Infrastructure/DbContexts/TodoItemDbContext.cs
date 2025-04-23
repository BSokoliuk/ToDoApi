using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.DbContexts;

public class TodoItemDbContext(DbContextOptions<TodoItemDbContext> options) : DbContext(options)
{
  public DbSet<TodoItem> TodoItems { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Seed
        modelBuilder.Entity<TodoItem>().HasData(new TodoItem
        {
            Id = 1,
            Title = "Sample Todo Item",
            Description = "This is a sample todo item.",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            ExpiryDateTime = DateTime.UtcNow.AddDays(7)
        });

        modelBuilder.Entity<TodoItem>().HasData(new TodoItem
        {
            Id = 2,
            Title = "Another Todo Item",
            Description = "This is another todo item.",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            ExpiryDateTime = DateTime.UtcNow.AddDays(14)
        });

        modelBuilder.Entity<TodoItem>().HasData(new TodoItem
        {
            Id = 3,
            Title = "Completed Todo Item",
            Description = "This todo item is completed.",
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            ExpiryDateTime = DateTime.UtcNow.AddDays(-5)
        });
    }
}