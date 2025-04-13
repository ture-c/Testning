namespace E2ETesting.Steps;

using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class SimpleFormSteps
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;

    [BeforeScenario]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 200 });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [AfterScenario]
    public async Task Teardown()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Given("I am on the simple form page")]
    public async Task GivenIAmOnTheSimpleFormPage()
    {
        await _page.GotoAsync("https://www.selenium.dev/selenium/web/web-form.html");
    }

    [When(@"I enter ""(.*)"" as the name")]
    public async Task WhenIEnterAsTheName(string name)
    {
        await _page.FillAsync("input[name='my-text']", name);
    }

    [When(@"I enter ""(.*)"" as the password")]
    public async Task WhenIEnterAsThePassword(string password)
    {
        await _page.FillAsync("input[name='my-password']", password);
    }

    [When(@"I select ""(.*)"" from the dropdown")]
    public async Task WhenISelectFromTheDropdown(string value)
    {
        await _page.SelectOptionAsync("select[name='my-select']", new SelectOptionValue { Label = value });
    }

    [When("I submit the form")]
    public async Task WhenISubmitTheForm()
    {
        await _page.ClickAsync("button");
    }

    [Then("I should see a confirmation message")]
    public async Task ThenIShouldSeeAConfirmationMessage()
    {
        // Wait for response message
        await _page.WaitForSelectorAsync("h1");
        var heading = await _page.InnerTextAsync("h1");
        Assert.Equal("Form submitted", heading);
    }
}
