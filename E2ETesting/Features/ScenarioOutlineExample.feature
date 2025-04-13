Feature: Simple form submission

Scenario: Submit form
    Given I am on the simple form page
    When I enter "Alice" as the name
    And I enter "pass123" as the password
    And I select "One" from the dropdown
    And I submit the form
    Then I should see a confirmation message

    
Scenario Outline: Submit form with different names and dropdown values
    Given I am on the simple form page
    When I enter "<name>" as the name
    And I enter "<password>" as the password
    And I select "<dropdown>" from the dropdown
    And I submit the form
    Then I should see a confirmation message

    Examples:
      | name    | password  | dropdown |
      | Alice   | pass123   | One      |
      | Bob     | secret456 | Two      |