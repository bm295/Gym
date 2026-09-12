using Bunit;
using Bunit.JSInterop;
using Gym.WebUI.Shared;
using Gym.WebUI.Services.Ui;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Tests;

public sealed class AppShellUiTests : TestContext
{
    [Fact]
    public void Navigation_toggle_controls_the_collapsed_sidebar_state()
    {
        Services.AddSingleton<IUiLocalizer>(new UiLocalizer());
        var cut = RenderComponent<NavMenu>();
        Assert.Contains("app-navigation--collapsed", cut.Markup);
        cut.Find(".app-sidebar__toggle").Click();
        Assert.DoesNotContain("app-navigation--collapsed", cut.Markup);
    }

    [Fact]
    public void Navigation_rerenders_when_the_display_language_changes()
    {
        var localizer = new UiLocalizer();
        Services.AddSingleton<IUiLocalizer>(localizer);
        var cut = RenderComponent<NavMenu>();
        localizer.SetLanguage("en");
        cut.WaitForAssertion(() => Assert.Contains("Overview", cut.Markup));
    }

    [Fact]
    public void Top_bar_updates_branch_language_and_persists_both_preferences()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var shell = new AppShellState();
        var localizer = new UiLocalizer();
        Services.AddSingleton<IAppShellState>(shell);
        Services.AddSingleton<IUiLocalizer>(localizer);
        var cut = RenderComponent<AppTopBar>();
        cut.Find("#branch-context").Change(shell.Branches[1].Id.ToString());
        cut.Find("#language-selector").Change("en");
        Assert.Equal(shell.Branches[1], shell.SelectedBranch);
        Assert.Equal("en", localizer.Language);
        Assert.Contains(JSInterop.Invocations, call => call.Identifier == "sessionStorage.setItem" && call.Arguments.Contains("gymos.branch-id"));
        Assert.Contains(JSInterop.Invocations, call => call.Identifier == "sessionStorage.setItem" && call.Arguments.Contains("gymos.language"));
    }
}
