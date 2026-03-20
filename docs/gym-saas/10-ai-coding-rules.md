# AI Coding Rules / Prompting Guide

## 1. Mục tiêu của tài liệu này

Tài liệu này là hàng rào để AI code đúng domain gym SaaS, không tự bịa thêm chức năng, không phá cấu trúc repo và không lan sang module không liên quan.

## 2. Nguồn sự thật

- Product scope là nguồn sự thật cho phạm vi chức năng.
- Domain model và business rules là nguồn sự thật cho nghiệp vụ.
- API contract draft là nguồn sự thật cho HTTP contract nếu chưa có implementation.
- Nếu có mâu thuẫn, ưu tiên theo thứ tự:
  1. Business rules
  2. Use cases
  3. API contract
  4. Functional requirements

## 3. Coding conventions

- Dùng tiếng Anh cho tên class, method, variable, folder, table, API path.
- Dùng tiếng Việt hoặc tiếng Anh nhất quán cho nội dung docs và comment, nhưng code ưu tiên tiếng Anh.
- Không hardcode business rule trong UI nếu rule đã thuộc backend.
- Mọi datetime trong code phải dùng UTC ở backend.
- Mọi entity nghiệp vụ phải có `TenantId`.

## 4. Folder structure mong muốn

### Backend

- `src/Api`
- `src/Application`
- `src/Domain`
- `src/Infrastructure`
- `tests/UnitTests`
- `tests/IntegrationTests`

### Frontend

- `src/app/core`
- `src/app/shared`
- `src/app/features/members`
- `src/app/features/plans`
- `src/app/features/subscriptions`
- `src/app/features/payments`
- `src/app/features/checkins`
- `src/app/features/staff`
- `src/app/features/reports`

## 5. Quy tắc tạo và sửa file

- Chỉ tạo file mới khi thật sự cần cho một use case hoặc module đã có trong scope.
- Không đổi tên file, folder hoặc namespace hàng loạt nếu chưa được yêu cầu.
- Không sửa module unrelated để “cleanup tiện tay”.
- Khi sửa business rule quan trọng, phải cập nhật test liên quan.
- Không thêm seed data vào production code.

## 6. Quy tắc thêm package

- Không thêm package mới nếu chưa có yêu cầu rõ hoặc chưa được cho phép trong ADR.
- Package mặc định được phép:
  - Angular core ecosystem
  - ASP.NET Core built-in packages
  - EF Core + Npgsql
  - Docker / container config cần thiết
- Bất kỳ UI kit, state library, mapping library hoặc validation library nào ngoài baseline đều phải hỏi lại trước.

## 7. Quy tắc khi code backend

- Tách command/query rõ theo use case.
- Business rule phải nằm ở Application hoặc Domain, không nằm trực tiếp trong controller.
- Controller chỉ xử lý HTTP concern.
- Repository hoặc DbContext không được chứa logic domain phức tạp.
- Không bypass tenant filter.
- Không dùng hard delete cho payment completed hoặc subscription có lịch sử.

## 8. Quy tắc khi code frontend

- Tổ chức theo feature, không theo technical type thuần túy.
- Form phải có validation rõ và message thân thiện cho lễ tân.
- Table list phải hỗ trợ filter, sort và pagination server-side.
- Không hardcode mock data vào component production.
- Không tạo state management phức tạp nếu service + component state là đủ.

## 9. Quy tắc test

- Phải viết unit test cho business rule quan trọng:
  - chỉ một active subscription
  - subscription expired không được check-in
  - payment completed không được hard delete
  - cross-branch check-in bị chặn nếu plan không cho phép
- Viết integration test cho flow quan trọng:
  - tạo hội viên
  - bán gói
  - ghi nhận payment
  - check-in

## 10. Khi nào AI phải hỏi lại

- Khi yêu cầu mâu thuẫn với business rules hiện tại.
- Khi cần thêm package hoặc framework ngoài baseline.
- Khi thay đổi schema làm ảnh hưởng dữ liệu hiện có.
- Khi phải quyết định rule nghiệp vụ chưa được mô tả rõ.
- Khi cần sửa module ngoài phạm vi task hiện tại.

## 11. Khi nào AI không cần hỏi lại

- Tạo DTO, validator, handler, service hoặc component nhỏ để hoàn tất use case đã rõ.
- Thêm test còn thiếu cho rule đã có.
- Tạo migration phù hợp với data dictionary đã chốt.
- Thêm endpoint hoặc màn hình đúng với API draft và screen list.

## 12. Definition of Done cho mỗi task AI

- Code build được.
- Không phá tenant isolation.
- Không phá business rules đã chốt.
- Có test cho logic quan trọng mới thêm hoặc thay đổi.
- Có cập nhật doc nếu task làm thay đổi contract hoặc rule nghiệp vụ.
