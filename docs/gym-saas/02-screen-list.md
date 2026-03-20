# Screen List / Information Architecture

## 1. Nguyên tắc chung

- Mỗi màn hình phải hiển thị rõ `tenant context` và `branch context` khi có áp dụng.
- Danh sách dùng phân trang server-side, filter và sort.
- Các hành động thay đổi dữ liệu quan trọng phải có confirm dialog và audit log.

## 2. Danh sách màn hình

| Screen | Mục đích | Dữ liệu chính | Action chính | API chính |
| --- | --- | --- | --- | --- |
| Login | Đăng nhập staff | email/username, password | login, refresh token | `POST /api/auth/login`, `POST /api/auth/refresh` |
| Dashboard | Tổng quan vận hành | active members, check-ins today, doanh thu hôm nay, subscriptions sắp hết hạn | lọc theo chi nhánh, mở nhanh chi tiết | `GET /api/dashboard/summary` |
| Member List | Quản lý danh sách hội viên | member code, tên, số điện thoại, status, branch, active subscription | tìm kiếm, filter, mở chi tiết, tạo mới, khóa/mở | `GET /api/members`, `POST /api/members`, `POST /api/members/{id}/lock` |
| Member Detail | Xem hồ sơ và timeline hội viên | profile, subscription hiện tại, lịch sử thanh toán, lịch sử check-in | sửa hồ sơ, bán gói, gia hạn, xem timeline | `GET /api/members/{id}`, `GET /api/members/{id}/timeline` |
| Create/Edit Member | Tạo hoặc sửa hội viên | form profile, branch mặc định, emergency contact | lưu, hủy | `POST /api/members`, `PUT /api/members/{id}` |
| Plan List | Quản lý gói tập | plan code, tên gói, thời hạn, giá, visit limit, status | tạo, sửa, archive | `GET /api/plans`, `POST /api/plans`, `PUT /api/plans/{id}`, `POST /api/plans/{id}/archive` |
| Plan Detail | Xem chi tiết gói tập | policy sử dụng, cross-branch, giá, subscriptions liên quan | chỉnh sửa, archive | `GET /api/plans/{id}` |
| Sell Subscription | Bán gói cho hội viên | member snapshot, plan, start date, branch, giá bán, thanh toán ban đầu | xác nhận bán gói | `POST /api/subscriptions/sell` |
| Renew Subscription | Gia hạn gói | current subscription, plan mới hoặc plan hiện tại, ngày bắt đầu mới | gia hạn | `POST /api/subscriptions/{id}/renew` |
| Subscription List | Quản lý subscription | member, plan, start/end date, status, branch | filter, xem chi tiết, suspend/cancel | `GET /api/subscriptions` |
| Payment History | Tra cứu thanh toán | payment no, member, amount, method, status, paid at | tạo thanh toán, void, export | `GET /api/payments`, `POST /api/payments`, `POST /api/payments/{id}/void` |
| Check-in Screen | Check-in tại quầy | ô tìm kiếm member, trạng thái gói, kết quả check-in mới nhất | check-in, xem lỗi lý do từ chối | `POST /api/checkins`, `GET /api/checkins/today` |
| Attendance History | Xem lịch sử đi tập | member, branch, checked in at, created by | lọc theo ngày, theo chi nhánh | `GET /api/checkins` |
| Staff List | Quản lý người dùng nội bộ | tên, email, role, branch access, status | tạo tài khoản, reset password, khóa | `GET /api/staff`, `POST /api/staff`, `PUT /api/staff/{id}` |
| Branch List | Quản lý chi nhánh | branch code, tên, địa chỉ, trạng thái | tạo, sửa, archive | `GET /api/branches`, `POST /api/branches`, `PUT /api/branches/{id}` |
| Reports | Báo cáo cơ bản | doanh thu theo ngày, số check-in, subscription hết hạn | lọc ngày, chi nhánh, export CSV | `GET /api/reports/revenue`, `GET /api/reports/attendance`, `GET /api/reports/subscription-expiry` |
| Audit Log | Tra cứu thay đổi quan trọng | entity, action, user, timestamp, diff summary | filter, xem chi tiết | `GET /api/audit-logs` |

## 3. Điều hướng chính

- `Dashboard`
- `Members`
- `Plans`
- `Subscriptions`
- `Payments`
- `Check-in`
- `Staff`
- `Branches`
- `Reports`
- `Audit Logs`

## 4. Màn hình ưu tiên build trước

1. Login
2. Dashboard
3. Member List
4. Create/Edit Member
5. Plan List
6. Sell Subscription
7. Payment History
8. Check-in Screen
9. Member Detail
10. Reports

## 5. Ghi chú UI/UX cho AI

- Dùng layout admin nhiều form và table, tối ưu desktop trước nhưng không bỏ qua mobile width cơ bản.
- Từ `Member Detail` phải thao tác được các flow quan trọng mà không cần chuyển màn hình quá nhiều.
- Các màn hình bán gói, thanh toán và check-in phải hiển thị cảnh báo nghiệp vụ rõ ràng, không chỉ trả lỗi kỹ thuật.
