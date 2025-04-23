using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        // Table name
        builder.ToTable("todo_items");

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd(); // Id is auto-incremented

        builder.Property(t => t.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(200); // Title is required and has a max length of 200 characters

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(1000); // Description has a max length of 1000 characters

        builder.Property(t => t.PercentComplete)
            .HasColumnName("percent_complete")
            .IsRequired(); // PercentComplete is required

        builder.Property(t => t.IsCompleted)
            .HasColumnName("is_completed")
            .HasDefaultValue(false); // Default value for IsCompleted is false

        builder.Property(t => t.ExpiryDateTime)
            .HasColumnName("expiry_date_time")
            .HasColumnType("timestamp with time zone") // Use timestamp with time zone for ExpiryDateTime
            .IsRequired(); // ExpiryDateTime is required

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for CreatedAt is the current timestamp

        builder.Property(t => t.LastUpdatedAt)
            .HasColumnName("last_updated_at")
            .HasColumnType("timestamp with time zone"); // LastUpdatedAt is optional and uses timestamp with time zone
    }
}