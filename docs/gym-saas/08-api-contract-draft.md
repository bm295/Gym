# API Contract Draft

## 1. Quy ước chung

- Base path: `/api`
- Auth: `Bearer JWT`
- Tenant context lấy từ token, không lấy từ body để authorize
- Content type: `application/json`
- Datetime trả về theo ISO 8601 UTC

## 2. Envelope đề xuất

### Success

```json
{
  "data": {},
  "meta": {
    "page": 1,
    "pageSize": 20,
    "total": 100
  }
}
```

### Error

```json
{
  "error": {
    "code": "subscription_expired",
    "message": "Subscription has expired.",
    "details": {}
  }
}
```

## 3. Auth APIs

### `POST /api/auth/login`

Purpose: đăng nhập staff.

Request:

```json
{
  "username": "reception01",
  "password": "string"
}
```

Response:

```json
{
  "data": {
    "accessToken": "jwt",
    "refreshToken": "string",
    "user": {
      "id": "uuid",
      "fullName": "string",
      "role": "Receptionist",
      "allowedBranchIds": ["uuid"]
    }
  }
}
```

Errors:

- `invalid_credentials`
- `user_locked`

### `POST /api/auth/refresh`

Purpose: lấy access token mới.

## 4. Member APIs

### `GET /api/members`

Purpose: danh sách hội viên.

Query:

- `q`
- `status`
- `branchId`
- `hasActiveSubscription`
- `page`
- `pageSize`

### `POST /api/members`

Purpose: tạo hội viên.

Request:

```json
{
  "fullName": "Nguyen Van A",
  "phoneNumber": "0901234567",
  "dateOfBirth": "1998-01-01",
  "gender": "Male",
  "email": "a@example.com",
  "address": "string",
  "homeBranchId": "uuid",
  "note": "string"
}
```

Validation:

- full name bắt buộc
- phone number bắt buộc
- home branch phải thuộc tenant hiện tại nếu được gửi

### `GET /api/members/{id}`

Purpose: chi tiết hội viên.

### `PUT /api/members/{id}`

Purpose: cập nhật hội viên.

### `POST /api/members/{id}/lock`

Purpose: khóa hội viên.

Request:

```json
{
  "reason": "Membership violation"
}
```

### `POST /api/members/{id}/unlock`

Purpose: mở lại hội viên.

### `GET /api/members/{id}/timeline`

Purpose: lấy timeline gồm profile summary, subscriptions, payments, check-ins.

## 5. Plan APIs

### `GET /api/plans`

Purpose: danh sách plan.

### `POST /api/plans`

Request:

```json
{
  "planCode": "MONTHLY-01",
  "name": "Goi 1 thang",
  "description": "string",
  "durationDays": 30,
  "visitLimit": null,
  "defaultPrice": 800000,
  "currency": "VND",
  "allowCrossBranch": false
}
```

### `GET /api/plans/{id}`

Purpose: chi tiết plan.

### `PUT /api/plans/{id}`

Purpose: cập nhật plan cho sale tương lai.

### `POST /api/plans/{id}/archive`

Purpose: archive plan.

## 6. Subscription APIs

### `GET /api/subscriptions`

Query:

- `memberId`
- `status`
- `branchId`
- `planId`
- `expiringFrom`
- `expiringTo`
- `page`
- `pageSize`

### `POST /api/subscriptions/sell`

Purpose: bán gói cho hội viên và tạo sale.

Request:

```json
{
  "memberId": "uuid",
  "planId": "uuid",
  "homeBranchId": "uuid",
  "startDate": "2026-03-21",
  "salePrice": 800000,
  "payments": [
    {
      "method": "Cash",
      "amount": 800000,
      "paidAtUtc": "2026-03-20T15:00:00Z",
      "referenceNo": null,
      "note": "full payment"
    }
  ]
}
```

Response:

```json
{
  "data": {
    "subscriptionId": "uuid",
    "status": "Active",
    "memberId": "uuid",
    "planName": "Goi 1 thang",
    "startDate": "2026-03-21",
    "endDate": "2026-04-19",
    "amountPaid": 800000
  }
}
```

Errors:

- `member_suspended`
- `plan_inactive`
- `active_subscription_conflict`
- `invalid_branch_access`

### `POST /api/subscriptions/{id}/renew`

Purpose: gia hạn từ subscription hiện tại.

### `GET /api/subscriptions/{id}`

Purpose: chi tiết subscription.

### `POST /api/subscriptions/{id}/cancel`

Purpose: hủy subscription chưa hoặc không còn hợp lệ theo rule.

## 7. Payment APIs

### `GET /api/payments`

Query:

- `memberId`
- `subscriptionId`
- `method`
- `status`
- `from`
- `to`
- `branchId`
- `page`
- `pageSize`

### `POST /api/payments`

Purpose: ghi nhận thanh toán bổ sung.

Request:

```json
{
  "subscriptionId": "uuid",
  "method": "BankTransfer",
  "amount": 300000,
  "paidAtUtc": "2026-03-20T15:10:00Z",
  "referenceNo": "VCB12345",
  "note": "partial payment"
}
```

### `POST /api/payments/{id}/void`

Purpose: void payment.

Request:

```json
{
  "reason": "Duplicate transaction"
}
```

Errors:

- `payment_not_found`
- `payment_already_voided`
- `insufficient_permission`

## 8. Check-in APIs

### `POST /api/checkins`

Purpose: check-in hội viên.

Request:

```json
{
  "memberQuery": "M000123",
  "branchId": "uuid"
}
```

Response:

```json
{
  "data": {
    "checkInId": "uuid",
    "memberId": "uuid",
    "memberName": "Nguyen Van A",
    "checkedInAtUtc": "2026-03-20T15:15:00Z",
    "subscriptionId": "uuid",
    "remainingVisits": 11
  }
}
```

Errors:

- `member_not_found`
- `subscription_not_active`
- `subscription_expired`
- `branch_not_allowed`
- `duplicate_checkin_window`
- `visit_limit_exhausted`

### `GET /api/checkins`

Purpose: lịch sử check-in.

### `GET /api/checkins/today`

Purpose: danh sách check-in hôm nay.

## 9. Staff APIs

### `GET /api/staff`

Purpose: danh sách staff.

### `POST /api/staff`

Purpose: tạo staff user.

### `PUT /api/staff/{id}`

Purpose: cập nhật role, branch access hoặc profile.

### `POST /api/staff/{id}/lock`

Purpose: khóa staff.

## 10. Branch APIs

### `GET /api/branches`

Purpose: danh sách branch.

### `POST /api/branches`

Purpose: tạo branch.

### `PUT /api/branches/{id}`

Purpose: cập nhật branch.

### `POST /api/branches/{id}/archive`

Purpose: archive branch.

## 11. Reporting APIs

### `GET /api/dashboard/summary`

Purpose: dữ liệu dashboard tổng quan.

### `GET /api/reports/revenue`

Query:

- `from`
- `to`
- `branchId`
- `groupBy=day|week|month`

### `GET /api/reports/attendance`

Purpose: tổng hợp attendance.

### `GET /api/reports/subscription-expiry`

Purpose: danh sách subscription sắp hết hạn.

## 12. Audit APIs

### `GET /api/audit-logs`

Query:

- `entityName`
- `entityId`
- `actorUserId`
- `from`
- `to`
- `page`
- `pageSize`
