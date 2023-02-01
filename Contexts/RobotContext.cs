﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using robot_controller_api.Models;

namespace robot_controller_api.Contexts
{
    public partial class RobotContext : DbContext
    {
        public virtual DbSet<Map> Maps { get; set; } = null!;
        public virtual DbSet<RobotCommand> RobotCommands { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

    public RobotContext()
        {
        }

    public RobotContext(DbContextOptions<RobotContext>
        options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=password;Database=sit331");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Map>(entity =>
            {
                entity.ToTable("map");

                entity.HasIndex(e => e.Name, "unique_name_map")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Columns).HasColumnName("columns");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_date");

                entity.Property(e => e.Description)
                    .HasMaxLength(800)
                    .HasColumnName("description");

                entity.Property(e => e.IsSquareMap)
                    .HasColumnName("is_square_map")
                    .HasComputedColumnSql("((rows > 0) AND (rows = columns))", true);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Rows).HasColumnName("rows");
            });

            modelBuilder.Entity<RobotCommand>(entity =>
            {
                entity.ToTable("robot_command");

                entity.HasIndex(e => e.Name, "unique_name_rb")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_date");

                entity.Property(e => e.Description)
                    .HasMaxLength(800)
                    .HasColumnName("description");

                entity.Property(e => e.IsMoveCommand).HasColumnName("is_move_command");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_date");

                entity.Property(e => e.Description)
                    .HasMaxLength(800)
                    .HasColumnName("description");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified_date");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(100)
                    .HasColumnName("password_hash");

                entity.Property(e => e.Role)
                    .HasMaxLength(800)
                    .HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
