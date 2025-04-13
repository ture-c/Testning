using BudgetApp.Models;

namespace BudgetApp.Data
{
    public class SampleData
    {
        public static void SeedData(ApplicationDbContext database)
        {
            if (!database.Categories.Any())
            {
                database.Categories.AddRange(
                    new List<Category>
                    {
                        new Category { Name = "Food" },
                        new Category { Name = "Transport" },
                        new Category { Name = "Utilities" },
                        new Category { Name = "Entertainment" },
                        new Category { Name = "Other" },
                    }
                );

                database.SaveChanges();
            }
        }
    }
}
