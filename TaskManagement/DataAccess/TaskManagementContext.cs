using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;
using Task = TaskManagement.Models.Task;

namespace TaskManagement.DataAccess
{
    public class TaskManagementContext: IdentityDbContext<User, IdentityRole<int>, int>
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
        public DbSet<TaskAttachment> TaskAttachment { get; set; }

        public new DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Tasks)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.Username).IsUnique();

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
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User).WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(t => t.Attachments)
                    .WithOne(a => a.Task)
                    .HasForeignKey(a => a.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

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

            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    //entity.HasIndex(e => e.Username).IsUnique();

            //    entity.HasIndex(e => e.Email).IsUnique();

            //});

            //modelBuilder.Entity<IdentityRole<int>>().HasData(
            //    new IdentityRole<int>
            //    {
            //        Id = 1,
            //        Name = "Admin",
            //        NormalizedName = "ADMIN"
            //    },
            //    new IdentityRole<int>
            //    {
            //        Id = 2,
            //        Name = "User",
            //        NormalizedName = "USER"
            //    });

        }
    }
}
