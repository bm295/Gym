# Business Rules

## 1. Rule chung

- Mọi dữ liệu nghiệp vụ đều bắt buộc mang `TenantId`.
- Mọi truy vấn đọc/ghi phải bị giới hạn theo `TenantId` lấy từ identity của staff đăng nhập.
- Thời gian lưu ở UTC; kiểm tra theo ngày hoặc hết hạn cần quy đổi đúng timezone tenant trước khi hiển thị.
- Mọi thao tác thay đổi dữ liệu quan trọng phải ghi `AuditLog`.

## 2. Member Rules

- Mỗi hội viên có một `MemberCode` unique trong tenant.
- Hội viên có thể ở trạng thái `Active`, `Inactive` hoặc `Suspended`.
- Hội viên `Suspended` không được check-in, không được bán gói mới cho đến khi mở lại.
- Không hard delete hội viên đã từng phát sinh subscription, payment hoặc check-in.
- Thay đổi các trường nhạy cảm như họ tên, số điện thoại, ngày sinh, trạng thái phải được audit.

## 3. Membership Plan Rules

- Gói tập chỉ được bán khi `PlanStatus = Active`.
- Gói tập phải khai báo rõ một trong hai kiểu sử dụng:
  - Theo thời hạn
  - Theo thời hạn + giới hạn số buổi
- Giá chuẩn của plan dùng làm mặc định khi bán gói, nhưng giao dịch thực tế phải lưu `sale price snapshot`.
- Khi sửa plan, subscription đã bán trước đó không bị thay đổi hiệu lực hay giá.
- Plan archived vẫn được xem lịch sử nhưng không tạo sale mới.

## 4. Subscription Rules

- Một hội viên có thể có nhiều subscription theo thời gian nhưng chỉ một subscription `Active` tại một thời điểm trong cùng tenant.
- Có thể tạo subscription kế tiếp ở trạng thái `Pending` nếu cần chuẩn bị trước, nhưng chỉ khi start date sau end date của subscription active hiện tại.
- Subscription hết hạn thì không được check-in.
- Subscription `Cancelled` hoặc `Voided` không được tính là quyền sử dụng hợp lệ.
- Subscription phải lưu snapshot của plan: tên gói, giá, thời hạn, visit limit, cross-branch policy.
- Gia hạn gói tạo subscription mới thay vì sửa trực tiếp subscription cũ.
- Nếu gói có `visit limit`, mỗi lần check-in hợp lệ sẽ trừ một lượt.

## 5. Payment Rules

- Không được xóa payment đã hoàn tất, chỉ được mark `Voided`.
- Void payment bắt buộc nhập lý do và user thực hiện phải có quyền phù hợp.
- Payment phải thuộc đúng tenant với subscription hoặc member liên quan.
- Tổng tiền `Completed` của một sale không được âm và phải phản ánh chính xác sau khi void.
- Hệ thống cho phép ghi nhận thanh toán một phần trong giai đoạn 1, nhưng subscription chỉ `Active` khi đạt điều kiện kích hoạt đã định.
- Mặc định cho MVP: sale được kích hoạt ngay nếu tổng thanh toán completed >= tổng tiền cần thu.

## 6. Check-in Rules

- Check-in chỉ hợp lệ nếu member đang `Active` và có subscription `Active`.
- Nếu subscription đã hết hạn hoặc chưa bắt đầu hiệu lực, check-in bị từ chối.
- Check-in chỉ hợp lệ tại đúng chi nhánh của subscription, trừ khi plan snapshot cho phép `AllowCrossBranch = true`.
- Hệ thống phải chặn check-in trùng lặp ngoài ý muốn trong khoảng thời gian ngắn. Giá trị mặc định là 5 phút trong cùng branch.
- Với plan giới hạn số buổi, nếu `RemainingVisits = 0` thì check-in bị từ chối.
- Check-in bị từ chối vẫn có thể ghi log nghiệp vụ ở mức application log, nhưng không tạo bản ghi attendance chính thức.

## 7. Branch Rules

- Branch thuộc duy nhất một tenant.
- Staff chỉ xem và thao tác trên branch được cấp quyền, trừ `TenantAdmin`.
- Báo cáo theo branch phải chỉ tính dữ liệu phát sinh tại branch đó.
- Subscription có `HomeBranchId` để xác định chi nhánh mặc định khi gói không hỗ trợ cross-branch.

## 8. Staff & Authorization Rules

- `TenantAdmin` xem toàn bộ dữ liệu tenant.
- `BranchManager` xem dữ liệu trong các branch được gán.
- `Receptionist` được tạo hội viên, bán gói, thanh toán và check-in trong phạm vi branch được gán.
- `Trainer` chỉ xem danh sách hội viên liên quan và attendance history được cho phép; không void payment.
- Các quyền ghi đè đặc biệt, nếu có, phải cấu hình rõ thay vì suy ra ngầm.

## 9. Audit & Soft Delete Rules

- Soft delete áp dụng cho member, plan, branch, staff user khi phù hợp.
- Subscription và payment không hard delete trong vận hành thường ngày.
- Audit log bắt buộc cho:
  - tạo hoặc sửa member
  - bán gói hoặc gia hạn
  - ghi nhận payment
  - void payment
  - khóa hoặc mở staff
  - thay đổi phân quyền

## 10. Multi-tenant Rules

- Không cho phép query hoặc command vượt ra ngoài tenant của user đăng nhập.
- Mọi index và unique constraint nghiệp vụ phải xét cùng `TenantId`.
- Không dùng tenant id do client gửi lên làm nguồn sự thật cho authorization; tenant context lấy từ token hoặc server-side context.
