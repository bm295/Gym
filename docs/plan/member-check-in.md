# Implement Member Check-in Feature

## Summary

Implement `docs/features/member-check-in.md` as end-to-end vertical slices.
After every scenario below is complete, the receptionist can exercise that
scenario through the Blazor UI and the secured HTTP API, and its application,
API, and UI tests are green.

## Delivery Rule

Do not mark a scenario complete until all of these are done:

- application behavior and unit tests;
- secured API request/response mapping and API tests;
- Blazor receptionist flow and component/UI tests where supported;
- a manual smoke check of the UI calling the API.

## 1. Shared foundation (not a user-facing scenario)

- [x] Create a dedicated `Gym.Tests` project and add it to `Gym.sln`.
- [x] Add domain models for members, branches, subscriptions, plans, and check-ins.
- [x] Add member/subscription status types and an injectable UTC clock.
- [x] Add in-memory repositories and seed-data helpers.
- [x] Add check-in request, result, rejection-reason, and service contracts.
- [x] Verify the solution builds and the test project runs.
- [x] Add the `/check-in` page shell, an authenticated API client, and a shared
  request/response contract so the first scenario can be wired end-to-end.

## 2. Scenario: eligible member with an unlimited subscription

Deliver the first complete receptionist flow.

- [ ] Implement `CheckInService` lookup by member code or normalized phone number.
- [ ] Validate tenant, active member, active subscription, current date, home branch,
  and no recent duplicate check-in.
- [ ] Create one check-in record with a UTC timestamp without changing an unlimited
  subscription's visit count; log the successful operation.
- [ ] Expose `POST /api/tenants/{tenantId}/branches/{branchId}/check-ins`, secured
  with the existing tenant/branch authorization rules.
- [ ] Add the `/check-in` UI: member lookup, selected branch, confirmation, and a
  success result.
- [ ] Add application, API, and UI tests for the successful unlimited-plan flow.
- [ ] Smoke-test the flow from UI through API to the in-memory store.

## 3. Scenario: eligible member with a limited subscription

- [ ] Consume exactly one remaining visit after all validation succeeds.
- [ ] Return the updated visit information through the API and display it in the UI
  success result.
- [ ] Add application, API, and UI tests showing 3 visits become 2 and one check-in
  is created.
- [ ] Smoke-test the limited-plan flow from UI through API.

## 4. Scenario: inactive or suspended member

- [ ] Reject `Inactive` and `Suspended` members without creating attendance or
  changing a subscription.
- [ ] Map the rejection to a consistent API response.
- [ ] Show a clear rejected-state message in the `/check-in` UI.
- [ ] Add application, API, and UI tests for both statuses.
- [ ] Smoke-test each rejection from the UI.

## 5. Scenario: no active subscription

- [ ] Reject a member with no active subscription without creating attendance.
- [ ] Map and display the rejection consistently through API and UI.
- [ ] Add application, API, and UI tests.
- [ ] Smoke-test the rejection from the UI.

## 6. Scenario: subscription date is invalid

- [ ] Reject active subscriptions that have not started or have expired.
- [ ] Map and display each date rejection through API and UI.
- [ ] Add application, API, and UI tests proving no record or mutation occurs.
- [ ] Smoke-test both date cases from the UI.

## 7. Scenario: branch access

- [ ] Reject a branch different from the subscription home branch when cross-branch
  access is disabled.
- [ ] Allow a different branch when cross-branch access is enabled.
- [ ] Return and display clear API/UI outcomes for both paths.
- [ ] Add application, API, and UI tests for rejection and success.
- [ ] Smoke-test both branch paths from the UI.

## 8. Scenario: no visits remaining

- [ ] Reject a limited subscription with zero remaining visits.
- [ ] Enforce a non-negative visit-count invariant.
- [ ] Map and display the rejection through API and UI.
- [ ] Add application, API, and UI tests proving visits remain at zero.
- [ ] Smoke-test the rejection from the UI.

## 9. Scenario: duplicate check-in window

- [ ] Reject a check-in at the same branch within five minutes.
- [ ] Allow a check-in after five minutes.
- [ ] Use the fake clock for the four-minute and six-minute examples.
- [ ] Map and display duplicate and success outcomes through API and UI.
- [ ] Add application, API, and UI tests for both paths.
- [ ] Smoke-test both duplicate-window paths from the UI.

## 10. Scenario: concurrent limited-plan check-ins

- [ ] Make validation, attendance creation, and visit decrement atomic in the
  in-memory store/service transaction boundary.
- [ ] Ensure two concurrent requests with one remaining visit yield exactly one
  success, one check-in, and zero remaining visits.
- [ ] Keep the existing API/UI success and rejection results correct under the
  concurrent outcome.
- [ ] Add concurrency application tests plus API/UI regression tests.
- [ ] Smoke-test the normal UI path after the concurrency change.

## 11. Scenario: tenant isolation and authorization

- [ ] Scope member lookup and check-in operations to the authenticated tenant.
- [ ] Enforce `tenant_id`, `branch_id`, and staff-role authorization on the endpoint.
- [ ] Ensure the UI cannot select or expose branches outside the caller's scope.
- [ ] Add application, API authorization, and UI tests proving cross-tenant data is
  not exposed.
- [ ] Smoke-test authorized and unauthorized receptionist flows.

## 12. Final verification

- [ ] Run all unit, concurrency, API, and UI tests.
- [ ] Run the solution build and GitHub Actions workflow.
- [ ] Confirm coverage output includes the implemented scenarios.
- [ ] Re-run an end-to-end UI-to-API smoke test for every successful scenario.

## Assumptions and Defaults

- Persistence is intentionally in-memory for this iteration; no EF Core or database
  migration is added.
- The existing `tenant_id` and `branch_id` claims remain the authorization source.
- `TenantAdmin` can check in at any branch in the tenant; other staff require a
  matching branch claim.
- The duplicate window is five minutes, matching the feature document.
- Check-in timestamps are stored as UTC `DateTimeOffset`.
- Official attendance records are created only for successful check-ins.
- Existing payment and unrelated demo features remain unchanged.
