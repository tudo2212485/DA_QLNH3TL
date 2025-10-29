USE [master];
GO
IF DB_ID('QLNHDB') IS NULL
    CREATE DATABASE QLNHDB;
GO
USE QLNHDB;
GO

-- MenuItems
CREATE TABLE MenuItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(18,2) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    ImageUrl NVARCHAR(200)
);

-- Orders
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Date DATE NOT NULL,
    Time NVARCHAR(10) NOT NULL,
    Guests INT NOT NULL,
    Note NVARCHAR(500),
    TotalPrice DECIMAL(18,2),
    Status NVARCHAR(50)
);

-- OrderItems
CREATE TABLE OrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    MenuItemId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id)
);

-- ContactMessages
CREATE TABLE ContactMessages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Message NVARCHAR(1000) NOT NULL,
    Date DATETIME NOT NULL
);

-- Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    Role NVARCHAR(20)
);

-- Ratings
CREATE TABLE Ratings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MenuItemId INT NOT NULL,
    Score INT NOT NULL,
    Comment NVARCHAR(200),
    Date DATETIME NOT NULL,
    FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id)
);

-- Seed data cho MenuItems
INSERT INTO MenuItems (Name, Description, Price, Category, ImageUrl) VALUES
(N'Gỏi cuốn tôm thịt', N'Món khai vị truyền thống', 35000, N'Món khai vị', N'/images/goicuon.jpg'),
(N'Bò lúc lắc', N'Món chính hấp dẫn', 95000, N'Món chính', N'/images/boluclac.jpg'),
(N'Sườn nướng mật ong', N'Món nướng đặc biệt', 120000, N'Món nướng', N'/images/suonnuong.jpg'),
(N'Lẩu thái hải sản', N'Các món lẩu thơm ngon', 250000, N'Các món lẩu', N'/images/lautai.jpg'),
(N'Chè khúc bạch', N'Tráng miệng mát lạnh', 30000, N'Tráng miệng', N'/images/chekhucbach.jpg'),
(N'Trà đào cam sả', N'Đồ uống giải khát', 40000, N'Đồ uống', N'/images/tradao.jpg');
