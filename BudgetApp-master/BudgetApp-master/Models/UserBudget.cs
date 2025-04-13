using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class UserBudget
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
    }
}

//Varje användare har sin egna budget(antal pengar) + expenses 
//Användare delar inte budget eller expenses med varandra endast categories