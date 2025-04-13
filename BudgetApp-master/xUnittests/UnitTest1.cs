using BudgetApp.Data;
using BudgetApp.Models;
using BudgetApp.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
public class ExpenseModelTests
{
    [Fact]
    public void Expense_ShouldHaveCorrectProperties()
    {
        //DB
        var expense = new Expense
        {
            Id = 1,
            Name = "Groceries",
            Amount = 1500,
            CategoryId = 1,
            UserId = "user-123"
        };
        Console.WriteLine($"Expense created with Name: {expense.Name}, Amount: {expense.Amount}");


        // Assert
        Assert.Equal(1, expense.Id);
        Assert.Equal("Groceries", expense.Name);
        Assert.Equal(1500, expense.Amount);
        Assert.Equal(1, expense.CategoryId);
        Assert.Equal("user-123", expense.UserId);

        Console.WriteLine("Test completed successfully");
    }

    [Fact]
    public void Expense_WithCategoryRelationship_ShouldWorkCorrectly()
    {
        // Arrange
        var category = new Category
        {
            Id = 2,
            Name = "Transport"
        };

        var expense = new Expense
        {
            Id = 5,
            Name = "Train Ticket",
            Amount = 2000,
            CategoryId = category.Id,
            UserId = "test",
            Category = category 
        };

        Console.WriteLine($"Category: {category.Name} (ID: {category.Id})");
        Console.WriteLine($"Expense: {expense.Name} (Amount: {expense.Amount}, CategoryId: {expense.CategoryId})");

        // Assert
        Assert.Equal(2, expense.CategoryId);
        Assert.Same(category, expense.Category);
        Assert.Equal("Transport", expense.Category.Name);

        // Ser om beloppet är större än 0
        Assert.True(expense.Amount > 0, "Expense amount should be positive");

        Console.WriteLine("Test completed successfully");

    }
    [Fact]
    public void MainpageModel_GetExpensesByCategory_CalculatesCorrectTotals()
    {
        // Arrange
        var mainpageModel = new MainpageModel(null, null); 

        var foodCategory = new Category { Id = 1, Name = "Food" };
        var transportCategory = new Category { Id = 2, Name = "Transport" };

        mainpageModel.Expenses = new List<Expense>
    {
        new Expense { Id = 1, Name = "Groceries", Amount = 1500, Category = foodCategory },
        new Expense { Id = 2, Name = "Lunch", Amount = 500, Category = foodCategory },
        new Expense { Id = 3, Name = "Train", Amount = 600, Category = transportCategory },
        new Expense { Id = 4, Name = "Taxi", Amount = 400, Category = transportCategory }
    };
        Console.WriteLine($"Created {mainpageModel.Expenses.Count} expenses across {2} categories");
        foreach (var expense in mainpageModel.Expenses)
        {
            Console.WriteLine($" {expense.Name}: ${expense.Amount} (Category: {expense.Category.Name})");
        }
        // Act
        var categorySummary = mainpageModel.GetExpensesByCategory();
        var totalExpenses = mainpageModel.Expenses.Sum(e => e.Amount);

        Console.WriteLine("Results:");
        Console.WriteLine($"Total categories found: {categorySummary.Count}");
        
        foreach (var category in categorySummary)
        {
            Console.WriteLine($"- {category.Key}: ${category.Value}");
        }
        Console.WriteLine($"Total expenses: ${totalExpenses}");
        Console.WriteLine($"75% threshold: ${totalExpenses * 0.75m}");
        // Assert
        Assert.Equal(2, categorySummary.Count);

        Assert.Equal(2000, categorySummary["Food"]);
        Assert.Equal(1000, categorySummary["Transport"]);

        Assert.Equal(3000, totalExpenses);

        Assert.Equal(totalExpenses, categorySummary.Values.Sum());
        //Ser om ingen kategori överstiger 75% av totalbeloppet
        foreach (var categoryTotal in categorySummary.Values)
        {
            Assert.True(categoryTotal <= totalExpenses * 0.75m,
                "No single category should exceed 75% of total expenses");
        }
    }


}