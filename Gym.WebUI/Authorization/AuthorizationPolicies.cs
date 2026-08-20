namespace Gym.WebUI.Authorization;

public static class AuthorizationPolicies
{
    public const string CanVoidPayment = nameof(CanVoidPayment);
}

public static class StaffRoles
{
    public const string TenantAdmin = nameof(TenantAdmin);
    public const string BranchManager = nameof(BranchManager);
    public const string Receptionist = nameof(Receptionist);
    public const string Trainer = nameof(Trainer);
}
