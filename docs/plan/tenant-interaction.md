# Implement Tenant Interaction

## Summary

Implement `docs/features/tenant-interaction.md` as secured end-to-end vertical
slices. Tenant identity must originate from authenticated claims, branch access
must be role-scoped, and the WebUI must consume a tenant context rather than
asking operational users for tenant or branch identifiers.

## Delivery Rule

Do not mark a scenario complete until all applicable tasks are done:

- backend domain/application behavior and tests;
- API contract, authorization, and integration tests;
- WebUI behavior and component tests;
- smoke test using an authenticated user and the in-memory store.

## 1. Shared tenant-context foundation

- [ ] Define a tenant-context application contract containing tenant identity,
  tenant name, authenticated staff identity, role, and allowed branches.
- [ ] Define a branch-access model for Tenant Admin, Branch Manager, and
  Receptionist, including tenant-wide versus explicit branch access.
- [ ] Add an application service that resolves tenant context from authenticated
  claims without accepting a tenant ID from UI input.
- [ ] Extend in-memory seed data with two tenants, multiple branches, staff
  roles, branch access assignments, and tenant-scoped operational records.
- [ ] Add repositories/query methods that require tenant scope for members,
  plans, subscriptions, payments, check-ins, audit entries, and reports.
- [ ] Add `GET /api/context` or equivalent authenticated endpoint that returns
  the tenant context and allowed branch list for the current staff user.
- [ ] Update JWT claim conventions and authorization handlers to validate tenant
  ID, staff ID, role, and branch access consistently.
- [ ] Add backend unit and API tests for missing, malformed, and conflicting
  tenant/branch claims.
- [ ] Replace WebUI prototype shell context with an authenticated tenant-context
  client when the API is available; retain an explicit prototype adapter only
  for demo mode.
- [ ] Add a WebUI tenant-context loading state, unavailable state, and a clear
  distinction between live and prototype mode.

## 2. Scenario: Staff enters the operational workspace

- [ ] BE: return only the authenticated tenant's tenant name, staff identity,
  role, and permitted branches from the context endpoint.
- [ ] BE: reject context requests without a valid authenticated staff identity.
- [ ] FE: load tenant context during app-shell initialization and show tenant
  name and current staff role in the orientation/profile area.
- [ ] FE: remove tenant UUID fields from all operational UI flows.
- [ ] FE: route to an authorization/unavailable state if tenant context cannot
  be loaded.
- [ ] Tests: prove a receptionist sees only their tenant workspace and never a
  tenant UUID input.
- [ ] Smoke: authenticate as a receptionist and open the app shell.

## 3. Scenario: Receptionist changes active branch

- [ ] BE: add branch-scoped query parameters/contracts for dashboard, recent
  attendance, member lookup, and check-in operations.
- [ ] BE: validate that a requested branch belongs to the authenticated tenant
  and is permitted for the authenticated staff role.
- [ ] FE: populate the top-bar branch selector from the context endpoint, not
  local seed data.
- [ ] FE: refresh dashboard, recent check-ins, lookup results, and check-in
  submission context when the selected branch changes.
- [ ] FE: persist the selected permitted branch in browser session storage and
  validate it against refreshed context before reuse.
- [ ] Tests: prove selecting a permitted branch updates branch-scoped UI data;
  prove unavailable branches are absent from the selector.
- [ ] Smoke: switch District 1 to Thao Dien, verify dashboard/attendance, and
  record a check-in at the new branch.

## 4. Scenario: Tenant Admin and Branch Manager access

- [ ] BE: implement tenant-wide branch resolution for Tenant Admin.
- [ ] BE: implement explicit allowed-branch resolution for Branch Manager and
  Receptionist.
- [ ] BE: add authorization policies to branch, staff, plan, report, member,
  payment, subscription, and attendance endpoints.
- [ ] BE: return `403 Forbidden` for a valid tenant user requesting a branch
  outside their allowed access; return `404` or scoped empty results for records
  outside tenant scope according to endpoint contract.
