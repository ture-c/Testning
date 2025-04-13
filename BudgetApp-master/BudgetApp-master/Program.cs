using BudgetApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   
    //dbContext.Database.Migrate();

    SampleData.SeedData(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/api/budget", async (HttpContext context, [FromServices] ApplicationDbContext dbContext, [FromServices] SignInManager<IdentityUser> signInManager) =>
{
    var userId = signInManager.UserManager.GetUserId(context.User);
    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    // Hämta användarens budget
    var userBudget = await dbContext.UserBudgets.FirstOrDefaultAsync(b => b.UserId == userId);

    // Hämta användarens utgifter
    var expenses = await dbContext.Expenses
        .Where(e => e.UserId == userId)
        .Include(e => e.Category)  // Se till att kategori är inkluderat
        .ToListAsync();

    // Skaffa tot budget 
    var budgetAmount = userBudget?.Amount ?? 0;

    // SKaffa total exp
    var totalExpenses = expenses.Sum(e => e.Amount);

    // Grupera expenses - transaction script
    var categoryExpenses = expenses
        .GroupBy(e => e.Category.Name)  // Gruppera efter namn
        .Select(g => new
        {
            Name = g.Key,
            Amount = g.Sum(e => e.Amount)
        })
        .ToList();

    // Returnera budget amount, total exp och category
    return Results.Json(new
    {
        totalBudget = budgetAmount,
        totalExpenses,
        categories = categoryExpenses  
    });
});



app.Run();
