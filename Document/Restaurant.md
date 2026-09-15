# 🍽️ Restaurant Management System (POS)

## 📖 Giới thiệu dự án
Đây là phần mềm Quản lý nhà hàng (Point of Sale - POS) được phát triển trên nền tảng Desktop dành cho hệ điều hành Windows. Ứng dụng giúp số hóa quy trình bán hàng, từ việc quản lý sơ đồ bàn ăn, gọi món, đến thanh toán và quản lý danh mục dữ liệu của nhà hàng một cách trực quan và hiệu quả.

## 💻 Công nghệ sử dụng
Dự án được xây dựng dựa trên các công nghệ và framework chuẩn công nghiệp của Microsoft:
* **Ngôn ngữ lập trình:** C#
* **Framework giao diện:** WPF (Windows Presentation Foundation) trên nền .NET Core / .NET 5+
* **ORM (Object-Relational Mapping):** Entity Framework Core
* **Hệ quản trị CSDL:** SQL Server

## 🏗️ Kiến trúc hệ thống (3-Tier Architecture)
Dự án áp dụng chặt chẽ kiến trúc 3 tầng (N-Tier) kết hợp với Repository Pattern nhằm đảm bảo tính toàn vẹn của dữ liệu, dễ dàng bảo trì và mở rộng code:
1. **BusinessObjects:** Chứa các lớp thực thể (Entity Classes / POCO) đại diện cho các bảng trong CSDL.
2. **DataAccessLayer (DAL):** Quản lý cấu hình `DbContext` và các lớp Data Access Object (DAO) để thực thi truy vấn trực tiếp xuống SQL Server.
3. **Repositories & Services (BLL):** Lớp trung gian xử lý logic nghiệp vụ (Business Logic), tiếp nhận yêu cầu từ UI và gọi xuống DAL.
4. **WPFApp (Presentation Layer):** Chứa các file giao diện XAML và code-behind xử lý tương tác của người dùng.

## 🗄️ Cấu trúc cơ sở dữ liệu (Database Schema)
Hệ thống sử dụng cơ sở dữ liệu thiết kế theo mô hình quan hệ, bao gồm 7 bảng cốt lõi:
* `Role`: Lưu danh sách các chức danh hệ thống (1: Quản lí, 2: Thu ngân, 3: Phục vụ, 4: Đầu bếp).
* `Account`: Quản lý thông tin và phân quyền đăng nhập của nhân viên (Admin/Cashier/Waitstaff/Chef).
* `DiningTable`: Lưu trữ thông tin sơ đồ bàn ăn và trạng thái bàn (Trống / Đang có khách).
* `Category`: Phân loại thực đơn (ví dụ: Đồ uống, Món chính, Tráng miệng).
* `FoodItem`: Thông tin chi tiết về món ăn, giá cả, ảnh minh họa và **số lượng suất ăn sẵn có/tồn kho (`Quantity`)**.
* `Order`: Quản lý các hóa đơn (phiếu gọi món) của khách hàng, gắn liền với bàn ăn và trạng thái thanh toán.
* `OrderDetail`: Chi tiết hóa đơn (khách gọi món gì, số lượng bao nhiêu, đơn giá, ghi chú và **trạng thái chế biến món (`Status`: Chờ chế biến / Đang làm / Hoàn thành)**).

## ✨ Chức năng cốt lõi

### 1. Phân quyền và Bảo mật
* Đăng nhập hệ thống với cơ chế xác thực tài khoản.
* Phân quyền chức năng theo Role: 
  * **Quản lý (Role = 1):** Toàn quyền truy cập tất cả các màn hình (Bán hàng, Quản lý thực đơn, Bàn ăn, Tài khoản, Thống kê, Màn hình Bếp).
  * **Thu ngân (Role = 2) & Phục vụ (Role = 3):** Màn hình bán hàng, sơ đồ bàn và order món.
  * **Đầu bếp (Role = 4):** Màn hình Nhà Bếp (điều phối chế biến món ăn và quản lý số lượng suất ăn).

### 2. Màn hình Bán hàng (POS - Dành cho Thu ngân & Phục vụ)
* **Quản lý sơ đồ bàn:** Hiển thị trực quan danh sách bàn ăn với mã màu báo hiệu trạng thái (Bàn trống / Bàn đang phục vụ).
* **Gọi món & Kiểm tra số lượng tồn:** Hiển thị danh sách món ăn kèm số suất ăn còn lại (`Còn: X suất`). Khi khách order, hệ thống tự động trừ số lượng tồn kho và ngăn chặn order nếu món ăn đã hết suất phục vụ.
* **Theo dõi tiến độ chế biến:** Hiển thị trạng thái chế biến thực tế của từng món ("Chờ chế biến", "Đang làm", "Hoàn thành") được cập nhật từ Nhà Bếp.
* **Thanh toán:** Tự động tính tổng tiền hóa đơn, cập nhật trạng thái hóa đơn và giải phóng bàn (chuyển bàn về trạng thái trống) sau khi thanh toán thành công.

### 3. Màn hình Quản trị (Dành cho Quản lý)
* **CRUD Danh mục & Món ăn:** Thêm, Đọc, Sửa, Xóa các danh mục, chi tiết món ăn và cài đặt số lượng tồn kho khởi tạo.
* **CRUD Bàn ăn:** Mở rộng quy mô nhà hàng bằng cách quản lý số lượng bàn.
* **CRUD Nhân sự:** Quản lý tài khoản đăng nhập của nhân viên và phân quyền Role (bao gồm quyền Đầu bếp).

