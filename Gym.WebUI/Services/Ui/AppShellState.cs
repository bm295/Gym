namespace Gym.WebUI.Services.Ui;

public sealed class AppShellState : IAppShellState
{
    private readonly List<BranchContext> _branches =
    [
        new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "HCM-01", "District 1"),
        new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "HCM-02", "Thao Dien")
    ];

    public IReadOnlyList<BranchContext> Branches => _branches;
    public BranchContext SelectedBranch { get; private set; }
    public event Action? Changed;
    public StaffContext CurrentStaff { get; } = new("Linh Nguyen", "Receptionist", "LN");
    public IReadOnlyList<AppNotification> Notifications { get; } =
    [
        new("3 memberships expire soon", "Review subscriptions ending this week.", true),
        new("Daily report is ready", "Revenue and attendance figures were updated.", false)
    ];

    public AppShellState() => SelectedBranch = _branches[0];

    public void SelectBranch(Guid branchId)
    {
        var selected = _branches.SingleOrDefault(branch => branch.Id == branchId);
        if (selected is not null && selected != SelectedBranch)
        {
            SelectedBranch = selected;
            Changed?.Invoke();
        }
    }
}
