# Nếu bạn muốn tạo migration mới hoặc cập nhật database thì làm theo các bước sau:

### 1. Từ thư mục solution root

```
cd WaifuAIAssistant.Infrastructure
```

### 2. Tạo migration mới

```
dotnet ef migrations add AddTable --startup-project ../WaifuAIAssistant.API
```

### 3. Cập nhật database (apply migration)

```
dotnet ef database update --startup-project ../WaifuAIAssistant.API
```
### 3.1. Remove migration if error
```	
dotnet ef migrations remove --startup-project ../WaifuAIAssistant.API
```

### Hoặc áp dụng migration mới từ thư mục solution root
```
dotnet ef database update [MigrationName] --startup-project ../WaifuAIAssistant.API
```

### 4. Xem lịch sử migrations

```
dotnet ef migrations list --startup-project ../WaifuAIAssistant.API
```
### 5. Migration script backup
```
dotnet ef migrations script -o migration.sql ../WaifuAIAssistant.Infrastructure --startup-project ../WaifuAIAssistant.API
```

### 6. Roll back migration
```
dotnet ef database update [TênMigrationTrướcĐó] --startup-project ../WaifuAIAssistant.API
```
### Lưu ý: Nếu bạn muốn thay đổi database mà ko mất dữ liệu cũ thì cần phải vào file `AddProductTable.cs` trong thư mục `Migrations` và sửa lại hàm `Up` để chỉ thêm các trường mới mà không xóa các trường cũ. 