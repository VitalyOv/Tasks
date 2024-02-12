using Microsoft.EntityFrameworkCore;
using Task = Domain.Entities.Task;

namespace Domain.DatabaseContext;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Task> Tasks { get; set; }
}