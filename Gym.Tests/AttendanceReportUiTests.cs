using Bunit;
using Gym.WebUI.Pages;
using Gym.WebUI.Services.Prototype;
using Gym.WebUI.Services.Ui;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Tests;

public sealed class AttendanceReportUiTests : TestContext
{
    [Fact]
    public void Filters_period_and_refreshes_on_branch_change()
    {
        var state = new PrototypeGymState();
        var shell = new AppShellState();
        Services.AddSingleton<IPrototypeGymState>(state);
        Services.AddSingleton<IAppShellState>(shell);
        var cut = RenderComponent<AttendanceReport>();
        var date = TimeZoneInfo.ConvertTime(state.CheckIns[0].CheckedInAt,
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh")).ToString("yyyy-MM-dd");
        cut.Find("#attendance-start").Change(date);
        cut.Find("#attendance-end").Change(date);
        cut.Find("form").Submit();
        Assert.Contains("1 calendar days", cut.Markup);
        Assert.DoesNotContain("No check-ins in this period", cut.Markup);
        shell.SelectBranch(shell.Branches[1].Id);
        cut.WaitForAssertion(() => Assert.Contains("No check-ins in this period", cut.Markup));
        Assert.Contains(shell.SelectedBranch.Name, cut.Markup);
    }

    [Fact]
    public void Invalid_period_shows_error_and_hides_old_results()
    {
        Services.AddSingleton<IPrototypeGymState>(new PrototypeGymState());
        Services.AddSingleton<IAppShellState>(new AppShellState());
        var cut = RenderComponent<AttendanceReport>();
        cut.Find("#attendance-start").Change("2026-10-10");
        cut.Find("#attendance-end").Change("2026-10-01");
        cut.Find("form").Submit();
        Assert.Contains("Select between 1 and 366 calendar days", cut.Find("[role=alert]").TextContent);
        Assert.Empty(cut.FindAll(".dashboard-metrics"));
    }
}
