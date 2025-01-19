using System;
using Gaming.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gaming.DAL.Context;

public class AppDbContext : IdentityDbContext<User>
{
	public DbSet<Game> Games { get; set; }
	public DbSet<Category> Categories { get; set; }

    public AppDbContext(DbContextOptions opt) : base(opt) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
        base.OnModelCreating(builder);
    }
}