### 4. Màn hình Nhà Bếp (KDS - Dành cho Đầu bếp)
* **Quản lý số lượng suất ăn:** Cập nhật nhanh số lượng suất ăn nhà hàng có thể đáp ứng trong ngày.
* **Điều phối chế biến món ăn:** Tiếp nhận món từ các bàn order, cập nhật trạng thái chế biến từ `Chờ chế biến` ➔ `Đang chế biến` ➔ `Hoàn thành`.

## 🚀 Hướng dẫn cài đặt và chạy dự án
1. **Clone repository** về máy tính cục bộ.
2. **Cấu hình Database:**
   * Mở SQL Server và chạy file script `RestaurantDB.sql` để khởi tạo CSDL, các bảng và dữ liệu mẫu.
   * Cấu hình chuỗi kết nối (`ConnectionString`) trong file `appsettings.json` của project `Restaurant` cho khớp với SQL Server của bạn.
3. **Build Solution:** Mở solution bằng Visual Studio, nhấn `Ctrl + Shift + B` để khôi phục các package NuGet (như Entity Framework Core) và build các project.
4. Chọn project `Restaurant` làm **Startup Project**.
5. Nhấn `F5` để chạy ứng dụng.

---

📋 Module 1: Quản lý Bán hàng (Dành cho Thu ngân & Phục vụ)
Đây là module xương sống của ứng dụng, nơi thu ngân và phục vụ thao tác hàng ngày.

UC1.1: Quản lý phiên sử dụng bàn: Chọn bàn trống để mở Order mới, hoặc bấm vào bàn đang có khách để xem chi tiết hóa đơn.

UC1.2: Cập nhật Order (Gọi món): Tìm kiếm món ăn, thêm món vào bàn, tự động kiểm tra số lượng tồn (`Quantity`), trừ số suất ăn khả dụng và hiển thị cảnh báo khi hết món.

UC1.3: Chuyển bàn / Gộp bàn: Chuyển toàn bộ dữ liệu Order từ bàn này sang bàn khác, hoặc gộp 2 hóa đơn của 2 bàn lại thành 1.

UC1.4: Thanh toán & In biên lai: Tính tổng tiền, chuyển trạng thái Order sang "Đã thanh toán", giải phóng bàn (chuyển trạng thái bàn về "Trống") và xuất hóa đơn.

📋 Module 2: Quản lý Thực đơn & Sơ đồ bàn (Dành cho Admin)
Module này dùng để thiết lập dữ liệu nền tảng cho nhà hàng.

UC2.1: Quản lý Danh mục (CRUD Category): Thêm, xem, sửa, xóa các nhóm đồ ăn (Ví dụ: Đồ nướng, Lẩu, Nước giải khát).

UC2.2: Quản lý Món ăn (CRUD FoodItem): Thêm món mới, cập nhật giá bán, số lượng tồn kho (`Quantity`), đổi tên món hoặc ngừng bán một món.

UC2.3: Quản lý Sơ đồ bàn ăn (CRUD DiningTable): Khai báo thêm bàn mới khi nhà hàng mở rộng, đổi tên bàn (VD: Bàn VIP 1) hoặc tạm ẩn bàn đang hỏng/bảo trì.

🔐 Module 3: Quản lý Hệ thống & Tài khoản (Dành cho Admin)
Bảo mật và kiểm soát ai được quyền làm gì trong ứng dụng.

UC3.1: Đăng nhập / Đăng xuất: Xác thực thông tin người dùng dựa trên Username và Password. Điều hướng giao diện dựa trên quyền (Role: Quản lý, Thu ngân, Phục vụ, Đầu bếp).

UC3.2: Quản lý Nhân sự (CRUD Account): Tạo tài khoản mới cho nhân viên, phân quyền Role Đầu bếp (Role 4), đổi mật khẩu hoặc vô hiệu hóa tài khoản của nhân viên đã nghỉ việc.

📈 Module 4: Thống kê & Báo cáo (Dành cho Admin)
Các tính năng giúp theo dõi tình hình kinh doanh.

UC4.1: Báo cáo Doanh thu: Lọc và xem tổng doanh thu theo ngày, khoảng thời gian dựa trên các Order đã thanh toán.

UC4.2: Thống kê Món bán chạy (Top-sellers): Liệt kê các món ăn có số lượng order cao nhất.

UC4.3: Xuất báo cáo: Chuyển đổi dữ liệu thống kê trên DataGrid ra file định dạng CSV/Excel.

👨‍🍳 Module 5: Quản lý Nhà Bếp & Điều phối món (Dành cho Đầu bếp)
Module chuyên biệt giúp kết nối khu vực chế biến với bộ phận phục vụ.

UC5.1: Quản lý Suất ăn Khả dụng: Cho phép Đầu bếp xem danh sách thực đơn và cập nhật nhanh số lượng suất ăn nhà hàng có thể cung cấp trong ngày.

UC5.2: Điều phối Chế biến (Kitchen Display System): Tiếp nhận danh sách các món ăn được bồi bàn order theo từng bàn, nhấn chuyển trạng thái chế biến (`Chờ chế biến` ➔ `Đang chế biến` ➔ `Hoàn thành`).

UC5.3: Đồng bộ Trạng thái Realtime: Trạng thái món ăn sau khi Đầu bếp cập nhật sẽ hiển thị tức thì trên màn hình hóa đơn của Thu ngân/Bồi bàn.