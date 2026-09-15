# 🗄️ Cấu trúc Cơ sở dữ liệu RestaurantPOS

Dưới đây là Script SQL Server để khởi tạo cơ sở dữ liệu cho phần mềm Quản lý Nhà hàng / Quán ăn (Restaurant POS).

```sql
-- 1. Tạo Database
CREATE DATABASE RestaurantPOS;
GO

USE RestaurantPOS;
GO

-- 2. Tạo bảng Role (Chức vụ)
CREATE TABLE Role (
    RoleID INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);
GO

-- 3. Tạo bảng Account (Tài khoản nhân viên)
CREATE TABLE Account (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role INT NOT NULL DEFAULT 3, -- 1: Admin, 2: Thu ngân, 3: Phục vụ
    Email NVARCHAR(100) NULL,
    Status INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Account_Role FOREIGN KEY (Role) REFERENCES Role(RoleID)
);
GO

-- 4. Tạo bảng DiningTable (Bàn ăn)
CREATE TABLE DiningTable (
    TableID INT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(50) NOT NULL, -- Vd: Bàn 1, Bàn 2, VIP 1
    Status INT NOT NULL DEFAULT 0    -- 0: Trống, 1: Đang có khách
);
GO

-- 5. Tạo bảng Category (Danh mục món ăn)
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- 6. Tạo bảng FoodItem (Món ăn)
CREATE TABLE FoodItem (
    FoodID INT IDENTITY(1,1) PRIMARY KEY,
    FoodName NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,0) NOT NULL,    -- Dùng DECIMAL để lưu tiền Việt (VND)
    CategoryID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 50, -- (Mới) Số lượng suất ăn sẵn có/tồn kho
    Img NVARCHAR(MAX) NULL,          -- (Mới thêm) Lưu đường dẫn/tên ảnh của món ăn
    IsAvailable BIT NOT NULL DEFAULT 1, -- (Mới thêm cho Module 2) Trạng thái còn/hết hàng
    CONSTRAINT FK_Food_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);
GO

-- 7. Tạo bảng Orders (Hóa đơn)
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    TableID INT NOT NULL,
    AccountID INT NOT NULL,          -- Nhân viên nào lập hóa đơn này
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    CheckoutDate DATETIME NULL,      -- (Mới thêm cho Module 4) Dùng để lọc doanh thu theo ngày chính xác
    TotalPrice DECIMAL(18,0) DEFAULT 0,
    Discount DECIMAL(18,0) NOT NULL DEFAULT 0, -- (Mới thêm cho Module 4) Để tính toán doanh thu thực tế
    Status INT NOT NULL DEFAULT 0,   -- 0: Chưa thanh toán, 1: Đã thanh toán
    CONSTRAINT FK_Order_Table FOREIGN KEY (TableID) REFERENCES DiningTable(TableID),
    CONSTRAINT FK_Order_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID)
);
GO

-- 8. Tạo bảng OrderDetail (Chi tiết hóa đơn)
CREATE TABLE OrderDetail (
    DetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    FoodID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    UnitPrice DECIMAL(18,0) NOT NULL, -- Lưu giá tiền tại thời điểm gọi món
    Status INT NOT NULL DEFAULT 0,   -- (Mới) Trạng thái chế biến: 0: Chờ chế biến, 1: Đang chế biến, 2: Hoàn thành
    Notes NVARCHAR(255) NULL,         -- (Mới thêm) Ghi chú món (VD: ít đá, nhiều cay)
    CONSTRAINT FK_Detail_Order FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_Detail_Food FOREIGN KEY (FoodID) REFERENCES FoodItem(FoodID)
);
GO
```
