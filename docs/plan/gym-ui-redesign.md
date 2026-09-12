# Gym SaaS UI Redesign — Operational Prototype

## Goal

Redesign `Gym.WebUI` as a modern operational Gym SaaS application. The product
is desktop- and tablet-first, uses Vietnamese by default with an English option,
and provides interactive client-side prototype data for all business modules.
This plan does not change backend APIs, authentication, persistence, or database
behavior.

## Product Decisions

- Visual direction: bright operational SaaS UI, with teal/green as the action
  color and clear status colors.
- Language: Vietnamese is the default; users can switch the UI to English.
- Prototype data: modules without supporting APIs use realistic in-memory UI
  data and interactions for the current browser session.
- Check-in: use a receptionist-oriented prototype flow. Do not expose tenant,
  branch, or user UUID fields in the user interface.
- Device priority: desktop and tablet. Mobile remains responsive for essential
  viewing and actions.

## 1. Design Foundation

- [x] Define application color tokens for brand, surface, text, borders,
  success, warning, danger, and informational states.
- [x] Define typography, spacing, elevation, radius, and responsive breakpoint
  tokens.
- [x] Create reusable Blazor UI primitives: page header, cards, status badges,
  buttons, search/filter bars, data tables, empty states, loading states,
  validation summaries, confirmation modal, drawer, and toast notifications.
- [ ] Establish accessible focus states, keyboard operation, semantic headings,
  and sufficient color contrast for all primitives.
- [ ] Replace the default Bootstrap starter appearance with the operational SaaS
  visual system while retaining only useful Bootstrap layout utilities.

## 2. App Shell and Localization

- [ ] Replace the starter layout with a responsive application shell.
- [ ] Add a grouped sidebar for Overview, Members, Plans & Subscriptions, Front
  Desk, Finance, and Administration.
- [ ] Add a top bar with branch context, global quick search entry point,
  notifications, language switcher, and staff profile menu.
- [ ] Implement compact/collapsible navigation for tablet widths and usable
  responsive behavior for mobile widths.
- [ ] Introduce a UI-only current staff member and selected branch context.
- [ ] Add a client-side localization dictionary/resource layer for Vietnamese and
  English navigation, labels, messages, statuses, and help text.
- [ ] Persist the selected language and selected branch for the browser session.

## 3. Interactive Domain Prototypes

- [ ] Create realistic UI-only seed data for tenants, branches, staff, members,
  membership plans, subscriptions, payments, check-ins, audit entries, and
  reports.
- [ ] Create a UI state service that owns prototype records and applies create,
  edit, lock, archive, sell, renew, payment, void, and check-in actions in the
  active browser session.
- [ ] Clearly identify simulated actions in modules that do not yet have backend
  support; do not imply that these actions are persisted remotely.
- [ ] Keep the existing API client and backend contracts unchanged and separate
  from the prototype state.

## 4. Operational Screens

- [ ] Build the Dashboard with active-member, today’s check-in, today’s revenue,
  and expiring-subscription cards; include branch filtering and action shortcuts.
- [ ] Build Member List with search, status/branch filters, sortable columns,
  pagination, and create-member entry point.
- [ ] Build Member Detail with profile summary, current subscription status,
  payments, check-in history, and a chronological activity timeline.
- [ ] Build create/edit member forms with inline validation and an explicit
  cancel/unsaved-changes experience.
- [ ] Build Plan List and Plan Detail with plan status, duration, price, visit
  limit, cross-branch policy, and archive interaction.
- [ ] Build subscription sale, renewal, and subscription-list flows using plan
  snapshots, date range, home branch, amount paid, and activation status.
- [ ] Build Payment History with date/member/status filters, payment details, and
  a reason-required void confirmation prototype.
- [ ] Build Attendance History with date, branch, and member filters.
- [ ] Build Branch, Staff, Reports, and Audit Log prototype screens with domain
  appropriate tables, filters, empty states, and role/status badges.

## 5. Reception Check-in

- [ ] Replace UUID entry with the selected branch from the app context.
- [ ] Make member-code/phone lookup the primary visual and keyboard-focused
  action.
- [ ] Display a member summary card with name, member code, photo placeholder,
  member status, plan name, validity period, permitted branch status, and
  remaining visits or unlimited entitlement.
- [ ] Support clear prototype outcomes: successful unlimited plan, successful
  limited plan, inactive/suspended member, no active subscription, invalid
  subscription date, wrong branch, zero visits, and duplicate recent check-in.
- [ ] Present a high-confidence success state with timestamp and an immediate
  "check in another member" action.
- [ ] Present actionable rejection states that explain the business reason and
  direct the receptionist to the relevant member/subscription workflow.
- [ ] Add a recent check-in list for the selected branch to help the receptionist
  verify the latest attendance activity.

## 6. Quality and Verification

- [ ] Add component tests for app-shell navigation, language switching, branch
  selection, responsive sidebar state, and persisted session state.
- [ ] Add UI tests for tables, filters, pagination, empty states, form
  validation, confirmation modals, and prototype data mutations.
- [ ] Add check-in UI tests for all supported success and rejection outcomes.
- [ ] Smoke-test the key desktop and tablet paths: dashboard, member lookup,
  subscription sale/renewal prototype, payment void prototype, and check-in.
- [ ] Verify keyboard navigation, focus order, screen-reader status messages, and
  color contrast on key operational flows.
- [ ] Confirm no backend code, API contracts, database schema, or authentication
  behavior has changed.
