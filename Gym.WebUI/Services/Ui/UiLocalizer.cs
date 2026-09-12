namespace Gym.WebUI.Services.Ui;

public sealed class UiLocalizer : IUiLocalizer
{
    private static readonly IReadOnlyDictionary<string, (string Vietnamese, string English)> Resources =
        new Dictionary<string, (string Vietnamese, string English)>
        {
            ["Nav.Overview"] = ("Tổng quan", "Overview"),
            ["Nav.Dashboard"] = ("Bảng điều khiển", "Dashboard"),
            ["Nav.Members"] = ("Hội viên", "Members"),
            ["Nav.PlansSubscriptions"] = ("Gói tập & đăng ký", "Plans & subscriptions"),
            ["Nav.MembershipPlans"] = ("Gói tập", "Membership plans"),
            ["Nav.Subscriptions"] = ("Đăng ký", "Subscriptions"),
            ["Nav.FrontDesk"] = ("Quầy lễ tân", "Front desk"),
            ["Nav.CheckIn"] = ("Check-in", "Check-in"),
            ["Nav.Attendance"] = ("Lịch sử điểm danh", "Attendance"),
            ["Nav.Finance"] = ("Tài chính", "Finance"),
            ["Nav.Payments"] = ("Thanh toán", "Payments"),
            ["Nav.Administration"] = ("Quản trị", "Administration"),
            ["Nav.Branches"] = ("Chi nhánh", "Branches"),
            ["Nav.Staff"] = ("Nhân sự", "Staff"),
            ["Nav.Reports"] = ("Báo cáo", "Reports"),
            ["Nav.AuditLog"] = ("Nhật ký thay đổi", "Audit log"),
            ["TopBar.Branch"] = ("Chi nhánh đang chọn", "Selected branch"),
            ["TopBar.Search"] = ("Tìm hội viên, thanh toán, hoặc đăng ký", "Search members, payments, or subscriptions"),
            ["TopBar.Notifications"] = ("Thông báo", "Notifications"),
            ["TopBar.Language"] = ("Ngôn ngữ hiển thị", "Display language"),
            ["TopBar.Profile"] = ("Hồ sơ nhân viên", "Staff profile"),
            ["TopBar.QuickSearch"] = ("Tìm kiếm nhanh", "Quick search"),
            ["Status.SystemReady"] = ("Hệ thống sẵn sàng", "System ready"),
            ["Help.StartTyping"] = ("Bắt đầu nhập để tìm trong chi nhánh đang chọn.", "Start typing to search across the selected branch."),
            ["Message.NoResults"] = ("Chưa có kết quả mẫu", "No prototype results yet"),
            ["Role.Receptionist"] = ("Lễ tân", "Receptionist"),
            ["Action.ProfileSettings"] = ("Cài đặt hồ sơ", "Profile settings"),
            ["Action.SignOut"] = ("Đăng xuất", "Sign out")
        };

    public event Action? Changed;
    public string Language { get; private set; } = "vi";
    public string this[string key] => Resources.TryGetValue(key, out var value)
        ? Language == "vi" ? value.Vietnamese : value.English
        : key;

    public void SetLanguage(string language)
    {
        var normalized = language == "en" ? "en" : "vi";
        if (Language == normalized)
        {
            return;
        }

        Language = normalized;
        Changed?.Invoke();
    }
}
