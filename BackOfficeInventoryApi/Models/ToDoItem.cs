﻿namespace BackOfficeInventoryApi.Models
{
    public class ToDoItem
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public bool IsComplete { get; set; }

        public string? Secret { get; set; }
    }
}
