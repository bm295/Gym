# Tenant Interaction

Feature: Tenant interaction in Gym SaaS

  A tenant represents one gym business or gym chain. It owns its branches,
  staff, members, plans, subscriptions, payments, attendance, reports, and
  audit history. Staff operate in the authenticated tenant context and should
  never select or enter a tenant identifier in normal operational screens.

  Background:
    Given the staff user is authenticated
    And the authentication context contains a tenant identifier
    And the UI has loaded the tenant name, allowed branches, and staff role

  Scenario: Staff enters the operational workspace for their tenant
    Given Linh is a Receptionist for tenant "GymOS Fitness"
    When Linh opens the Gym SaaS workspace
    Then the app shell shows the tenant's operational workspace
    And Linh does not see or enter a tenant UUID
    And member, subscription, payment, and attendance screens are scoped to
      "GymOS Fitness"

  Scenario: Receptionist changes the active branch within their tenant
    Given Linh can access branches "District 1" and "Thao Dien"
    And the selected branch is "District 1"
    When Linh selects "Thao Dien" in the branch context selector
    Then the selected branch becomes "Thao Dien"
    And the dashboard and recent check-in list use "Thao Dien" data
    And a new check-in is recorded for "Thao Dien"
    And the selected branch is preserved for the browser session

  Scenario: Tenant administrator manages all branches in their tenant
    Given Minh is a Tenant Admin for "GymOS Fitness"
    When Minh opens Branches, Staff, Plans, or Reports
    Then Minh can view records across all branches owned by "GymOS Fitness"
    And branch-specific filters are available where applicable
    And Minh cannot view records belonging to another tenant

  Scenario: Branch manager is limited to permitted branches
    Given a Branch Manager has access only to "District 1"
    When the Branch Manager opens the branch context selector
    Then only "District 1" is available
    And branch-scoped dashboard, member, attendance, and payment results use
      "District 1"
    And requests for another branch are rejected by authorization

  Scenario: Receptionist checks in a member in the tenant context
    Given a Receptionist has selected an allowed branch
    And a member belongs to the authenticated tenant
    When the Receptionist looks up the member by member code or phone number
    Then the UI shows the member's tenant-scoped membership entitlement
    And the check-in request includes the authenticated tenant and selected
      branch context
    And the Receptionist never needs to enter tenant or branch UUID values

  Scenario: Tenant data remains isolated from another gym business
    Given tenant "GymOS Fitness" and tenant "Power Club" have members with
      different records
    When a "GymOS Fitness" staff user searches for a Power Club member code
    Then the member is not returned
    And the user cannot read, change, pay for, renew, or check in that member
    And audit history records only actions within "GymOS Fitness"

  Scenario: Tenant admin views tenant-wide operational reporting
    Given the Tenant Admin selects a reporting period
    When the Tenant Admin opens the Dashboard or Reports workspace
    Then active member, attendance, revenue, and expiring-subscription metrics
      are calculated from records in the authenticated tenant
    And branch filtering narrows the tenant-wide result set
    And no metric includes another tenant's data

  Scenario: UI prototype communicates its tenant-data limitation
    Given the UI is running in prototype mode
    When staff creates, edits, sells, renews, voids, or checks in a record
    Then the UI identifies the action as simulated
    And the change exists only in the current browser session
    And the prototype does not imply that tenant data was persisted remotely

## UI Principles

- Tenant is established by authentication and is not a normal form field.
- Branch is the operational context exposed in the top bar, limited by staff
  access within the authenticated tenant.
- Tenant name may be shown in the app shell or profile menu for orientation;
  tenant UUIDs are never shown to front-desk users.
- Tenant-wide screens are role-aware. Tenant Admin can see all permitted
  branches; Branch Manager and Receptionist see only their allowed branches.
- API authorization remains the source of truth. UI filtering improves usability
  but must not be relied on as the security boundary.
