using BudgetApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Expense> Expenses { get; set; } // DbSet sparas i databasen
    public DbSet<Category> Categories { get; set; }

    public DbSet<UserBudget> UserBudgets { get; set; } //Skapade en ny DbSet för UserBudget
}
