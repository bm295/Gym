# Data Dictionary / Database Draft

## 1. Quy ước chung

- Database: PostgreSQL
- Primary key: `uuid`
- Multi-tenant: mọi bảng nghiệp vụ bắt buộc có `tenant_id`
- Audit fields chuẩn:
  - `created_at_utc`
  - `created_by`
  - `updated_at_utc`
  - `updated_by`
  - `deleted_at_utc` khi dùng soft delete
  - `deleted_by` khi dùng soft delete

## 2. Bảng `tenants`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Tenant id |
| code | varchar(50) | No | Yes | Mã tenant |
| name | varchar(200) | No | No | Tên doanh nghiệp gym |
| status | varchar(30) | No | No | Active, Suspended |
| timezone | varchar(100) | No | No | Timezone mặc định |
| created_at_utc | timestamptz | No | No | Audit |
| updated_at_utc | timestamptz | No | No | Audit |

## 3. Bảng `branches`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Branch id |
| tenant_id | uuid | No | No | FK -> tenants.id |
| branch_code | varchar(50) | No | Unique per tenant | Mã chi nhánh |
| name | varchar(200) | No | No | Tên chi nhánh |
| address | varchar(500) | Yes | No | Địa chỉ |
| status | varchar(30) | No | No | Active, Archived |
| created_at_utc | timestamptz | No | No | Audit |
| updated_at_utc | timestamptz | No | No | Audit |
| deleted_at_utc | timestamptz | Yes | No | Soft delete |

## 4. Bảng `staff_users`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Staff user id |
| tenant_id | uuid | No | No | FK -> tenants.id |
| username | varchar(100) | No | Unique per tenant | Tên đăng nhập |
| email | varchar(200) | No | Unique per tenant | Email |
| password_hash | varchar(500) | No | No | Mật khẩu băm |
| full_name | varchar(200) | No | No | Họ tên |
| role | varchar(50) | No | No | TenantAdmin, BranchManager, Receptionist, Trainer |
| status | varchar(30) | No | No | Active, Locked |
| last_login_at_utc | timestamptz | Yes | No | Lần đăng nhập cuối |
| created_at_utc | timestamptz | No | No | Audit |
| updated_at_utc | timestamptz | No | No | Audit |

## 5. Bảng `staff_user_branches`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Mapping id |
| tenant_id | uuid | No | No | Tenant scope |
| staff_user_id | uuid | No | No | FK -> staff_users.id |
| branch_id | uuid | No | No | FK -> branches.id |

## 6. Bảng `trainer_profiles`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Trainer profile id |
| tenant_id | uuid | No | No | Tenant scope |
| staff_user_id | uuid | No | Unique | FK -> staff_users.id |
| display_name | varchar(200) | No | No | Tên hiển thị |
| bio | text | Yes | No | Mô tả ngắn |
| status | varchar(30) | No | No | Active, Inactive |

## 7. Bảng `members`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Member id |
| tenant_id | uuid | No | No | Tenant scope |
| member_code | varchar(50) | No | Unique per tenant | Mã hội viên |
| full_name | varchar(200) | No | No | Họ tên |
| phone_number | varchar(30) | No | No | Số điện thoại gốc |
| phone_number_normalized | varchar(30) | No | Index | Số điện thoại chuẩn hóa |
| date_of_birth | date | Yes | No | Ngày sinh |
| gender | varchar(20) | Yes | No | Gender |
| email | varchar(200) | Yes | No | Email |
| address | varchar(500) | Yes | No | Địa chỉ |
| home_branch_id | uuid | Yes | No | FK -> branches.id |
| status | varchar(30) | No | No | Active, Inactive, Suspended |
| note | text | Yes | No | Ghi chú |
| created_at_utc | timestamptz | No | No | Audit |
| created_by | uuid | Yes | No | Staff user id |
| updated_at_utc | timestamptz | No | No | Audit |
| updated_by | uuid | Yes | No | Staff user id |
| deleted_at_utc | timestamptz | Yes | No | Soft delete |

## 8. Bảng `membership_plans`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Plan id |
| tenant_id | uuid | No | No | Tenant scope |
| plan_code | varchar(50) | No | Unique per tenant | Mã gói |
| name | varchar(200) | No | No | Tên gói |
| description | text | Yes | No | Mô tả |
| duration_days | int | No | No | Số ngày hiệu lực |
| visit_limit | int | Yes | No | Null = không giới hạn |
| default_price | numeric(18,2) | No | No | Giá chuẩn |
| currency | varchar(10) | No | No | Ví dụ VND |
| allow_cross_branch | boolean | No | No | Có cho phép dùng khác chi nhánh |
| status | varchar(30) | No | No | Draft, Active, Archived |
| created_at_utc | timestamptz | No | No | Audit |
| updated_at_utc | timestamptz | No | No | Audit |
| deleted_at_utc | timestamptz | Yes | No | Soft delete |