- [ ] FE: show all tenant branches for Tenant Admin and only permitted branches
  for Branch Manager/Receptionist.
- [ ] FE: hide or disable actions/modules not allowed by the current role while
  preserving server-side authorization as the security boundary.
- [ ] Tests: cover Tenant Admin all-branch visibility and Branch Manager
  single-branch UI/API rejection paths.
- [ ] Smoke: exercise both roles against multiple branches.

## 5. Scenario: Tenant-scoped member check-in

- [ ] BE: update check-in request handling to derive tenant and staff from
  authenticated context; retain branch as a validated route/request value.
- [ ] BE: scope member code/phone lookup, active subscription selection,
  duplicate-window checks, attendance creation, and audit records to tenant.
- [ ] BE: return tenant-safe check-in response/rejection contracts without
  leaking another tenant's member or subscription data.
- [ ] FE: submit check-in using selected branch and authenticated context, with
  no tenant/branch UUID entry controls.
- [ ] FE: show tenant-scoped member entitlement, branch permission, and
  actionable rejection guidance.
- [ ] Tests: prove same code/phone in another tenant cannot be checked in or
  revealed; cover authorized successful check-in.
- [ ] Smoke: check in a valid member as an allowed receptionist branch user.

## 6. Scenario: Cross-tenant data isolation

- [ ] BE: enforce tenant predicates in every repository read/write path,
  including direct ID lookups, list filters, payment void, renewal, sale,
  check-in, reports, and audit logs.
- [ ] BE: ensure every newly created subscription, payment, check-in, and audit
  entry receives the tenant ID from the authenticated context.
- [ ] BE: add integration tests for attempted cross-tenant read, mutation,
  payment void, renewal, sale, check-in, and audit access.
- [ ] FE: clear branch/session context on logout or tenant-context change.
- [ ] FE: handle authorization/not-found responses without showing foreign
  record metadata.
- [ ] Tests: use GymOS Fitness and Power Club seed data to prove isolation in
  application, API, and UI layers.
- [ ] Smoke: search and attempt actions on a known other-tenant member.

## 7. Scenario: Tenant-wide reporting

- [ ] BE: implement tenant-scoped dashboard and reports queries for active
  members, attendance, revenue, and expiring subscriptions.
- [ ] BE: support valid reporting-period and optional permitted-branch filters.
- [ ] BE: aggregate only completed/non-voided payments and tenant-owned records.
- [ ] FE: add reporting-period selector and branch filter to Dashboard/Reports.
- [ ] FE: present tenant-wide results to Tenant Admin and branch-limited results
  to other staff roles.
- [ ] Tests: verify aggregation correctness, branch narrowing, role scope, and
  cross-tenant exclusion.
- [ ] Smoke: compare tenant-wide and single-branch metrics as Tenant Admin.

## 8. Scenario: Prototype-mode limitation

- [ ] FE: make prototype mode an explicit configuration/runtime mode, separate
  from live authenticated API mode.
- [ ] FE: display a persistent notice that create/edit/sell/renew/void/check-in
  actions are browser-session simulations in prototype mode.
- [ ] FE: ensure live mode never labels successfully persisted API actions as
  simulated.
- [ ] Tests: verify prototype state does not issue API calls and is reset after
  browser session reset; verify live mode uses API clients.
- [ ] Smoke: perform each prototype mutation and confirm no server/in-memory API
  record changed.

## 9. Final verification

- [ ] Run all application, API authorization, integration, and Blazor UI tests.
- [ ] Run the solution build and GitHub Actions workflow.
- [ ] Run authenticated smoke tests for Receptionist, Branch Manager, and Tenant
  Admin across two tenants and multiple branches.
- [ ] Review endpoint contracts and logs to confirm no cross-tenant data leak.
- [ ] Confirm tenant/branch UUID inputs are absent from operational UI screens.
