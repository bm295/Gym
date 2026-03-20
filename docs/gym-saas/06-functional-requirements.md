# Functional Requirements

## 1. Authentication & Authorization

- `FR-AUTH-001`: Hệ thống phải cho phép staff đăng nhập bằng username/email và password.
- `FR-AUTH-002`: Hệ thống phải cấp access token cho các request API đã xác thực.
- `FR-AUTH-003`: Hệ thống phải áp dụng role-based authorization theo `TenantAdmin`, `BranchManager`, `Receptionist`, `Trainer`.
- `FR-AUTH-004`: Hệ thống phải giới hạn truy cập theo `TenantId` và danh sách branch được cấp.

## 2. Member Management

- `FR-MEM-001`: Tạo hội viên mới.
- `FR-MEM-002`: Chỉnh sửa thông tin hội viên.
- `FR-MEM-003`: Tìm kiếm hội viên theo tên, số điện thoại, mã hội viên.
- `FR-MEM-004`: Filter hội viên theo branch, status, có gói active hay không.
- `FR-MEM-005`: Khóa hoặc mở lại hội viên.
- `FR-MEM-006`: Xem chi tiết hội viên và timeline tổng hợp.

## 3. Plan Management

- `FR-PLAN-001`: Tạo gói tập tháng, quý, năm hoặc custom duration.
- `FR-PLAN-002`: Cấu hình giá, thời hạn, visit limit, cross-branch policy.
- `FR-PLAN-003`: Chỉnh sửa plan còn hiệu lực cho sale tương lai.
- `FR-PLAN-004`: Archive plan thay vì hard delete.
- `FR-PLAN-005`: Tra cứu danh sách plan theo status.

## 4. Subscription & Sales

- `FR-SUB-001`: Bán gói tập cho hội viên.
- `FR-SUB-002`: Gia hạn gói bằng cách tạo subscription mới.
- `FR-SUB-003`: Xem danh sách subscription theo status, branch, plan, ngày hết hạn.
- `FR-SUB-004`: Ngăn chặn trùng subscription active trong cùng tenant.
- `FR-SUB-005`: Lưu snapshot plan tại thời điểm sale.
- `FR-SUB-006`: Tự động cập nhật trạng thái subscription theo điều kiện thanh toán và thời hạn.

## 5. Payment Management

- `FR-PAY-001`: Ghi nhận thanh toán tiền mặt.
- `FR-PAY-002`: Ghi nhận thanh toán chuyển khoản.
- `FR-PAY-003`: Hỗ trợ nhiều payment cho một subscription sale.
- `FR-PAY-004`: Xem lịch sử thanh toán theo hội viên, ngày, branch, method, status.
- `FR-PAY-005`: Void payment với lý do bắt buộc.
- `FR-PAY-006`: Ghi audit log cho mọi payment action.

## 6. Attendance

- `FR-ATT-001`: Check-in hội viên bằng member code hoặc phone number.
- `FR-ATT-002`: Từ chối check-in nếu subscription không hợp lệ.
- `FR-ATT-003`: Từ chối check-in sai branch nếu plan không cho phép cross-branch.
- `FR-ATT-004`: Chống check-in trùng lặp trong khoảng thời gian cấu hình.
- `FR-ATT-005`: Xem lịch sử check-in theo member, ngày, branch.

## 7. Staff & Branch

- `FR-STAFF-001`: Quản lý staff user nội bộ.
- `FR-STAFF-002`: Gán role cho staff.
- `FR-STAFF-003`: Gán branch access cho staff.
- `FR-STAFF-004`: Khóa hoặc mở lại tài khoản staff.
- `FR-BRANCH-001`: Tạo và chỉnh sửa branch.
- `FR-BRANCH-002`: Archive branch không còn hoạt động.

## 8. Reports

- `FR-REP-001`: Dashboard hiển thị active members, check-ins hôm nay, revenue hôm nay.
- `FR-REP-002`: Báo cáo doanh thu theo ngày, tuần, tháng và branch.
- `FR-REP-003`: Báo cáo attendance theo ngày và branch.
- `FR-REP-004`: Danh sách subscription sắp hết hạn trong khoảng thời gian lọc.

## 9. Audit & Data Lifecycle

- `FR-AUD-001`: Ghi audit log cho thay đổi dữ liệu quan trọng.
- `FR-AUD-002`: Soft delete đối với entity phù hợp thay vì hard delete.
- `FR-AUD-003`: Không cho hard delete payment completed và subscription đã phát sinh.

## 10. Baseline Non-Functional Requirements

- `NFR-001`: Dữ liệu tenant này không được lộ sang tenant khác.
- `NFR-002`: Các màn hình list/search phổ biến nên phản hồi trong mục tiêu dưới 2 giây với dữ liệu quy mô MVP.
- `NFR-003`: Tất cả thay đổi payment và chỉnh sửa thông tin nhạy cảm phải được audit.
- `NFR-004`: Hệ thống phải hỗ trợ backup và restore database theo quy trình vận hành.
- `NFR-005`: Thông tin cá nhân cơ bản của hội viên phải được bảo vệ theo nguyên tắc least privilege.
- `NFR-006`: Dữ liệu nghiệp vụ cần hỗ trợ soft delete hoặc trạng thái tương đương để tránh mất lịch sử.
