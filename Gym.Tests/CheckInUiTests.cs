using Bunit;
using Gym.WebUI.Pages;
using Gym.WebUI.Services.Prototype;
using Gym.WebUI.Services.Ui;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Tests;

public sealed class CheckInUiTests : TestContext
{
    [Theory]
    [InlineData("MEM-1001", "Check-in successful")]
    [InlineData("MEM-1002", "Check-in successful")]
    [InlineData("MEM-1003", "Check-in not allowed")]
    [InlineData("MEM-1004", "Check-in not allowed")]
    [InlineData("MEM-1005", "Check-in not allowed")]
    [InlineData("MEM-1006", "Check-in not allowed")]
    public void Check_in_displays_the_expected_prototype_outcome(string memberCode, string expectedHeading)
    {
        Services.AddSingleton<IPrototypeGymState, PrototypeGymState>();
        Services.AddSingleton<IAppShellState, AppShellState>();
        var cut = RenderComponent<CheckIn>();

        cut.Find("#member-lookup").Change(memberCode);
        cut.Find("form").Submit();
        cut.FindAll(".ui-button--primary").Last().Click();

        Assert.Contains(expectedHeading, cut.Markup);
    }

    [Fact]
    public void Unknown_member_shows_an_actionable_lookup_error()
    {
        Services.AddSingleton<IPrototypeGymState, PrototypeGymState>();
        Services.AddSingleton<IAppShellState, AppShellState>();
        var cut = RenderComponent<CheckIn>();

        cut.Find("#member-lookup").Change("UNKNOWN");
        cut.Find("form").Submit();

        Assert.Contains("No member matches", cut.Markup);
    }

    [Fact]
    public void Check_in_uses_the_selected_branch_context_for_a_wrong_branch_rejection()
    {
        var state = new PrototypeGymState();
        var shell = new AppShellState();
        shell.SelectBranch(shell.Branches[1].Id);
        Services.AddSingleton<IPrototypeGymState>(state);
        Services.AddSingleton<IAppShellState>(shell);
        var cut = RenderComponent<CheckIn>();

        cut.Find("#member-lookup").Change("MEM-1001");
        cut.Find("form").Submit();
        cut.FindAll(".ui-button--primary").Last().Click();

        Assert.Contains("Check-in not allowed", cut.Markup);
        Assert.Contains("selected branch", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }
}
