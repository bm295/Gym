using Gym.Domain;
using Gym.Infrastructure.Repositories;

namespace Gym.Infrastructure.SeedData;

/// <summary>
/// Stable, non-production demo records for the in-memory development store.
/// </summary>
public static class InMemoryGymDataSeeder
{
    public static readonly Guid AtlasFitnessTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid PulseWellnessTenantId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid AtlasDistrictOneBranchId = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid AtlasThuDucBranchId = Guid.Parse("11111111-1111-1111-1111-111111111102");
    public static readonly Guid PulseHaiChauBranchId = Guid.Parse("22222222-2222-2222-2222-222222222201");

    public static void Seed(InMemoryGymDataStore store)
    {
        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);
        var atlasAdminId = Guid.Parse("11111111-1111-1111-1111-111111111201");
        var atlasReceptionistId = Guid.Parse("11111111-1111-1111-1111-111111111202");
        var pulseManagerId = Guid.Parse("22222222-2222-2222-2222-222222222301");
        var atlasMemberId = Guid.Parse("11111111-1111-1111-1111-111111111301");
        var pulseMemberId = Guid.Parse("22222222-2222-2222-2222-222222222401");
        var atlasPlanId = Guid.Parse("11111111-1111-1111-1111-111111111401");
        var pulsePlanId = Guid.Parse("22222222-2222-2222-2222-222222222501");
        var atlasSubscriptionId = Guid.Parse("11111111-1111-1111-1111-111111111501");
        var pulseSubscriptionId = Guid.Parse("22222222-2222-2222-2222-222222222601");

        store.Add(new Tenant { Id = AtlasFitnessTenantId, Code = "ATLAS", Name = "Atlas Fitness", Status = "Active", Timezone = "Asia/Ho_Chi_Minh", CreatedAt = now, UpdatedAt = now });
        store.Add(new Tenant { Id = PulseWellnessTenantId, Code = "PULSE", Name = "Pulse Wellness", Status = "Active", Timezone = "Asia/Ho_Chi_Minh", CreatedAt = now, UpdatedAt = now });

        store.Add(new Branch { Id = AtlasDistrictOneBranchId, TenantId = AtlasFitnessTenantId, BranchCode = "ATLAS-D1", Name = "Atlas District 1", Address = "District 1, Ho Chi Minh City", Status = "Active", CreatedAt = now, UpdatedAt = now });
        store.Add(new Branch { Id = AtlasThuDucBranchId, TenantId = AtlasFitnessTenantId, BranchCode = "ATLAS-TD", Name = "Atlas Thu Duc", Address = "Thu Duc City, Ho Chi Minh City", Status = "Active", CreatedAt = now, UpdatedAt = now });
        store.Add(new Branch { Id = PulseHaiChauBranchId, TenantId = PulseWellnessTenantId, BranchCode = "PULSE-HC", Name = "Pulse Hai Chau", Address = "Hai Chau, Da Nang", Status = "Active", CreatedAt = now, UpdatedAt = now });

        store.Add(new StaffUser { Id = atlasAdminId, TenantId = AtlasFitnessTenantId, Username = "atlas.admin", Email = "admin@atlas.demo", PasswordHash = "demo-only", FullName = "An Nguyen", Role = "TenantAdmin", Status = "Active", CreatedAt = now, UpdatedAt = now });
        store.Add(new StaffUser { Id = atlasReceptionistId, TenantId = AtlasFitnessTenantId, Username = "atlas.reception", Email = "reception@atlas.demo", PasswordHash = "demo-only", FullName = "Binh Tran", Role = "Receptionist", Status = "Active", CreatedAt = now, UpdatedAt = now });
        store.Add(new StaffUser { Id = pulseManagerId, TenantId = PulseWellnessTenantId, Username = "pulse.manager", Email = "manager@pulse.demo", PasswordHash = "demo-only", FullName = "Chi Le", Role = "BranchManager", Status = "Active", CreatedAt = now, UpdatedAt = now });
        store.Add(new StaffBranchAccessAssignment { TenantId = AtlasFitnessTenantId, StaffUserId = atlasReceptionistId, BranchId = AtlasDistrictOneBranchId });
        store.Add(new StaffBranchAccessAssignment { TenantId = PulseWellnessTenantId, StaffUserId = pulseManagerId, BranchId = PulseHaiChauBranchId });

