using System;
using System.Collections.Generic;
using ERP.EntityModels.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Models.Context;

public partial class ERPDbContext : DbContext
{
    public ERPDbContext(DbContextOptions<ERPDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
