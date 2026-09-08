# Implement Member Check-in Feature

## Summary

Implement `docs/features/member-check-in.md` incrementally. Each scenario is
implemented with its automated test, verified independently, and kept green
before the next scenario is started.

## Incremental Implementation Sequence

### 1. Test and domain foundation

- [x] Create a dedicated `Gym.Tests` project and add it to `Gym.sln`.
- [ ] Add domain models for members, branches, subscriptions, plans, and check-ins.
- [ ] Add member/subscription status types and an injectable UTC clock.
- [ ] Add in-memory repositories and seed-data helpers.
- [ ] Verify the solution builds and the test project runs.

### 2. Scenario: eligible member with an unlimited subscription

- [ ] Implement the application check-in service and result type.
- [ ] Support member lookup by member code or normalized phone number.
- [ ] Validate tenant, active member, active subscription, date range, branch, and
  duplicate-window prerequisites.
- [ ] Create one check-in record with a UTC timestamp and log the operation.
- [ ] Add tests for the complete successful scenario.

### 3. Scenario: eligible member with a limited subscription

- [ ] Decrement `RemainingVisits` after successful validation.
- [ ] Add tests proving a visit is consumed exactly once and the check-in is saved.

### 4. Scenarios: inactive or suspended member

- [ ] Reject `Inactive` and `Suspended` members.
- [ ] Add scenario-outline tests proving no attendance or subscription mutation.

### 5. Scenarios: invalid subscription dates or status

- [ ] Reject missing active subscriptions.
- [ ] Reject subscriptions that have not started or have expired.
- [ ] Add tests proving rejected requests do not create check-ins or alter state.

### 6. Scenarios: branch access

- [ ] Reject check-ins outside the subscription home branch when cross-branch access
  is disabled.
- [ ] Allow permitted cross-branch check-ins.
- [ ] Add tests for both paths.

### 7. Scenario: no visits remaining

- [ ] Reject limited subscriptions with zero remaining visits.
- [ ] Add a non-negative invariant for the visit counter and corresponding tests.

### 8. Scenarios: duplicate check-in window

- [ ] Reject a check-in at the same branch within five minutes.
- [ ] Allow a check-in after the five-minute window.
- [ ] Use the fake clock to test the four-minute and six-minute examples.

### 9. Scenario: concurrent limited-plan check-ins

- [ ] Make validation, attendance creation, and visit decrement atomic within the
  in-memory repository/service lock.
- [ ] Add a concurrency test proving only one of two simultaneous requests succeeds
  when one visit remains.

### 10. Tenant isolation and authorization

- [ ] Ensure member lookup and check-in operations are scoped by tenant.
- [ ] Use the existing `tenant_id`, `branch_id`, and staff-role claims.
- [ ] Add tests proving cross-tenant members are not returned and unauthorized
  branches cannot be used.

### 11. HTTP API integration

Expose the secured endpoint:

```text
POST /api/tenants/{tenantId}/branches/{branchId}/check-ins
```

- [ ] Accept member code or phone number.
- [ ] Derive authorization from authenticated claims rather than trusting client
  tenant or branch identity.
- [ ] Map application results to consistent HTTP responses.
- [ ] Add API authorization, success, validation, duplicate, and rejection tests.

### 12. Blazor receptionist UI

- [ ] Add the `/check-in` page.
- [ ] Support member search, branch selection, confirmation, and result messages.
- [ ] Add component tests where the selected Blazor test tooling supports them.

### 13. Final verification

- [ ] Run all unit, concurrency, API, and UI tests.
- [ ] Run the existing build and GitHub Actions workflow.
- [ ] Confirm coverage output includes the newly implemented scenarios.

## Assumptions and Defaults

- Persistence is intentionally in-memory for this iteration; no EF Core or database migration is added.
- The existing `tenant_id` and `branch_id` claims remain the authorization source.
- `TenantAdmin` can check in at any branch in the tenant; other staff require a matching branch claim.
- The duplicate window is five minutes, matching the feature document.
- Check-in timestamps are stored as UTC `DateTimeOffset`.
- Official attendance records are created only for successful check-ins.
- Existing payment and unrelated demo features remain unchanged.
- The implementation order follows the scenarios in `docs/features/member-check-in.md`.
- No later scenario is started until the current scenario's tests and build pass.
