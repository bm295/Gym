# Database Entity Relationship Diagram

## 1. Phạm vi

ERD này trực quan hóa database draft trong [Data Dictionary](./07-data-dictionary.md), bao gồm các bảng,
khóa chính, khóa ngoại trực tiếp và quan hệ chính của Gym SaaS MVP.

```mermaid
erDiagram
    TENANTS {
        uuid id PK
        varchar code UK
        varchar name
        varchar status
        varchar timezone
        timestamptz created_at_utc
        timestamptz updated_at_utc
    }

    BRANCHES {
        uuid id PK
        uuid tenant_id FK
        varchar branch_code UK
        varchar name
        varchar address "nullable"
        varchar status
        timestamptz created_at_utc
        timestamptz updated_at_utc
        timestamptz deleted_at_utc "nullable"
    }

    STAFF_USERS {
        uuid id PK
        uuid tenant_id FK
        varchar username UK
        varchar email UK
        varchar password_hash
        varchar full_name
        varchar role
        varchar status
        timestamptz last_login_at_utc "nullable"
        timestamptz created_at_utc
        timestamptz updated_at_utc
    }

    STAFF_USER_BRANCHES {
        uuid id PK
        uuid tenant_id FK
        uuid staff_user_id FK
        uuid branch_id FK
    }

    TRAINER_PROFILES {
        uuid id PK
        uuid tenant_id FK
        uuid staff_user_id FK, UK
        varchar display_name
        text bio "nullable"
        varchar status
    }

    MEMBERS {
        uuid id PK
        uuid tenant_id FK
        varchar member_code UK
        varchar full_name
        varchar phone_number
        varchar phone_number_normalized
        date date_of_birth "nullable"
        varchar gender "nullable"
        varchar email "nullable"
        varchar address "nullable"
        uuid home_branch_id FK "nullable"
        varchar status
        text note "nullable"
        timestamptz created_at_utc
        uuid created_by "nullable"
        timestamptz updated_at_utc
        uuid updated_by "nullable"
        timestamptz deleted_at_utc "nullable"
    }

    MEMBERSHIP_PLANS {
        uuid id PK
        uuid tenant_id FK
        varchar plan_code UK
        varchar name
        text description "nullable"
        int duration_days
        int visit_limit "nullable"
        numeric default_price
        varchar currency
        boolean allow_cross_branch
        varchar status
        timestamptz created_at_utc
        timestamptz updated_at_utc
        timestamptz deleted_at_utc "nullable"
    }

    SUBSCRIPTIONS {
        uuid id PK
        uuid tenant_id FK
        uuid member_id FK
        uuid plan_id FK
        uuid home_branch_id FK
        varchar status
        date start_date
        date end_date
        numeric sale_price
        numeric amount_paid
        varchar currency
        int remaining_visits "nullable"
        boolean allow_cross_branch_snapshot
        varchar plan_name_snapshot
        int duration_days_snapshot
        int visit_limit_snapshot "nullable"
        numeric original_plan_price_snapshot
        timestamptz created_at_utc
        uuid created_by "nullable"
        timestamptz updated_at_utc
        uuid updated_by "nullable"
    }

    PAYMENTS {
        uuid id PK
        uuid tenant_id FK
        uuid subscription_id FK
        varchar payment_no UK
        varchar method
        varchar status
        numeric amount
        varchar currency
        timestamptz paid_at_utc
        varchar reference_no "nullable"
        text note "nullable"
        text void_reason "nullable"
        timestamptz voided_at_utc "nullable"
        uuid voided_by "nullable"
        timestamptz created_at_utc
        uuid created_by "nullable"
    }

    CHECK_INS {
        uuid id PK
        uuid tenant_id FK
        uuid member_id FK
        uuid subscription_id FK
        uuid branch_id FK
        timestamptz checked_in_at_utc
        uuid created_by "nullable"
        text note "nullable"
    }

    AUDIT_LOGS {
        uuid id PK
        uuid tenant_id FK
        uuid actor_user_id FK "nullable"
        varchar entity_name
        uuid entity_id
        varchar action
        uuid branch_id FK "nullable"
        jsonb old_values "nullable"
        jsonb new_values "nullable"
        timestamptz created_at_utc
    }

    TENANTS ||--o{ BRANCHES : owns
    TENANTS ||--o{ STAFF_USERS : employs
    TENANTS ||--o{ STAFF_USER_BRANCHES : scopes
    TENANTS ||--o{ TRAINER_PROFILES : scopes
    TENANTS ||--o{ MEMBERS : manages
    TENANTS ||--o{ MEMBERSHIP_PLANS : offers
    TENANTS ||--o{ SUBSCRIPTIONS : scopes
    TENANTS ||--o{ PAYMENTS : scopes
    TENANTS ||--o{ CHECK_INS : scopes
    TENANTS ||--o{ AUDIT_LOGS : records

    STAFF_USERS ||--o{ STAFF_USER_BRANCHES : assigned_to
    BRANCHES ||--o{ STAFF_USER_BRANCHES : contains
    STAFF_USERS ||--o| TRAINER_PROFILES : may_have

    BRANCHES o|--o{ MEMBERS : home_branch
    MEMBERS ||--o{ SUBSCRIPTIONS : purchases
    MEMBERSHIP_PLANS ||--o{ SUBSCRIPTIONS : defines
    BRANCHES ||--o{ SUBSCRIPTIONS : home_branch
    SUBSCRIPTIONS ||--o{ PAYMENTS : receives

    MEMBERS ||--o{ CHECK_INS : performs
    SUBSCRIPTIONS ||--o{ CHECK_INS : authorizes
    BRANCHES ||--o{ CHECK_INS : occurs_at

    STAFF_USERS o|--o{ AUDIT_LOGS : acts_in
    BRANCHES o|--o{ AUDIT_LOGS : relates_to
```

## 2. Chú thích

- `PK`: primary key.
- `FK`: foreign key được mô tả trực tiếp trong data dictionary.
- `UK`: unique key; với dữ liệu nghiệp vụ, constraint thực tế có thể là unique key ghép cùng
  `tenant_id` thay vì unique toàn hệ thống.
- `||`: bắt buộc đúng một; `o|`: không hoặc một; `o{`: không hoặc nhiều.
- Các quan hệ trực tiếp từ `tenants` thể hiện nguyên tắc mọi bảng nghiệp vụ đều được cô lập theo tenant.

## 3. Ràng buộc và lưu ý triển khai

- Các mã `member_code`, `plan_code`, `branch_code` và `payment_no` phải unique trong phạm vi tenant.
- `trainer_profiles.staff_user_id` là unique, nên một staff user có tối đa một trainer profile.
- `members.home_branch_id`, `audit_logs.actor_user_id` và `audit_logs.branch_id` là quan hệ tùy chọn.
- Các cột `created_by`, `updated_by` và `voided_by` đang được data dictionary mô tả là staff user id,
  nhưng chưa được tuyên bố rõ là foreign key bắt buộc. Vì vậy ERD giữ chúng như logical reference và
  chưa vẽ quan hệ vật lý tới `staff_users`.
- `audit_logs.entity_id` là polymorphic reference được xác định cùng `entity_name`, không phải foreign key
  vật lý tới một bảng duy nhất.
- ERD là bản thiết kế; `DbContext`, entity configuration và migration cần được tạo trước khi triển khai schema.
