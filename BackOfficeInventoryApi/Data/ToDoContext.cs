using BackOfficeInventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeInventoryApi.Data
{
    public class ToDoContext:DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {

        }

        public DbSet<ToDoItem> toDoItems { get; set; } = null!;

        public DbSet<Products> Products { get; set; } = null!;
    }
}
