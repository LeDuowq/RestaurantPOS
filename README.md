# 🍽️ Restaurant POS - Hệ Thống Quản Lý Nhà Hàng

Dự án phần mềm **Quản lý Nhà hàng / Quán ăn** được xây dựng trên nền tảng **WPF (Windows Presentation Foundation)** áp dụng kiến trúc 3 tầng (3-Tier Architecture) kết hợp với Entity Framework Core. Đây là đồ án thực hành thuộc môn học PRN212.

---

## 🚀 Công Nghệ Sử Dụng

* **Ngôn ngữ lập trình:** C# (.NET 8.0)
* **Giao diện người dùng:** WPF (Windows Presentation Foundation)
* **Cơ sở dữ liệu:** Microsoft SQL Server
* **ORM:** Entity Framework Core 8.0 (Database First)
* **Kiến trúc ứng dụng:** 3-Tier Architecture (Business Object, Data Access Layer, Repositories, Services, UI)

---

## ✨ Tính Năng Chính

* **📋 Quản Lý Thực Đơn & Món Ăn:** Quản lý danh mục món ăn, cập nhật thông tin giá cả, hình ảnh minh họa và trạng thái khả dụng của từng món ăn trong nhà hàng.
* **🪑 Sơ Đồ Bàn Ăn & Gọi Món (POS):** Theo dõi trực quan trạng thái các bàn ăn (Bàn trống / Có khách), hỗ trợ đặt món nhanh chóng cho từng bàn.
* **👨‍🍳 Điều Phối Chế Biến Nhà Bếp:** Tiếp nhận yêu cầu gọi món từ bàn ăn, theo dõi và cập nhật tiến độ chế biến món ăn Realtime giữa Bếp và nhà hàng.
* **💳 Thanh Toán & Hóa Đơn:** Tự động tính tổng tiền hóa đơn, hỗ trợ áp dụng giảm giá, thực hiện thanh toán và tự động cập nhật lại trạng thái bàn trống.
* **🔐 Quản Lý Nhân Viên & Phân Quyền:** Quản lý tài khoản nhân viên và phân quyền truy cập chức năng hệ thống theo vai trò làm việc.

---

## 👥 Phân Quyền Hệ Thống (Roles)

Hệ thống hỗ trợ phân quyền người dùng theo 4 chức danh chính:

* **Quản Lý (Admin):** Toàn quyền quản lý hệ thống, nhân viên, danh mục món ăn, thực đơn, sơ đồ bàn và xem báo cáo doanh thu.
* **Nhân Viên Thu Ngân (Cashier):** Quản lý sơ đồ bàn ăn, thực hiện đặt món cho khách và thanh toán hóa đơn.
* **Nhân Viên Phục Vụ (Waitstaff):** Xem sơ đồ bàn ăn và kiểm tra danh sách các món ăn bàn đó đã gọi.
* **Đầu Bếp (Kitchen):** Màn hình chế biến nhà bếp, theo dõi và cập nhật trạng thái chế biến món ăn cũng như số lượng suất ăn khả dụng.

---

## 📁 Cấu Trúc Dự Án (Solution Structure)

```
RestaurantProject/
├── BusinessObject/      # Các class Entities (POCO) & DTO dữ liệu
├── DataAccessLayer/     # DbContext & các DAO thao tác trực tiếp với SQL Server
├── Repositories/        # Tầng trung gian (Repository Interfaces & Implementations)
├── Services/            # Tầng xử lý logic nghiệp vụ (Business Services)
├── Restaurant/          # Project UI khởi chạy ứng dụng WPF (Views & Code-behind)
└── Instruct/            # Thư mục lưu trữ bộ tài liệu hướng dẫn chi tiết
```

---

## 📚 Tài Liệu Hướng Dẫn

Để xem chi tiết hướng dẫn cài đặt, khởi chạy hoặc đóng gói ứng dụng, vui lòng tham khảo các tài liệu trong thư mục **[Instruct](../Instruct)**:

* 🚀 **[Hướng Dẫn Cài Đặt & Chạy App](../Instruct/HUONG_DAN_CHAY_APP.md)**: Hướng dẫn cài đặt CSDL SQL Server, nạp file script `RestaurantDB.sql`, cấu hình `appsettings.json` và khởi chạy dự án.
* 📦 **[Hướng Dẫn Đóng Gói & Triển Khai](../Instruct/HUONG_DAN_DONG_GOI.md)**: Hướng dẫn đóng gói ứng dụng (`dotnet publish`), tạo bộ cài đặt Installer (`Setup.exe`) và cấu hình máy chủ SQL Server.
