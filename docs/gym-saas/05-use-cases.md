# Use Cases / User Flows

## 1. UC-01 Tạo hội viên mới

### Input

- full name
- phone number
- date of birth, gender, địa chỉ nếu có
- home branch

### Preconditions

- User có quyền tạo hội viên trong branch tương ứng.

### Flow chính

1. User mở màn hình tạo hội viên.
2. Nhập thông tin bắt buộc.
3. Hệ thống validate dữ liệu cơ bản và kiểm tra `MemberCode` mới.
4. Hệ thống tạo member ở trạng thái `Active` hoặc `Inactive` theo cấu hình mặc định.
5. Hệ thống ghi audit log và trả về member detail.

### Kết quả

- Member mới được tạo thành công với `MemberCode` unique.

### Error cases

- Thiếu trường bắt buộc.
- User không có quyền ở branch này.
- Dữ liệu không hợp lệ theo validation format.

## 2. UC-02 Bán gói tập cho hội viên

### Input

- member id
- plan id
- start date
- home branch
- sale price
- danh sách payment ban đầu

### Preconditions

- Member không bị suspended.
- Plan ở trạng thái active.
- Member không có subscription active xung đột.

### Flow chính

1. User mở `Sell Subscription`.
2. Chọn hội viên và plan.
3. Hệ thống hiển thị giá chuẩn, policy plan và trạng thái subscription hiện tại.
4. User chỉnh `sale price` nếu được quyền và nhập payment ban đầu.
5. Hệ thống tạo subscription với plan snapshot.
6. Hệ thống tạo payment tương ứng.
7. Nếu đủ điều kiện kích hoạt thì subscription chuyển `Active`.
8. Hệ thống ghi audit log và trả kết quả sale.

### Kết quả

- Subscription mới được tạo.
- Payment được ghi nhận.
- Member có quyền sử dụng nếu subscription active.

### Error cases

- Plan archived hoặc inactive.
- Member đang bị suspended.
- Đã tồn tại subscription active xung đột.
- Payment amount không hợp lệ.

## 3. UC-03 Gia hạn gói tập

### Input

- subscription id hiện tại hoặc member id
- plan id mới hoặc plan cũ
- renewal start date
- payment list

### Preconditions

- User có quyền bán gói.
- Member tồn tại và không bị suspended.

### Flow chính

1. User chọn gia hạn từ member detail hoặc subscription detail.
2. Hệ thống nạp subscription hiện tại và đề xuất ngày bắt đầu mới.
3. User xác nhận plan, giá, payment.
4. Hệ thống tạo subscription mới thay vì sửa bản ghi cũ.
5. Hệ thống tạo payment và tính trạng thái activation.
6. Hệ thống ghi audit log.

### Kết quả

- Lịch sử subscription được bảo toàn.
- Subscription mới được liên kết đúng với member.

### Error cases

- Renewal start date chồng với active subscription.
- Plan không khả dụng.
- User không có quyền.

## 4. UC-04 Ghi nhận thanh toán bổ sung

### Input

- subscription id hoặc sale reference
- amount
- payment method
- paid at
- note

### Preconditions

- Sale tồn tại và chưa bị void toàn bộ.

### Flow chính

1. User mở payment history hoặc member detail.
2. Chọn giao dịch cần thu thêm.
3. Nhập thông tin thanh toán.
4. Hệ thống validate amount và method.
5. Hệ thống tạo payment completed.
6. Nếu sale đạt điều kiện kích hoạt, hệ thống cập nhật subscription sang active.
7. Hệ thống ghi audit log.

### Kết quả

- Payment mới được thêm vào lịch sử.

### Error cases

- Amount <= 0.
- Subscription không tồn tại.
- User không có quyền ở branch liên quan.

## 5. UC-05 Void payment

### Input

- payment id
- void reason

### Preconditions

- User có quyền void payment.
- Payment đang ở trạng thái completed.

### Flow chính

1. User chọn payment cần void.
2. Hệ thống yêu cầu xác nhận và nhập lý do.
3. Hệ thống đổi trạng thái payment sang `Voided`.
4. Hệ thống tính lại tình trạng tài chính của sale.
5. Nếu cần, hệ thống cập nhật subscription về trạng thái phù hợp theo rule.
6. Hệ thống ghi audit log đầy đủ trước và sau thay đổi.

### Kết quả

- Payment bị vô hiệu hóa về mặt nghiệp vụ nhưng vẫn giữ lịch sử.

### Error cases

- Payment không ở trạng thái completed.
- Thiếu void reason.
- User không có quyền.

## 6. UC-06 Check-in hội viên

### Input

- member code hoặc phone number
- branch id nơi check-in

### Preconditions

- User có quyền check-in tại branch.

### Flow chính

1. User nhập mã hội viên hoặc số điện thoại tại màn hình check-in.
2. Hệ thống tìm member trong tenant hiện tại.
3. Hệ thống kiểm tra trạng thái member và subscription active.
4. Hệ thống kiểm tra branch policy và visit limit.
5. Nếu hợp lệ, hệ thống tạo check-in record.
6. Nếu plan giới hạn số buổi, hệ thống trừ remaining visits.
7. Hệ thống trả thông báo thành công cho quầy.

### Kết quả

- Một bản ghi check-in được tạo thành công.

### Error cases

- Không tìm thấy hội viên.
- Subscription hết hạn hoặc chưa active.
- Không đúng branch và plan không cho phép cross-branch.
- Đã check-in trùng trong thời gian chặn.
- Hết số lượt tập.

## 7. UC-07 Xem lịch sử hội viên

### Input

- member id

### Preconditions

- User có quyền xem member thuộc branch hoặc tenant tương ứng.

### Flow chính

1. User mở member detail.
2. Hệ thống tải profile, active subscription, subscription history, payment history, check-in history.
3. Hệ thống trả timeline sắp xếp theo thời gian giảm dần.

### Kết quả

- User có thể tra cứu toàn bộ lịch sử liên quan của hội viên trên một màn hình.

### Error cases

- Không tìm thấy member trong tenant hiện tại.
- User không có quyền truy cập branch liên quan.
