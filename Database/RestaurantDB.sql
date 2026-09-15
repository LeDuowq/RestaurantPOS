-- 1. Tạo Database
CREATE DATABASE RestaurantPOS;
GO

USE RestaurantPOS;
GO

-- 2. Tạo bảng Account (Tài khoản nhân viên)
CREATE TABLE Account (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role INT NOT NULL DEFAULT 3 -- 1: Admin, 2: Thu ngân (Cashier), 3: Phục vụ (Waitstaff)
);
GO

-- 3. Tạo bảng DiningTable (Bàn ăn)
CREATE TABLE DiningTable (
    TableID INT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(50) NOT NULL, -- Vd: Bàn 1, Bàn 2, VIP 1
    Status INT NOT NULL DEFAULT 0    -- 0: Trống, 1: Đang có khách
);
GO

-- 4. Tạo bảng Category (Danh mục món ăn)
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- 5. Tạo bảng FoodItem (Món ăn)
CREATE TABLE FoodItem (
    FoodID INT IDENTITY(1,1) PRIMARY KEY,
    FoodName NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,0) NOT NULL,    -- Dùng DECIMAL để lưu tiền Việt (VND)
    CategoryID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 50, -- Số lượng suất ăn khả dụng
    Img NVARCHAR(MAX) NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Food_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);
GO

-- 6. Tạo bảng Orders (Hóa đơn)
-- Lưu ý: Đặt tên là Orders vì chữ Order là từ khóa của SQL
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    TableID INT NOT NULL,
    AccountID INT NOT NULL,          -- Nhân viên nào lập hóa đơn này
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    CheckoutDate DATETIME NULL,
    TotalPrice DECIMAL(18,0) DEFAULT 0,
    Discount DECIMAL(18,0) NOT NULL DEFAULT 0,
    Status INT NOT NULL DEFAULT 0,   -- 0: Chưa thanh toán, 1: Đã thanh toán
    CONSTRAINT FK_Order_Table FOREIGN KEY (TableID) REFERENCES DiningTable(TableID),
    CONSTRAINT FK_Order_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID)
);
GO

-- 7. Tạo bảng OrderDetail (Chi tiết hóa đơn)
CREATE TABLE OrderDetail (
    DetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    FoodID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    UnitPrice DECIMAL(18,0) NOT NULL, -- Lưu giá tiền tại thời điểm gọi món (phòng trường hợp sau này món ăn tăng giá)
    Status INT NOT NULL DEFAULT 0,   -- 0: Chờ chế biến, 1: Đang chế biến, 2: Hoàn thành
    Notes NVARCHAR(255) NULL,
    CONSTRAINT FK_Detail_Order FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_Detail_Food FOREIGN KEY (FoodID) REFERENCES FoodItem(FoodID)
);
GO

-- 1. Tạo bảng Role
CREATE TABLE [dbo].[Role] (
    [RoleID] INT PRIMARY KEY,
    [RoleName] NVARCHAR(50) NOT NULL
);

-- 2. Thêm dữ liệu các chức danh vào bảng Role
INSERT INTO [dbo].[Role] ([RoleID], [RoleName]) 
VALUES 
(1, N'Quản lí'), 
(2, N'Nhân viên Thu ngân'), 
(3, N'Nhân viên Phục vụ'),
(4, N'Đầu bếp');

-- 3. Nối Khóa ngoại từ bảng Account sang bảng Role
ALTER TABLE [dbo].[Account] 
ADD CONSTRAINT FK_Account_Role FOREIGN KEY ([Role]) REFERENCES [dbo].[Role]([RoleID]);

-- Thêm cột Email (Cho phép NULL tạm thời để các tài khoản cũ không bị lỗi)
ALTER TABLE [dbo].[Account]
ADD [Email] NVARCHAR(100) NULL;

-- Thêm cột Status để quản lý nhân viên còn làm hay đã nghỉ (1 là đang làm, 0 là đã nghỉ)
ALTER TABLE [dbo].[Account]
ADD [Status] INT NOT NULL DEFAULT 1;

-- Lệnh cập nhật cho Database đã tồn tại
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[FoodItem]') AND name = 'Quantity')
BEGIN
    ALTER TABLE [dbo].[FoodItem] ADD [Quantity] INT NOT NULL DEFAULT 50;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetail]') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[OrderDetail] ADD [Status] INT NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Role] WHERE [RoleID] = 4)
BEGIN
    INSERT INTO [dbo].[Role] ([RoleID], [RoleName]) VALUES (4, N'Đầu bếp');
END
GO