# Product Scope / Vision

## 1. Bối cảnh sản phẩm

Gym CRM SaaS là phần mềm quản lý khách hàng cho phòng gym theo mô hình thuê bao nhiều tenant. Mục tiêu của giai đoạn 1 là giải quyết bài toán vận hành hằng ngày cho một phòng gym hoặc chuỗi phòng gym nhỏ, tập trung vào dữ liệu hội viên, gói tập, bán gói, thanh toán, check-in và báo cáo cơ bản.

## 2. Vấn đề cần giải quyết

- Dữ liệu hội viên đang phân tán qua Excel, Zalo, sổ tay hoặc nhiều hệ thống rời rạc.
- Lễ tân mất thời gian tìm hội viên, bán gói, kiểm tra hạn sử dụng và check-in.
- Quản lý khó theo dõi doanh thu, lịch sử thanh toán, số lượng hội viên active và tình hình đi tập.
- Chuỗi phòng gym cần tách dữ liệu theo tenant nhưng vẫn quản lý nhiều chi nhánh trong cùng một tenant.

## 3. Người dùng chính

- `Tenant Admin`: cấu hình tenant, quản lý staff, xem toàn bộ dữ liệu tenant.
- `Branch Manager`: quản lý vận hành và báo cáo trong một hoặc nhiều chi nhánh được phân quyền.
- `Receptionist`: tạo hội viên, bán gói, ghi nhận thanh toán, check-in.
- `Trainer`: xem danh sách học viên được phân công và lịch sử đi tập ở phạm vi được cấp quyền.

## 4. Mục tiêu giai đoạn MVP

- Quản lý hồ sơ hội viên và trạng thái hoạt động.
- Quản lý danh mục gói tập theo tenant.
- Bán gói tập, gia hạn gói tập, theo dõi subscription theo thời gian.
- Ghi nhận thanh toán tiền mặt và chuyển khoản.
- Check-in cơ bản tại quầy.
- Xem lịch sử hội viên gồm gói tập, thanh toán, check-in.
- Báo cáo tổng quan đơn giản theo ngày, tuần, tháng.
- Hỗ trợ multi-tenant mức cơ bản bằng `TenantId`.

## 5. Không thuộc phạm vi giai đoạn 1

- PT schedule nâng cao và quản lý ca huấn luyện chi tiết.
- Tự động gia hạn qua cổng thanh toán online.
- Ứng dụng mobile cho hội viên.
- Marketing automation, loyalty point, voucher, referral.
- Microservices, event bus phức tạp hoặc workflow engine.
- Tính tiền sản phẩm retail, POS đầy đủ, kho hàng.

## 6. Phạm vi nghiệp vụ MVP

### 6.1 Module bắt buộc

- Members
- Membership Plans
- Subscriptions
- Payments
- Attendance / Check-in
- Staff & Role-based Access
- Branches
- Dashboard & Reports cơ bản

### 6.2 Dữ liệu cốt lõi cần theo dõi

- Hồ sơ hội viên và trạng thái khóa/mở
- Gói tập và chính sách sử dụng
- Subscription theo từng lần mua hoặc gia hạn
- Thanh toán, phương thức thanh toán, lịch sử void
- Nhật ký check-in
- Audit log cho thay đổi quan trọng

## 7. Phạm vi kỹ thuật chốt cho tài liệu này

- Frontend: Angular 21, ưu tiên standalone architecture
- Backend: ASP.NET Core Web API trên .NET 10 LTS
- Database: PostgreSQL 18
- Cache / background khi cần: Redis
- Local development: Docker Compose
- Deploy: containerized app sau reverse proxy như Nginx
- Kiến trúc: Modular Monolith, phân tầng `API -> Application -> Domain -> Infrastructure`

## 8. Success Criteria cho MVP

- Lễ tân có thể hoàn tất quy trình tạo hội viên mới trong dưới 2 phút.
- Bán gói và ghi nhận thanh toán hoàn tất trong một flow không quá 5 bước chính.
- Check-in tại quầy trả kết quả thành công hoặc lỗi hợp lệ trong dưới 2 giây ở điều kiện bình thường.
- Dữ liệu tenant này không truy cập được từ tenant khác.
- Quản lý có thể xem doanh thu theo ngày và danh sách hội viên active mà không cần xuất Excel.

## 9. Giả định sản phẩm

- Mỗi tenant là một doanh nghiệp gym hoặc thương hiệu gym.
- Một tenant có thể có nhiều chi nhánh.
- Người dùng nội bộ đăng nhập bằng tài khoản staff, không phải tài khoản hội viên.
- Hội viên chỉ cần được quản lý trên web admin trong giai đoạn 1.
- Check-in giai đoạn 1 là thao tác tại quầy, chưa yêu cầu kiosk riêng hoặc QR self-service.

## 10. Ràng buộc triển khai

- Không thêm framework ngoài stack đã chốt nếu chưa được phê duyệt.
- Ưu tiên CRUD nghiệp vụ rõ ràng và testable hơn tối ưu hóa sớm.
- Toàn bộ thời gian hệ thống lưu ở UTC, hiển thị theo timezone tenant khi cần.
- Dữ liệu nghiệp vụ quan trọng dùng soft delete hoặc trạng thái thay vì hard delete.
