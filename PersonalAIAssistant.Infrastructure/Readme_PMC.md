# N?u b?n mu?n t?o migration m?i ho?c c?p nh?t database thì làm theo các bu?c sau:

### 1. T? thu m?c solution root

```
cd PersonalAIAssistant.Infrastructure
```

### 2. T?o migration m?i

```
Add-Migration AddTable -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure 
```	

### 3. C?p nh?t database (apply migration)

```
Update-Database -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure
```

### 3.1. Remove migration if error
```	
Remove-Migration -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure
```

### Ho?c áp d?ng migration m?i t? thu m?c solution root
```
Update-Database [AddTable] -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure
```

### 4. Xem l?ch s? migrations

```
Get-Migration -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure
```
### 5. Migration script backup
```
Script-Migration -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure -Output migration.sql
```

### 6. Roll back migration
```
# Rollback v? migration tru?c dó (d?t tên migration mu?n quay v?)
Update-Database [TênMigrationTru?cÐó] -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure

# Rollback toàn b? v? tr?ng thái tr?ng
Update-Database 0 -StartupProject PersonalAIAssistant.API -Project PersonalAIAssistant.Infrastructure

```
### Luu ý: N?u b?n mu?n thay d?i database mà ko m?t d? li?u cu thì c?n ph?i vào file `AddProductTable.cs` trong thu m?c `Migrations` và s?a l?i hàm `Up` d? ch? thêm các tru?ng m?i mà không xóa các tru?ng cu. 