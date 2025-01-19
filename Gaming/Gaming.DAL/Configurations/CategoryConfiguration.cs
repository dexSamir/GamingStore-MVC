using System;
using Gaming.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gaming.DAL.Configurations;
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(x => x.Name)
           .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(64);

        builder.HasMany(x => x.Games)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);
    }
}

