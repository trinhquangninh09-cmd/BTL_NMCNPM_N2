-- Migration đăng ký tài khoản khách hàng cho TourManager v0.4
-- Chạy sau 03_upgrade_uc01_uc30.sql trên database TourManagerDB. Không xóa dữ liệu cũ.
USE TourManagerDB;
GO
IF COL_LENGTH('dbo.TaiKhoan','MaKhachHang') IS NULL
    ALTER TABLE dbo.TaiKhoan ADD MaKhachHang INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_TaiKhoan_KhachHang')
    ALTER TABLE dbo.TaiKhoan ADD CONSTRAINT FK_TaiKhoan_KhachHang FOREIGN KEY(MaKhachHang) REFERENCES dbo.KhachHang(MaKhachHang);
GO
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name=N'CK_TaiKhoan_VaiTro')
    ALTER TABLE dbo.TaiKhoan DROP CONSTRAINT CK_TaiKhoan_VaiTro;
GO
ALTER TABLE dbo.TaiKhoan ADD CONSTRAINT CK_TaiKhoan_VaiTro CHECK (VaiTro IN (N'Admin',N'BanHang',N'DieuHanh',N'HDV',N'KhachHang'));
GO
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name=N'IX_TaiKhoan_MaKhachHang' AND object_id=OBJECT_ID(N'dbo.TaiKhoan'))
    CREATE INDEX IX_TaiKhoan_MaKhachHang ON dbo.TaiKhoan(MaKhachHang);
GO