        store.Add(new MembershipPlan { Id = atlasPlanId, TenantId = AtlasFitnessTenantId, PlanCode = "ATLAS-UNL-30", Name = "Unlimited 30 Days", DurationDays = 30, VisitLimit = null, DefaultPrice = 699000m, Currency = "VND", AllowCrossBranch = true, Status = "Active", CreatedAt = now, UpdatedAt = now });
        store.Add(new MembershipPlan { Id = pulsePlanId, TenantId = PulseWellnessTenantId, PlanCode = "PULSE-12", Name = "12 Visit Pack", DurationDays = 30, VisitLimit = 12, DefaultPrice = 450000m, Currency = "VND", AllowCrossBranch = false, Status = "Active", CreatedAt = now, UpdatedAt = now });

        store.Add(new Member { Id = atlasMemberId, TenantId = AtlasFitnessTenantId, MemberCode = "ATLAS-1001", FullName = "Minh Pham", PhoneNumber = "0901001001", NormalizedPhoneNumber = "0901001001", HomeBranchId = AtlasDistrictOneBranchId, Status = MemberStatus.Active, CreatedAt = now, UpdatedAt = now });
        store.Add(new Member { Id = pulseMemberId, TenantId = PulseWellnessTenantId, MemberCode = "PULSE-1001", FullName = "Hoa Vo", PhoneNumber = "0902002002", NormalizedPhoneNumber = "0902002002", HomeBranchId = PulseHaiChauBranchId, Status = MemberStatus.Active, CreatedAt = now, UpdatedAt = now });
        store.Add(new Subscription { Id = atlasSubscriptionId, TenantId = AtlasFitnessTenantId, MemberId = atlasMemberId, MemberPlanId = atlasPlanId, HomeBranchId = AtlasDistrictOneBranchId, Status = SubscriptionStatus.Active, StartDate = today.AddDays(-5), EndDate = today.AddDays(25), SalePrice = 699000m, AmountPaid = 699000m, Currency = "VND", RemainingVisits = null, AllowCrossBranchSnapshot = true, PlanNameSnapshot = "Unlimited 30 Days", DurationDaysSnapshot = 30, VisitLimitSnapshot = null, OriginalPlanPriceSnapshot = 699000m, CreatedAt = now, UpdatedAt = now });
        store.Add(new Subscription { Id = pulseSubscriptionId, TenantId = PulseWellnessTenantId, MemberId = pulseMemberId, MemberPlanId = pulsePlanId, HomeBranchId = PulseHaiChauBranchId, Status = SubscriptionStatus.Active, StartDate = today.AddDays(-3), EndDate = today.AddDays(27), SalePrice = 450000m, AmountPaid = 450000m, Currency = "VND", RemainingVisits = 11, AllowCrossBranchSnapshot = false, PlanNameSnapshot = "12 Visit Pack", DurationDaysSnapshot = 30, VisitLimitSnapshot = 12, OriginalPlanPriceSnapshot = 450000m, CreatedAt = now, UpdatedAt = now });

        store.Add(new Payment { Id = Guid.Parse("11111111-1111-1111-1111-111111111601"), TenantId = AtlasFitnessTenantId, BranchId = AtlasDistrictOneBranchId });
        store.Add(new Payment { Id = Guid.Parse("22222222-2222-2222-2222-222222222701"), TenantId = PulseWellnessTenantId, BranchId = PulseHaiChauBranchId });
        store.Add(new CheckIn { Id = Guid.Parse("11111111-1111-1111-1111-111111111701"), TenantId = AtlasFitnessTenantId, MemberId = atlasMemberId, SubscriptionId = atlasSubscriptionId, BranchId = AtlasDistrictOneBranchId, CheckedInAt = now.AddHours(-1), CreatedBy = atlasReceptionistId, Note = "Demo check-in" });
        store.Add(new AuditEntry { Id = Guid.Parse("11111111-1111-1111-1111-111111111801"), TenantId = AtlasFitnessTenantId, BranchId = AtlasDistrictOneBranchId, StaffUserId = atlasReceptionistId, Action = "CheckIn", EntityType = "Member", EntityId = atlasMemberId, OccurredAt = now.AddHours(-1), Detail = "Demo check-in" });
        store.Add(new AuditEntry { Id = Guid.Parse("22222222-2222-2222-2222-222222222801"), TenantId = PulseWellnessTenantId, BranchId = PulseHaiChauBranchId, StaffUserId = pulseManagerId, Action = "SubscriptionSale", EntityType = "Subscription", EntityId = pulseSubscriptionId, OccurredAt = now.AddHours(-2), Detail = "Demo subscription sale" });
    }
}
