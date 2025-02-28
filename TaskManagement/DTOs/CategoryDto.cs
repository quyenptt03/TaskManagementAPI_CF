﻿namespace TaskManagement.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
