# Architecture Decision Record

## ADR-001: Kiến trúc tổng thể

- Quyết định: dùng `Modular Monolith`.
- Lý do:
  - Bài toán giai đoạn đầu chủ yếu là CRUD nghiệp vụ, transaction rõ, ít nhu cầu tách service.
  - Tăng tốc phát triển, debug, test và deploy.
  - Phù hợp team nhỏ và AI-assisted coding nếu module boundary rõ.
- Hệ quả:
  - Tách module trong cùng solution thay vì microservices.
  - Nếu scale sau này, có thể tách module nóng như Payments hoặc Attendance bằng dữ liệu và contract đã rõ.

## ADR-002: Stack công nghệ baseline

- Quyết định tại ngày `2026-03-20`:
  - Frontend: `Angular 21`
  - Backend: `ASP.NET Core Web API` trên `.NET 10 LTS`
  - Database: `PostgreSQL 18`
  - Cache/background khi cần: `Redis`
  - Local dev: `Docker Compose`
  - Reverse proxy khi deploy: `Nginx` hoặc tương đương
- Lý do:
  - Angular phù hợp dashboard nhiều form, table, routing và app lớn.
  - ASP.NET Core mạnh cho business API, lifecycle hỗ trợ rõ.
  - PostgreSQL phù hợp dữ liệu quan hệ, transaction và báo cáo SQL.
  - Docker Compose giúp chuẩn hóa local stack.

## ADR-003: Cấu trúc backend

- Quyết định: phân tầng `API -> Application -> Domain -> Infrastructure`.
- Lý do:
  - Tách rõ HTTP contract, use case, business rule và data access.
  - Giảm nguy cơ AI trộn logic nghiệp vụ với persistence.
- Gợi ý module:
  - `Members`
  - `Plans`
  - `Subscriptions`
  - `Payments`
  - `Attendance`
  - `Staff`
  - `Branches`
  - `Reports`
  - `Audit`

## ADR-004: Multi-tenant strategy

- Quyết định: multi-tenant đơn giản bằng cột `TenantId` trên mọi bảng nghiệp vụ.
- Lý do:
  - Nhanh triển khai, dễ debug, phù hợp giai đoạn đầu.
  - Không cần database-per-tenant ở MVP.
- Quy tắc:
  - Tenant context lấy từ identity server-side.
  - Mọi query có filter `TenantId`.
  - Unique constraint nghiệp vụ phải đi kèm `TenantId`.

## ADR-005: Authorization strategy

- Quyết định: dùng `JWT` cho authentication và `role-based authorization` cho access control.
- Lý do:
  - Phù hợp SPA + Web API.
  - Dễ kiểm soát permission theo role và branch.
- Ghi chú:
  - Authorization luôn kiểm tra thêm branch access, không chỉ role.

## ADR-006: Data lifecycle

- Quyết định:
  - Dùng `soft delete` cho entity nghiệp vụ phù hợp.
  - Không hard delete `payments` completed và `subscriptions` đã phát sinh lịch sử.
- Lý do:
  - Giữ lịch sử vận hành và dễ audit.

## ADR-007: Data access

- Quyết định: mặc định dùng `Entity Framework Core` với provider `Npgsql`.
- Lý do:
  - Phù hợp ASP.NET Core + PostgreSQL, giảm boilerplate CRUD.
  - Dễ áp dụng global query filters, migrations và optimistic concurrency khi cần.
- Ràng buộc:
  - Không bổ sung ORM khác nếu chưa có ADR mới.

## ADR-008: Frontend structure

- Quyết định:
  - Angular theo `standalone components`.
  - Route theo feature module logic, lazy load theo khu vực nghiệp vụ.
- Lý do:
  - Hợp với ứng dụng admin lớn.
  - Giữ cấu trúc rõ cho AI sinh màn hình theo feature.

## ADR-009: Cache và background work

- Quyết định: chỉ thêm Redis khi có nhu cầu rõ như dashboard cache, OTP, rate limit hoặc background processing đơn giản.
- Lý do:
  - Tránh complexity sớm.
  - MVP trước hết phải đúng nghiệp vụ.

## ADR-010: Không cho AI tự mở rộng stack

- Quyết định:
  - AI không được tự thêm framework, package lớn hoặc đổi kiến trúc nếu chưa được phê duyệt.
- Lý do:
  - Giữ repo nhất quán.
  - Tránh “framework drift” và phụ thuộc khó kiểm soát.
