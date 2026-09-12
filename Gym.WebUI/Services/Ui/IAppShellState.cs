namespace Gym.WebUI.Services.Ui;

public interface IAppShellState
{
    IReadOnlyList<BranchContext> Branches { get; }
    BranchContext SelectedBranch { get; }
    event Action? Changed;
    StaffContext CurrentStaff { get; }
    IReadOnlyList<AppNotification> Notifications { get; }
    void SelectBranch(Guid branchId);
}

public sealed record BranchContext(Guid Id, string Code, string Name);
public sealed record StaffContext(string Name, string Role, string Initials);
public sealed record AppNotification(string Title, string Description, bool IsUnread);
