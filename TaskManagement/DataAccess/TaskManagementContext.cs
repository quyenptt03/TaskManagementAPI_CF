using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;
using Task = TaskManagement.Models.Task;

namespace TaskManagement.DataAccess
{
    public class TaskManagementContext:DbContext
    {
        public TaskManagementContext()
        {

        }
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public DbSet<TaskLabel> TaskLabels { get; set; }

        public DbSet<TaskComment> TaskComments { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Tasks)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Category).WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.User).WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TaskLabel>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.LabelId });

                entity.HasOne(d => d.Task).WithMany(p => p.TaskLabels)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Label).WithMany(p => p.TaskLabels)
                .HasForeignKey(d => d.LabelId)
                .OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<TaskComment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Task).WithMany(p => p.TaskComments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User).WithMany(p => p.TaskComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Username).IsUnique();

                entity.HasIndex(e => e.Email).IsUnique();

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
