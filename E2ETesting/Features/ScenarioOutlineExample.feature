Feature: Budget App
  As a user of the Budget App
  I want to log in and manage my expenses
  So that I can track my spending in different categories

Background:
  Given I am on the budget application page

Scenario: Adding and then deleting a Transport expense
  When I add an expense of "2000" with the description "Train"
  And I select 2 from the dropdown with the label "Transport"
  Then I should see the expense added with the description "Train" and amount "2000"
  When I click the delete button for the "Train" expense
  Then I should not see the "Train" expense in the list

Scenario: Adding a Utilities expense
  When I add an expense of "1000" with the description "Electric" 
  And I select 3 from the dropdown with the label "Utilities"
  Then I should see the expense added with the description "Electric" and amount "1000"

Scenario: Editing an existing expense
  When I add an expense of "1500" with the description "Groceries"
  And I select 1 from the dropdown with the label "Food"
  Then I should see the expense added with the description "Groceries" and amount "1500"
  When I click the edit button for the "Groceries" expense
  And I change the description to "Monthly Groceries" and amount to "1800"
  And I submit the edit form
  Then I should see the expense updated with the description "Monthly Groceries" and amount "1800"

Scenario: Deleting all budget and expenses
  When I click on "Delete Budget & Expenses"
  And I confirm the deletion
  Then I should see the budget is reset
  And I should see no expenses
