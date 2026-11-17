1. Mở phần mềm SQL Server Management Studio (hoặc Visual Studio).
2. Mở file `Database_MyStore.sql` (nằm trong thư mục Project này).
3. Bấm **Execute** (hoặc F5) để chạy toàn bộ file. Hệ thống sẽ tự động tạo Database `MyStore` và nạp sẵn dữ liệu mẫu.

## 🚀 Hướng dẫn chạy Project
1. Mở file Solution (`.sln`) bằng Visual Studio.
2. Mở file `Web.config`, kiểm tra lại chuỗi kết nối (Connection String) ở thẻ `<connectionStrings>`. 
   * Nếu máy cô dùng SQL Server mặc định, hãy sửa `Data Source` thành `localhost` hoặc `.\SQLEXPRESS`.
   * Nếu dùng LocalDB của Visual Studio, giữ nguyên `Data Source=(LocalDB)\MSSQLLocalDB`.
3. Nhấn **F5** để chạy dự án.