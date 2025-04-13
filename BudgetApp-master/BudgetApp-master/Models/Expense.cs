using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        
    }
}
