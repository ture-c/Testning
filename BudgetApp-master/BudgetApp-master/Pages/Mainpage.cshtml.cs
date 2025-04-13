using BudgetApp.Data;
using BudgetApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BudgetApp.Pages
{
    public class MainpageModel : PageModel
    {
        //Database context and sign-in manager
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public MainpageModel(ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // Properties för att ladda data från db och visa i vyn 
        public List<Category> Categories { get; set; } = new();
        public List<Expense> Expenses { get; set; } = new();

        [BindProperty]
        public decimal Budget { get; set; }
        public decimal TotalExpenses { get; set; }

        [BindProperty]
        public Expense EditExpense { get; set; } = new();

        public Dictionary<string, decimal> GetExpensesByCategory()
        {
            return Expenses
                .GroupBy(e => e.Category.Name)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }



        public async Task OnGetAsync()
        {
            // Laddar kategorier för dropdown.
            Categories = await _context.Categories.ToListAsync();

            // Skaffa nuvarande user id
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Ladda expenses från databasen
            Expenses = await _context.Expenses
                                     .Where(e => e.UserId == userId)
                                     .Include(e => e.Category)
                                     .ToListAsync();

            // kalkulerar totalbelopp för expenses
            TotalExpenses = Expenses.Sum(e => e.Amount);

            // Ladda userbudget från databasen
            var userBudget = await _context.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (userBudget != null)
            {
                Budget = userBudget.Amount;
            }
        }

        // Metod för att sätta budget
        public async Task<IActionResult> OnPostSetBudgetAsync(decimal budget)
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }
            
            var userBudget = await _context.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (userBudget == null)
            {
                userBudget = new UserBudget { UserId = userId, Amount = budget };
                _context.UserBudgets.Add(userBudget);
            }
            else
            {
                userBudget.Amount = budget;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
        // Metod för att lägga till en ny utgift
        public async Task<IActionResult> OnPostAddExpenseAsync(string expenseName, decimal expenseAmount, int expenseCategory)
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Laddar kategorier för dropdown
            Categories = await _context.Categories.ToListAsync();

            // Validera att alla fält är ifyllda och att beloppet är större än 0
            if (string.IsNullOrEmpty(expenseName) || expenseAmount <= 0)
            {
                ModelState.AddModelError(string.Empty, "Alla fält måste vara ifyllda och beloppet måste vara större än 0.");
                Expenses = await _context.Expenses.Where(e => e.UserId == userId)
                                                  .Include(e => e.Category)
                                                  .ToListAsync();
                TotalExpenses = Expenses.Sum(e => e.Amount);
                return Page();
            }

            // Validera att kategorin finns
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == expenseCategory);
            if (category == null)
            {
                return NotFound();
            }

            // Skapa en ny utgift och spara i databasen
            var expense = new Expense
            {
                Name = expenseName,
                Amount = expenseAmount,
                Date = DateTime.Now,
                CategoryId = category.Id,
                UserId = userId
            };
            // Lägger till utgiften i databasen
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
        //Metod för att ta bort en utgift
        public async Task<IActionResult> OnPostDeleteExpenseAsync(int expenseId)
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetEditExpenseAsync(int expenseId)
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Ladda kategorier och utgifter
            Categories = await _context.Categories.ToListAsync();
            Expenses = await _context.Expenses
                                    .Where(e => e.UserId == userId)
                                    .Include(e => e.Category)
                                    .ToListAsync();

            //Gör så att budget inte tappar värdet när man trycker edit
            var userBudget = await _context.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (userBudget != null)
            {
                Budget = userBudget.Amount;
            }

            // Hämta den expense som ska redigeras
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId);
            if (expense == null)
            {
                return NotFound();
            }



            EditExpense = expense;
            return Page();
        }

        //Metod för att uppdatera expense
        public async Task<IActionResult> OnPostUpdateExpenseAsync()
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Kontrollera att vi har en giltig expense
            if (EditExpense == null || string.IsNullOrWhiteSpace(EditExpense.Name))
            {
                ModelState.AddModelError(string.Empty, "Expense Title cannot be empty.");
                Categories = await _context.Categories.ToListAsync();
                Expenses = await _context.Expenses.Where(e => e.UserId == userId).Include(e => e.Category).ToListAsync();
                return Page();
            }

            // Hitta den befintliga expense-posten
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == EditExpense.Id && e.UserId == userId);
            if (expense == null)
            {
                return NotFound();
            }

            // Uppdatera utgiften
            expense.Name = EditExpense.Name;
            expense.Amount = EditExpense.Amount;
            expense.CategoryId = EditExpense.CategoryId;
            //expense.Date = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        //Tar bort lagd budget och tillagda expenses
        public async Task<IActionResult> OnPostDeleteAllAsync()
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Ta bort alla expenses för användaren
            var userExpenses = _context.Expenses.Where(e => e.UserId == userId);
            _context.Expenses.RemoveRange(userExpenses);

            // Ta bort användarens budget
            var userBudget = await _context.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (userBudget != null)
            {
                _context.UserBudgets.Remove(userBudget);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }





    }
}
    