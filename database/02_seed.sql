-- Dữ liệu mẫu giai đoạn 1
USE TourManagerDB;
GO
DELETE FROM NhatKy; DELETE FROM TaiKhoan; DELETE FROM Tour;
GO

-- Tài khoản mẫu: mật khẩu băm SHA-256(salt + mật khẩu) giống PasswordHasher trong C# (mật khẩu chỉ gồm ký tự ASCII).
DECLARE @s VARBINARY(16);

SET @s = CRYPT_GEN_RANDOM(16);
INSERT INTO TaiKhoan (TenDangNhap, HoTen, MatKhauHash, Salt, VaiTro)
VALUES (N'admin', N'Quản trị viên', HASHBYTES('SHA2_256', @s + CAST('Admin@123' AS VARBINARY(100))), @s, N'Admin');

SET @s = CRYPT_GEN_RANDOM(16);
INSERT INTO TaiKhoan (TenDangNhap, HoTen, MatKhauHash, Salt, VaiTro)
VALUES (N'banhang01', N'Nguyễn Văn An', HASHBYTES('SHA2_256', @s + CAST('BanHang@123' AS VARBINARY(100))), @s, N'BanHang');

SET @s = CRYPT_GEN_RANDOM(16);
INSERT INTO TaiKhoan (TenDangNhap, HoTen, MatKhauHash, Salt, VaiTro)
VALUES (N'dieuhanh01', N'Trần Thị Bình', HASHBYTES('SHA2_256', @s + CAST('DieuHanh@123' AS VARBINARY(100))), @s, N'DieuHanh');

SET @s = CRYPT_GEN_RANDOM(16);
INSERT INTO TaiKhoan (TenDangNhap, HoTen, MatKhauHash, Salt, VaiTro)
VALUES (N'hdv01', N'Lê Minh Quân', HASHBYTES('SHA2_256', @s + CAST('Hdv@12345' AS VARBINARY(100))), @s, N'HDV');
GO

INSERT INTO Tour (TenTour, DiemDen, SoNgay, SoDem, MoTa, PhuongTienChinh) VALUES
(N'Hà Nội – Hạ Long', N'Hạ Long', 3, 2, N'Du thuyền vịnh Hạ Long, tham quan hang Sửng Sốt.', N'Xe du lịch'),
(N'Đà Nẵng – Hội An', N'Đà Nẵng, Hội An', 4, 3, N'Phố cổ Hội An, Bà Nà Hills, bán đảo Sơn Trà.', N'Máy bay + xe du lịch'),
(N'Phú Quốc', N'Phú Quốc', 4, 3, N'Biển Bãi Sao, cáp treo Hòn Thơm, chợ đêm.', N'Máy bay'),
(N'Sa Pa', N'Sa Pa', 3, 2, N'Bản Cát Cát, Fansipan, chợ phiên.', N'Xe giường nằm'),
(N'Nha Trang', N'Nha Trang', 4, 3, N'Vinpearl, tắm bùn, lặn ngắm san hô.', N'Máy bay');
GO
