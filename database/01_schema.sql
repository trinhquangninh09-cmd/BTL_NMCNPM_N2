-- TourManagerDB - lược đồ giai đoạn 1 (tài khoản, tour, nhật ký)
-- Chạy bằng SQL Server Management Studio (SSMS), SQL Server 2019 trở lên.
IF DB_ID(N'TourManagerDB') IS NULL
    CREATE DATABASE TourManagerDB;
GO
USE TourManagerDB;
GO

IF OBJECT_ID(N'NhatKy') IS NOT NULL DROP TABLE NhatKy;
IF OBJECT_ID(N'TaiKhoan') IS NOT NULL DROP TABLE TaiKhoan;
IF OBJECT_ID(N'Tour') IS NOT NULL DROP TABLE Tour;
IF OBJECT_ID(N'HuongDanVien') IS NOT NULL DROP TABLE HuongDanVien;
GO

CREATE TABLE HuongDanVien (
    MaHDV       INT IDENTITY(1,1) CONSTRAINT PK_HuongDanVien PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    SoTheHDV    NVARCHAR(30)  NOT NULL CONSTRAINT UQ_HuongDanVien_SoTheHDV UNIQUE,
    SoDienThoai NVARCHAR(15)  NULL,
    NgonNgu     NVARCHAR(100) NULL,
    TrangThai   BIT NOT NULL CONSTRAINT DF_HuongDanVien_TrangThai DEFAULT 1
);

CREATE TABLE TaiKhoan (
    MaTaiKhoan  INT IDENTITY(1,1) CONSTRAINT PK_TaiKhoan PRIMARY KEY,
    TenDangNhap NVARCHAR(50)  NOT NULL CONSTRAINT UQ_TaiKhoan_TenDangNhap UNIQUE,
    HoTen       NVARCHAR(100) NOT NULL,
    MatKhauHash VARBINARY(32) NOT NULL,           -- SHA-256(salt + mật khẩu UTF-8)
    Salt        VARBINARY(16) NOT NULL,
    VaiTro      NVARCHAR(20)  NOT NULL
        CONSTRAINT CK_TaiKhoan_VaiTro CHECK (VaiTro IN (N'Admin', N'BanHang', N'DieuHanh', N'HDV')),
    TrangThai   BIT NOT NULL CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT 1,   -- 1 = hoạt động, 0 = bị Admin khóa
    SoLanSai    INT NOT NULL CONSTRAINT DF_TaiKhoan_SoLanSai DEFAULT 0,
    KhoaDen     DATETIME NULL,                    -- khóa tạm thời do nhập sai nhiều lần
    MaHDV       INT NULL CONSTRAINT FK_TaiKhoan_HuongDanVien REFERENCES HuongDanVien(MaHDV),
    NgayTao     DATETIME NOT NULL CONSTRAINT DF_TaiKhoan_NgayTao DEFAULT GETDATE()
);

CREATE TABLE Tour (
    MaTour          INT IDENTITY(1,1) CONSTRAINT PK_Tour PRIMARY KEY,
    TenTour         NVARCHAR(150) NOT NULL,
    DiemDen         NVARCHAR(100) NOT NULL,
    SoNgay          TINYINT NOT NULL CONSTRAINT CK_Tour_SoNgay CHECK (SoNgay > 0),
    SoDem           TINYINT NOT NULL CONSTRAINT CK_Tour_SoDem CHECK (SoDem >= 0),
    MoTa            NVARCHAR(MAX) NULL,
    PhuongTienChinh NVARCHAR(50)  NULL,
    TrangThai       BIT NOT NULL CONSTRAINT DF_Tour_TrangThai DEFAULT 1    -- 1 = đang bán, 0 = ẩn
);

CREATE TABLE NhatKy (
    MaNhatKy    BIGINT IDENTITY(1,1) CONSTRAINT PK_NhatKy PRIMARY KEY,
    MaTaiKhoan  INT NOT NULL CONSTRAINT FK_NhatKy_TaiKhoan REFERENCES TaiKhoan(MaTaiKhoan),
    ThoiDiem    DATETIME NOT NULL CONSTRAINT DF_NhatKy_ThoiDiem DEFAULT GETDATE(),
    HanhDong    NVARCHAR(20) NOT NULL,
    DoiTuong    NVARCHAR(50) NULL,
    NoiDung     NVARCHAR(MAX) NULL
);

CREATE INDEX IX_Tour_DiemDen ON Tour(DiemDen);
CREATE INDEX IX_NhatKy_ThoiDiem ON NhatKy(ThoiDiem);
GO
