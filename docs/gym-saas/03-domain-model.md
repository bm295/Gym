# Domain Model

## 1. Bounded Context cho giai đoạn 1

- `Identity & Access`
- `Tenant & Branch`
- `Members`
- `Catalog` cho gói tập
- `Sales` cho subscription và payment
- `Attendance`
- `Reporting`
- `Audit`

## 2. Entity chính

### 2.1 Tenant

Đại diện cho một khách hàng SaaS, ví dụ một thương hiệu gym hoặc một công ty vận hành chuỗi gym.

### 2.2 Branch

Đại diện cho một chi nhánh vật lý thuộc một tenant. Nhiều nghiệp vụ như check-in, bán gói và báo cáo cần gắn với branch.

### 2.3 StaffUser

Người dùng nội bộ của tenant đăng nhập hệ thống. StaffUser có role và phạm vi branch được phép truy cập.

### 2.4 TrainerProfile

Hồ sơ mở rộng cho staff có vai trò trainer. Giai đoạn 1 chỉ lưu thông tin cơ bản để phân loại và hiển thị; scheduling chi tiết để phase sau.

### 2.5 Member

Hội viên được quản lý bởi tenant. Member là thực thể trung tâm để gắn subscription, payment history và check-in history.

### 2.6 MembershipPlan

Danh mục gói tập do tenant định nghĩa, ví dụ tháng, quý, năm. Plan có giá chuẩn, thời hạn, giới hạn số buổi và chính sách cross-branch.

### 2.7 Subscription

Bản ghi quyền sử dụng dịch vụ của member theo thời gian. Subscription được tạo khi bán gói hoặc gia hạn. Subscription phải lưu snapshot plan tại thời điểm bán để chống sai lệch lịch sử.

### 2.8 Payment

Bản ghi tài chính cho giao dịch thu tiền. Payment có thể gắn với subscription sale hoặc renewal và phải có trạng thái để hỗ trợ void thay vì xóa.

### 2.9 CheckIn

Nhật ký check-in của member tại chi nhánh, tham chiếu subscription được dùng để xác thực quyền vào cửa.

### 2.10 AuditLog

Nhật ký hệ thống cho các thao tác quan trọng như sửa hồ sơ hội viên, bán gói, void payment, khóa user.

## 3. Aggregate gợi ý

- `Member` aggregate
  - Member
  - MemberNote trong tương lai nếu cần
- `Plan` aggregate
  - MembershipPlan
- `Subscription` aggregate
  - Subscription
  - Payment
- `Attendance` aggregate
  - CheckIn
- `Staff` aggregate
  - StaffUser
  - TrainerProfile

## 4. Quan hệ chính

- Một `Tenant` có nhiều `Branch`.
- Một `Tenant` có nhiều `StaffUser`.
- Một `Tenant` có nhiều `Member`.
- Một `Tenant` có nhiều `MembershipPlan`.
- Một `Member` có nhiều `Subscription` theo thời gian.
- Một `Subscription` thuộc về đúng một `Member`.
- Một `Subscription` có thể có nhiều `Payment`.
- Một `Member` có nhiều `CheckIn`.
- Một `CheckIn` tham chiếu một `Subscription` hợp lệ tại thời điểm check-in.
- Một `StaffUser` có thể được gán quyền ở nhiều `Branch`.

## 5. Sơ đồ quan hệ mức khái niệm

```text
Tenant
 ├─ Branch
 ├─ StaffUser ── TrainerProfile?
 ├─ Member
 ├─ MembershipPlan
 ├─ Subscription ── Payment
 ├─ CheckIn
 └─ AuditLog

Member 1 --- * Subscription
Subscription 1 --- * Payment
Member 1 --- * CheckIn
Branch 1 --- * CheckIn
Branch 1 --- * Subscription
MembershipPlan 1 --- * Subscription
```

## 6. Value Object và enum chính

### Value Object

- `Money`: amount + currency
- `PersonName`: full name normalized
- `PhoneNumber`: raw value + normalized value
- `DateRange`: start date, end date
- `PlanSnapshot`: plan name, duration, price, visit limit, cross-branch policy tại thời điểm bán

### Enum

- `MemberStatus`: Draft, Active, Inactive, Suspended
- `PlanStatus`: Draft, Active, Archived
- `SubscriptionStatus`: Pending, Active, Expired, Suspended, Cancelled, Voided
- `PaymentMethod`: Cash, BankTransfer
- `PaymentStatus`: Pending, Completed, Voided
- `StaffRole`: TenantAdmin, BranchManager, Receptionist, Trainer

## 7. Invariant cốt lõi theo aggregate

### Member

- Member code phải unique trong tenant.
- Member bị khóa không được bán gói mới hoặc check-in.

### MembershipPlan

- Plan archived không được dùng cho sale mới.
- Giá và policy hiện hành không được làm thay đổi subscription lịch sử.

### Subscription

- Mỗi member chỉ có tối đa một subscription `Active` tại một thời điểm trong cùng tenant.
- End date phải lớn hơn hoặc bằng start date.
- Nếu plan có giới hạn số buổi, remaining visits không được âm.

### Payment

- Payment `Completed` không được hard delete.
- Payment `Voided` bắt buộc có `voidReason`.

### CheckIn

- Check-in chỉ được tạo khi có subscription hợp lệ tại thời điểm thao tác.
- Check-in phải gắn branch thực tế xảy ra sự kiện.
