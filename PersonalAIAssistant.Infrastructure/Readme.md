# Nếu bạn muốn tạo migration mới hoặc cập nhật database thì làm theo các bước sau:

### 1. Từ thư mục solution root
```bash
cd PersonalAIAssistant.Infrastructure
```

### 2. Tạo migration mới
```bash
dotnet ef migrations add AddTable --startup-project ../PersonalAIAssistant.API
```

### 3. Cập nhật database
```bash
dotnet ef database update --startup-project ../PersonalAIAssistant.API
```

### 3.1. Remove migration
```bash
dotnet ef migrations remove --startup-project ../PersonalAIAssistant.API
```

### 4. Xem lịch sử
```bash
dotnet ef migrations list --startup-project ../PersonalAIAssistant.API
```

### 5. Script migration
```bash
dotnet ef migrations script -o migration.sql --startup-project ../PersonalAIAssistant.API
```

### 6. Rollback
```bash
dotnet ef database update [TênMigration] --startup-project ../PersonalAIAssistant.API
dotnet ef database update 0 --startup-project ../PersonalAIAssistant.API   # rollback hết
```

### Lưu ý: Nếu bạn muốn thay đổi database mà ko mất dữ liệu cũ thì cần phải vào file `AddProductTable.cs` trong thư mục `Migrations` và sửa lại hàm `Up` để chỉ thêm các trường mới mà không xóa các trường cũ. 