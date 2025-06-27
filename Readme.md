# WaifuAIAssistant Backend

**WaifuAIAssistant** là một hệ thống trợ lý AI ảo được phát triển với ASP.NET Core, cung cấp API backend để quản lý nhân vật AI, tương tác hội thoại, cài đặt người dùng và lịch sử trò chuyện. Hệ thống hỗ trợ tích hợp AI (OpenAI, Local Model, v.v.), cho phép cá nhân hóa theo người dùng.

---

## 🚀 Công nghệ sử dụng

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core** (Database First / Code First)
- **SQL Server** (Hệ quản trị cơ sở dữ liệu chính)
- **JWT Authentication**
- **Swagger / Swashbuckle** (API Documentation)
- **Serilog** (Logging)
- **AutoMapper** (DTO Mapping)
- **Clean Architecture** (Domain-Driven Design - DDD)

---

## 🧠 Kiến trúc dự án

Dự án tuân thủ theo mô hình Clean Architecture gồm 4 tầng chính:

- `WaifuAIAssistant.Domain`: Khai báo entity, interface repository, logic nghiệp vụ thuần.
- `WaifuAIAssistant.Application`: Chứa DTO, UseCase, interface service và logic ứng dụng.
- `WaifuAIAssistant.Infrastructure`: Giao tiếp với database (EF Core), cấu hình repository.
- `WaifuAIAssistant.API`: API Endpoint, DI container, cấu hình middleware, controller, v.v.

---

## 📦 Các tính năng chính

- 🔐 **Đăng ký / Đăng nhập** với JWT Token
- 🤖 **Quản lý nhân vật AI** (ModelCharacter)
- 🗣️ **Tạo và lưu trữ đoạn hội thoại giữa người dùng và AI**
- 🧠 **Tùy chỉnh tính cách, cốt truyện của nhân vật AI**
- 📊 **Lưu lịch sử trò chuyện của người dùng**
- 🛡️ **Cấu hình và quản lý người dùng, phân quyền Role**

---