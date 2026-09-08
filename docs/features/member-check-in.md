Feature: Member check-in
  As a gym receptionist
  I want to check members into a branch
  So that the gym records valid visits and enforces membership rules

  Background:
    Given the receptionist is authenticated
    And the receptionist has access to the selected branch
    And the current time is represented in UTC

  Scenario: Check in an eligible member with an unlimited subscription
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has an "Active" subscription
    And the current local date is between the subscription start date and end date
    And the subscription home branch is the selected branch
    And the member has no check-in at the selected branch within the last 5 minutes
    When the receptionist searches for the member by member code or phone number
    And the receptionist confirms the check-in at the selected branch
    Then the check-in is successful
    And exactly one check-in record is created
    And the check-in record contains the tenant, member, subscription, branch, and UTC timestamp
    And the subscription remaining visit count is unchanged
    And the check-in operation is written to the operational or audit log

  Scenario: Check in an eligible member with a limited subscription
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has an "Active" subscription
    And the current local date is between the subscription start date and end date
    And the selected branch is permitted by the subscription
    And the subscription has 3 remaining visits
    And the member has no check-in at the selected branch within the last 5 minutes
    When the receptionist confirms the check-in
    Then the check-in is successful
    And exactly one check-in record is created
    And the subscription has 2 remaining visits

  Scenario Outline: Reject check-in when the member is not active
    Given the member belongs to the current tenant
    And the member status is "<member_status>"
    And the member has an "Active" subscription
    When the receptionist confirms the check-in
    Then the check-in is rejected
    And no check-in record is created
    And the subscription is not changed

    Examples:
      | member_status |
      | Inactive      |
      | Suspended     |

  Scenario: Reject check-in when the member has no active subscription
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has no "Active" subscription
    When the receptionist confirms the check-in
    Then the check-in is rejected
    And no check-in record is created

  Scenario Outline: Reject check-in when the subscription is not currently valid
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has an "Active" subscription
    And the subscription is "<validity>"
    When the receptionist confirms the check-in
    Then the check-in is rejected
    And no check-in record is created
    And the subscription is not changed

    Examples:
      | validity    |
      | not started |
      | expired     |

  Scenario: Reject check-in at a branch not permitted by the subscription
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has an "Active" subscription
    And the selected branch is not the subscription home branch
    And the subscription does not allow cross-branch check-in
    When the receptionist confirms the check-in
    Then the check-in is rejected
    And no check-in record is created
    And the subscription is not changed

  Scenario: Allow check-in at another branch when cross-branch access is enabled
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has an "Active" subscription
    And the selected branch is not the subscription home branch
    And the subscription allows cross-branch check-in
    And the member has no check-in at the selected branch within the last 5 minutes
    When the receptionist confirms the check-in
    Then the check-in is successful
    And exactly one check-in record is created for the selected branch

  Scenario: Reject check-in when no visits remain
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has a valid "Active" subscription
    And the subscription has 0 remaining visits
    When the receptionist confirms the check-in
    Then the check-in is rejected
    And no check-in record is created
    And the remaining visit count stays at 0

  Scenario: Reject a duplicate check-in within five minutes
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has a valid "Active" subscription
    And the member checked in at the selected branch 4 minutes ago
    When the receptionist confirms the check-in
    Then the check-in is rejected as a duplicate
    And no additional check-in record is created
    And the subscription is not changed

  Scenario: Allow check-in after the duplicate window
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has a valid "Active" subscription
    And the member checked in at the selected branch 6 minutes ago
    When the receptionist confirms the check-in
    Then the check-in is successful
    And exactly one additional check-in record is created

  Scenario: Reject a member from another tenant without exposing data
    Given the searched member belongs to another tenant
    When the receptionist searches for the member
    Then the member is not returned
    And no check-in record is created
    And no cross-tenant member data is exposed

  Scenario: Prevent concurrent limited-plan check-ins from exceeding the visit limit
    Given the member belongs to the current tenant
    And the member status is "Active"
    And the member has a valid "Active" subscription
    And the subscription has 1 remaining visit
    And two check-in requests are submitted concurrently
    When both requests are processed
    Then exactly one request succeeds
    And exactly one check-in record is created
    And the subscription has 0 remaining visits
    And the remaining visit count never becomes negative

  Rule: Rejected attempts may be logged for troubleshooting
    But a rejected attempt must not create an official check-in record
    And a rejected attempt must not consume a subscription visit
