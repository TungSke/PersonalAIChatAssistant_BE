# Nếu bạn muốn tạo migration mới hoặc cập nhật database thì làm theo các bước sau:

### 1. Từ thư mục solution root

```
cd WaifuAIAssistant.Infrastructure
```

### 2. Tạo migration mới

```
Add-Migration AddTable -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure 
```	

### 3. Cập nhật database (apply migration)

```
Update-Database -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure
```

### 3.1. Remove migration if error
```	
Remove-Migration -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure
```

### Hoặc áp dụng migration mới từ thư mục solution root
```
Update-Database [AddTable] -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure
```

### 4. Xem lịch sử migrations

```
Get-Migration -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure
```
### 5. Migration script backup
```
Script-Migration -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure -Output migration.sql
```

### 6. Roll back migration
```
# Rollback về migration trước đó (đặt tên migration muốn quay về)
Update-Database [TênMigrationTrướcĐó] -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure

# Rollback toàn bộ về trạng thái trắng
Update-Database 0 -StartupProject WaifuAIAssistant.API -Project WaifuAIAssistant.Infrastructure

```
### Lưu ý: Nếu bạn muốn thay đổi database mà ko mất dữ liệu cũ thì cần phải vào file `AddProductTable.cs` trong thư mục `Migrations` và sửa lại hàm `Up` để chỉ thêm các trường mới mà không xóa các trường cũ. 