## 9. Bảng `subscriptions`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Subscription id |
| tenant_id | uuid | No | No | Tenant scope |
| member_id | uuid | No | No | FK -> members.id |
| plan_id | uuid | No | No | FK -> membership_plans.id |
| home_branch_id | uuid | No | No | FK -> branches.id |
| status | varchar(30) | No | No | Pending, Active, Expired, Suspended, Cancelled, Voided |
| start_date | date | No | No | Ngày bắt đầu |
| end_date | date | No | No | Ngày kết thúc |
| sale_price | numeric(18,2) | No | No | Giá bán thực tế |
| amount_paid | numeric(18,2) | No | No | Tổng completed payment hiện tại |
| currency | varchar(10) | No | No | Tiền tệ |
| remaining_visits | int | Yes | No | Null = không giới hạn |
| allow_cross_branch_snapshot | boolean | No | No | Snapshot từ plan |
| plan_name_snapshot | varchar(200) | No | No | Snapshot tên gói |
| duration_days_snapshot | int | No | No | Snapshot thời hạn |
| visit_limit_snapshot | int | Yes | No | Snapshot số buổi |
| original_plan_price_snapshot | numeric(18,2) | No | No | Snapshot giá chuẩn |
| created_at_utc | timestamptz | No | No | Audit |
| created_by | uuid | Yes | No | Staff user id |
| updated_at_utc | timestamptz | No | No | Audit |
| updated_by | uuid | Yes | No | Staff user id |

## 10. Bảng `payments`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Payment id |
| tenant_id | uuid | No | No | Tenant scope |
| subscription_id | uuid | No | No | FK -> subscriptions.id |
| payment_no | varchar(50) | No | Unique per tenant | Mã thanh toán |
| method | varchar(30) | No | No | Cash, BankTransfer |
| status | varchar(30) | No | No | Pending, Completed, Voided |
| amount | numeric(18,2) | No | No | Số tiền |
| currency | varchar(10) | No | No | Tiền tệ |
| paid_at_utc | timestamptz | No | No | Thời điểm ghi nhận |
| reference_no | varchar(100) | Yes | No | Mã chuyển khoản hoặc tham chiếu |
| note | text | Yes | No | Ghi chú |
| void_reason | text | Yes | No | Lý do void |
| voided_at_utc | timestamptz | Yes | No | Thời điểm void |
| voided_by | uuid | Yes | No | Staff user id |
| created_at_utc | timestamptz | No | No | Audit |
| created_by | uuid | Yes | No | Staff user id |

## 11. Bảng `check_ins`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Check-in id |
| tenant_id | uuid | No | No | Tenant scope |
| member_id | uuid | No | No | FK -> members.id |
| subscription_id | uuid | No | No | FK -> subscriptions.id |
| branch_id | uuid | No | No | FK -> branches.id |
| checked_in_at_utc | timestamptz | No | No | Thời điểm check-in |
| created_by | uuid | Yes | No | Staff user id tại quầy |
| note | text | Yes | No | Ghi chú |

## 12. Bảng `audit_logs`

| Field | Type | Null | Unique | Ý nghĩa |
| --- | --- | --- | --- | --- |
| id | uuid | No | PK | Audit id |
| tenant_id | uuid | No | No | Tenant scope |
| actor_user_id | uuid | Yes | No | Staff user id |
| entity_name | varchar(100) | No | No | Tên entity |
| entity_id | uuid | No | No | Id bản ghi |
| action | varchar(50) | No | No | Create, Update, VoidPayment, LockUser... |
| branch_id | uuid | Yes | No | Branch liên quan nếu có |
| old_values | jsonb | Yes | No | Snapshot trước thay đổi |
| new_values | jsonb | Yes | No | Snapshot sau thay đổi |
| created_at_utc | timestamptz | No | No | Thời điểm log |

## 13. Index và constraint bắt buộc

- Unique `(tenant_id, member_code)` trên `members`
- Unique `(tenant_id, plan_code)` trên `membership_plans`
- Unique `(tenant_id, branch_code)` trên `branches`
- Unique `(tenant_id, payment_no)` trên `payments`
- Index `(tenant_id, phone_number_normalized)` trên `members`
- Index `(tenant_id, status, end_date)` trên `subscriptions`
- Index `(tenant_id, branch_id, checked_in_at_utc)` trên `check_ins`
