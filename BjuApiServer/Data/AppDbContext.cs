using Microsoft.EntityFrameworkCore;
using BjuApiServer.Models;

namespace BjuApiServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();

}
