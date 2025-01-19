using System;
using Gaming.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gaming.DAL.Configurations;
public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(64);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); 
    }
}

