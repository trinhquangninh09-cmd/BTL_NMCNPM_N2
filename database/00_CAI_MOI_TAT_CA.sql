IF DB_ID(N'TourManagerDB_New') IS NULL
    CREATE DATABASE TourManagerDB_New;
GO
USE TourManagerDB_New;
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
    MatKhauHash VARBINARY(32) NOT NULL,
    Salt        VARBINARY(16) NOT NULL,
    VaiTro      NVARCHAR(20)  NOT NULL
        CONSTRAINT CK_TaiKhoan_VaiTro CHECK (VaiTro IN (N'Admin', N'BanHang', N'DieuHanh', N'HDV')),
    TrangThai   BIT NOT NULL CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT 1,
    SoLanSai    INT NOT NULL CONSTRAINT DF_TaiKhoan_SoLanSai DEFAULT 0,
    KhoaDen     DATETIME NULL,
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
    TrangThai       BIT NOT NULL CONSTRAINT DF_Tour_TrangThai DEFAULT 1
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


USE TourManagerDB_New;
GO


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


USE TourManagerDB_New;
GO
IF OBJECT_ID('KhachHang','U') IS NULL CREATE TABLE KhachHang(
 MaKhachHang INT IDENTITY PRIMARY KEY, HoTen NVARCHAR(120) NOT NULL, CCCD NVARCHAR(20) NOT NULL UNIQUE,
 NgaySinh DATE NULL, GioiTinh NVARCHAR(10) NULL, SoDienThoai NVARCHAR(20) NULL,
 Email NVARCHAR(120) NULL, DiaChi NVARCHAR(250) NULL, NgayTao DATETIME2 NOT NULL DEFAULT SYSDATETIME(), GhiChu NVARCHAR(500) NULL);
IF OBJECT_ID('LichTrinhTour','U') IS NULL CREATE TABLE LichTrinhTour(
 MaLichTrinh INT IDENTITY PRIMARY KEY, MaTour INT NOT NULL REFERENCES Tour(MaTour), NgayThu TINYINT NOT NULL CHECK(NgayThu>0),
 TieuDe NVARCHAR(150) NOT NULL, DiemThamQuan NVARCHAR(500) NULL, BuaAn NVARCHAR(200) NULL, LuuTru NVARCHAR(200) NULL,
 GhiChu NVARCHAR(500) NULL, UNIQUE(MaTour,NgayThu));
IF OBJECT_ID('GiaTour','U') IS NULL CREATE TABLE GiaTour(
 MaGia INT IDENTITY PRIMARY KEY, MaTour INT NOT NULL REFERENCES Tour(MaTour), GiaNguoiLon DECIMAL(18,2) NOT NULL CHECK(GiaNguoiLon>=0),
 GiaTreEm DECIMAL(18,2) NOT NULL CHECK(GiaTreEm>=0), PhuThuPhongDon DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK(PhuThuPhongDon>=0),
 PhuThuCaoDiem DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK(PhuThuCaoDiem>=0), TuNgay DATE NOT NULL, DenNgay DATE NULL,
 CHECK(DenNgay IS NULL OR DenNgay>=TuNgay));
IF OBJECT_ID('DotKhoiHanh','U') IS NULL CREATE TABLE DotKhoiHanh(
 MaDot INT IDENTITY PRIMARY KEY, MaTour INT NOT NULL REFERENCES Tour(MaTour), NgayDi DATETIME2 NOT NULL, NgayVe DATETIME2 NOT NULL,
 SoChoToiDa INT NOT NULL CHECK(SoChoToiDa>0), TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Mở bán'
 CHECK(TrangThai IN(N'Mở bán',N'Đóng',N'Hủy',N'Đang chạy',N'Hoàn thành')), GhiChu NVARCHAR(500) NULL, CHECK(NgayVe>NgayDi));
IF OBJECT_ID('PhuongTien','U') IS NULL CREATE TABLE PhuongTien(
 MaXe INT IDENTITY PRIMARY KEY, BienSo NVARCHAR(20) NOT NULL UNIQUE, TenXe NVARCHAR(100) NOT NULL,
 SoCho INT NOT NULL CHECK(SoCho>0), LoaiXe NVARCHAR(50) NULL, TrangThai BIT NOT NULL DEFAULT 1, GhiChu NVARCHAR(500) NULL);
IF OBJECT_ID('KhuyenMai','U') IS NULL CREATE TABLE KhuyenMai(
 MaKhuyenMai INT IDENTITY PRIMARY KEY, MaCode NVARCHAR(30) NOT NULL UNIQUE, TenKhuyenMai NVARCHAR(120) NOT NULL,
 TyLeGiam DECIMAL(5,2) NOT NULL CHECK(TyLeGiam BETWEEN 0 AND 100), TuNgay DATE NOT NULL, DenNgay DATE NOT NULL,
 TrangThai BIT NOT NULL DEFAULT 1, CHECK(DenNgay>=TuNgay));
IF OBJECT_ID('CauHinhNghiepVu','U') IS NULL CREATE TABLE CauHinhNghiepVu(
 MaCauHinh INT IDENTITY PRIMARY KEY, Khoa NVARCHAR(50) NOT NULL UNIQUE, GiaTri DECIMAL(18,2) NOT NULL, MoTa NVARCHAR(250) NULL);
IF NOT EXISTS(SELECT 1 FROM CauHinhNghiepVu WHERE Khoa=N'GiamGiaNhom') INSERT CauHinhNghiepVu(Khoa,GiaTri,MoTa) VALUES(N'GiamGiaNhom',5,N'% giảm cho đoàn từ 10 khách');
IF OBJECT_ID('Booking','U') IS NULL CREATE TABLE Booking(
 MaBooking BIGINT IDENTITY PRIMARY KEY, MaCode NVARCHAR(30) NOT NULL UNIQUE, MaDot INT NOT NULL REFERENCES DotKhoiHanh(MaDot),
 MaKhachHang INT NOT NULL REFERENCES KhachHang(MaKhachHang), MaTaiKhoan INT NOT NULL REFERENCES TaiKhoan(MaTaiKhoan),
 SoNguoiLon INT NOT NULL CHECK(SoNguoiLon>=0), SoTreEm INT NOT NULL CHECK(SoTreEm>=0), PhuThu DECIMAL(18,2) NOT NULL DEFAULT 0,
 GiamGia DECIMAL(18,2) NOT NULL DEFAULT 0, TongTien DECIMAL(18,2) NOT NULL CHECK(TongTien>=0),
 TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Giữ chỗ' CHECK(TrangThai IN(N'Giữ chỗ',N'Đã cọc',N'Đã thanh toán',N'Đã hủy')),
 NgayDat DATETIME2 NOT NULL DEFAULT SYSDATETIME(), GhiChu NVARCHAR(500) NULL);
IF OBJECT_ID('ThanhToan','U') IS NULL CREATE TABLE ThanhToan(
 MaThanhToan BIGINT IDENTITY PRIMARY KEY, MaBooking BIGINT NOT NULL REFERENCES Booking(MaBooking), SoTien DECIMAL(18,2) NOT NULL CHECK(SoTien>0),
 NgayThanhToan DATETIME2 NOT NULL DEFAULT SYSDATETIME(), PhuongThuc NVARCHAR(30) NOT NULL, LoaiThanhToan NVARCHAR(20) NOT NULL,
 MaTaiKhoan INT NOT NULL REFERENCES TaiKhoan(MaTaiKhoan), GhiChu NVARCHAR(300) NULL);
IF OBJECT_ID('PhanCongHDV','U') IS NULL CREATE TABLE PhanCongHDV(
 MaPhanCong INT IDENTITY PRIMARY KEY, MaDot INT NOT NULL REFERENCES DotKhoiHanh(MaDot), MaHDV INT NOT NULL REFERENCES HuongDanVien(MaHDV),
 NgayPhanCong DATETIME2 NOT NULL DEFAULT SYSDATETIME(), GhiChu NVARCHAR(300) NULL);
IF OBJECT_ID('PhanCongXe','U') IS NULL CREATE TABLE PhanCongXe(
 MaPhanCong INT IDENTITY PRIMARY KEY, MaDot INT NOT NULL REFERENCES DotKhoiHanh(MaDot), MaXe INT NOT NULL REFERENCES PhuongTien(MaXe),
 NgayPhanCong DATETIME2 NOT NULL DEFAULT SYSDATETIME(), GhiChu NVARCHAR(300) NULL);
IF OBJECT_ID('TrangThaiDoan','U') IS NULL CREATE TABLE TrangThaiDoan(
 MaCapNhat BIGINT IDENTITY PRIMARY KEY, MaDot INT NOT NULL REFERENCES DotKhoiHanh(MaDot), TrangThai NVARCHAR(30) NOT NULL,
 GhiChu NVARCHAR(1000) NULL, MaTaiKhoan INT NOT NULL REFERENCES TaiKhoan(MaTaiKhoan), ThoiDiem DATETIME2 NOT NULL DEFAULT SYSDATETIME());
IF OBJECT_ID('ChinhSachHuy','U') IS NULL CREATE TABLE ChinhSachHuy(
 SoNgayToiThieu INT NOT NULL PRIMARY KEY, TyLePhi DECIMAL(5,2) NOT NULL CHECK(TyLePhi BETWEEN 0 AND 100));
IF NOT EXISTS(SELECT 1 FROM ChinhSachHuy) INSERT ChinhSachHuy VALUES(30,10),(15,30),(7,50),(0,100);
GO
CREATE OR ALTER VIEW vw_DotConCho AS
SELECT d.MaDot,d.MaTour,d.NgayDi,d.NgayVe,d.SoChoToiDa,d.TrangThai,
 ISNULL(SUM(CASE WHEN b.TrangThai<>N'Đã hủy' THEN b.SoNguoiLon+b.SoTreEm ELSE 0 END),0) SoKhachDaDat,
 d.SoChoToiDa-ISNULL(SUM(CASE WHEN b.TrangThai<>N'Đã hủy' THEN b.SoNguoiLon+b.SoTreEm ELSE 0 END),0) SoChoCon
FROM DotKhoiHanh d LEFT JOIN Booking b ON b.MaDot=d.MaDot
GROUP BY d.MaDot,d.MaTour,d.NgayDi,d.NgayVe,d.SoChoToiDa,d.TrangThai;
GO
CREATE OR ALTER PROCEDURE sp_TaoBooking
 @MaDot INT,@MaKhachHang INT,@MaTaiKhoan INT,@SoNguoiLon INT,@SoTreEm INT,
 @PhuThu DECIMAL(18,2)=0,@MaCode NVARCHAR(30)=NULL,@GhiChu NVARCHAR(500)=NULL
AS BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @SoNguoiLon<0 OR @SoTreEm<0 OR @SoNguoiLon+@SoTreEm=0 THROW 50001,N'Số lượng khách không hợp lệ.',1;
 BEGIN TRAN;
 DECLARE @TrangThai NVARCHAR(20),@MaTour INT,@NgayDi DATETIME2,@Cho INT,@DaDat INT;
 SELECT @TrangThai=TrangThai,@MaTour=MaTour,@NgayDi=NgayDi,@Cho=SoChoToiDa FROM DotKhoiHanh WITH(UPDLOCK,HOLDLOCK) WHERE MaDot=@MaDot;
 IF @MaTour IS NULL THROW 50002,N'Đợt khởi hành không tồn tại.',1;
 IF @TrangThai<>N'Mở bán' THROW 50003,N'Đợt khởi hành không ở trạng thái Mở bán.',1;
 SELECT @DaDat=ISNULL(SUM(SoNguoiLon+SoTreEm),0) FROM Booking WITH(UPDLOCK,HOLDLOCK) WHERE MaDot=@MaDot AND TrangThai<>N'Đã hủy';
 IF @Cho-@DaDat<@SoNguoiLon+@SoTreEm THROW 50004,N'Không đủ chỗ trống.',1;
 DECLARE @GiaNL DECIMAL(18,2),@GiaTE DECIMAL(18,2),@PhuCao DECIMAL(18,2),@Giam DECIMAL(18,2)=0,
 @Tong DECIMAL(18,2),@TongKhach INT=@SoNguoiLon+@SoTreEm;
 SELECT TOP 1 @GiaNL=GiaNguoiLon,@GiaTE=GiaTreEm,@PhuCao=PhuThuCaoDiem FROM GiaTour
 WHERE MaTour=@MaTour AND TuNgay<=CAST(@NgayDi AS DATE) AND (DenNgay IS NULL OR DenNgay>=CAST(@NgayDi AS DATE)) ORDER BY TuNgay DESC;
 IF @GiaNL IS NULL THROW 50005,N'Chưa cấu hình giá có hiệu lực cho ngày khởi hành.',1;
 SET @Tong=@GiaNL*@SoNguoiLon+@GiaTE*@SoTreEm+ISNULL(@PhuThu,0)+ISNULL(@PhuCao,0)*@TongKhach;
 IF @MaCode IS NOT NULL AND LTRIM(RTRIM(@MaCode))<>N'' BEGIN
  SELECT @Giam=@Tong*TyLeGiam/100 FROM KhuyenMai WHERE MaCode=@MaCode AND TrangThai=1 AND CAST(GETDATE() AS DATE) BETWEEN TuNgay AND DenNgay;
  IF @Giam IS NULL OR @Giam=0 THROW 50006,N'Mã khuyến mãi không hợp lệ hoặc hết hạn.',1;
 END ELSE IF @TongKhach>=10 SELECT @Giam=@Tong*GiaTri/100 FROM CauHinhNghiepVu WHERE Khoa=N'GiamGiaNhom';
 SET @Tong=@Tong-ISNULL(@Giam,0);
 INSERT Booking(MaCode,MaDot,MaKhachHang,MaTaiKhoan,SoNguoiLon,SoTreEm,PhuThu,GiamGia,TongTien,GhiChu)
 VALUES(CONCAT(N'BK',FORMAT(SYSDATETIME(),N'yyyyMMddHHmmss'),RIGHT(CONVERT(NVARCHAR(20),ABS(CHECKSUM(NEWID()))),4)),
 @MaDot,@MaKhachHang,@MaTaiKhoan,@SoNguoiLon,@SoTreEm,ISNULL(@PhuThu,0)+ISNULL(@PhuCao,0)*@TongKhach,ISNULL(@Giam,0),@Tong,@GhiChu);
 DECLARE @Id BIGINT=SCOPE_IDENTITY(); COMMIT; SELECT * FROM Booking WHERE MaBooking=@Id;
END;
GO
CREATE OR ALTER PROCEDURE sp_GhiThanhToan @MaBooking BIGINT,@SoTien DECIMAL(18,2),@PhuongThuc NVARCHAR(30),@LoaiThanhToan NVARCHAR(20),@MaTaiKhoan INT AS
BEGIN SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN;
 DECLARE @Tong DECIMAL(18,2),@DaThu DECIMAL(18,2),@TrangThai NVARCHAR(20);
 SELECT @Tong=TongTien,@TrangThai=TrangThai FROM Booking WITH(UPDLOCK,HOLDLOCK) WHERE MaBooking=@MaBooking;
 IF @Tong IS NULL THROW 50011,N'Booking không tồn tại.',1;
 IF @TrangThai=N'Đã hủy' THROW 50012,N'Booking đã hủy.',1;
 SELECT @DaThu=ISNULL(SUM(SoTien),0) FROM ThanhToan WHERE MaBooking=@MaBooking;
 IF @SoTien<=0 OR @DaThu+@SoTien>@Tong THROW 50013,N'Số tiền thanh toán không hợp lệ.',1;
 INSERT ThanhToan(MaBooking,SoTien,PhuongThuc,LoaiThanhToan,MaTaiKhoan) VALUES(@MaBooking,@SoTien,@PhuongThuc,@LoaiThanhToan,@MaTaiKhoan);
 SET @DaThu=@DaThu+@SoTien; UPDATE Booking SET TrangThai=CASE WHEN @DaThu>=@Tong THEN N'Đã thanh toán' ELSE N'Đã cọc' END WHERE MaBooking=@MaBooking; COMMIT;
END;
GO
CREATE OR ALTER PROCEDURE sp_HuyBooking @MaBooking BIGINT,@MaTaiKhoan INT AS
BEGIN SET NOCOUNT ON; SET XACT_ABORT ON; BEGIN TRAN;
 DECLARE @MaDot INT,@TrangThai NVARCHAR(20),@NgayDi DATETIME2,@DaThu DECIMAL(18,2),@NgayCon INT,@TyLe DECIMAL(5,2),@Phi DECIMAL(18,2);
 SELECT @MaDot=b.MaDot,@TrangThai=b.TrangThai,@NgayDi=d.NgayDi FROM Booking b WITH(UPDLOCK,HOLDLOCK) JOIN DotKhoiHanh d ON d.MaDot=b.MaDot WHERE b.MaBooking=@MaBooking;
 IF @MaDot IS NULL THROW 50021,N'Booking không tồn tại.',1;
 IF @TrangThai=N'Đã hủy' THROW 50022,N'Booking đã hủy.',1;
 IF @NgayDi<=SYSDATETIME() THROW 50023,N'Không thể hủy sau ngày khởi hành.',1;
 SELECT @DaThu=ISNULL(SUM(SoTien),0) FROM ThanhToan WHERE MaBooking=@MaBooking;
 SET @NgayCon=DATEDIFF(DAY,CAST(SYSDATETIME() AS DATE),CAST(@NgayDi AS DATE));
 SELECT TOP 1 @TyLe=TyLePhi FROM ChinhSachHuy WHERE SoNgayToiThieu<=@NgayCon ORDER BY SoNgayToiThieu DESC;
 SET @Phi=ISNULL(@DaThu,0)*ISNULL(@TyLe,100)/100;
 UPDATE Booking SET TrangThai=N'Đã hủy',GhiChu=CONCAT(ISNULL(GhiChu,N''),N' | Hủy; phí=',@Phi) WHERE MaBooking=@MaBooking;
 COMMIT; SELECT @Phi PhiHuy,(@DaThu-@Phi) SoTienHoan,@TyLe TyLePhi;
END;
GO
CREATE OR ALTER VIEW vw_BaoCaoDoanhThu AS
SELECT CAST(t.NgayThanhToan AS DATE) Ngay,b.MaDot,tr.MaTour,tr.TenTour,SUM(t.SoTien) DoanhThu,COUNT(DISTINCT b.MaBooking) SoBooking
FROM ThanhToan t JOIN Booking b ON b.MaBooking=t.MaBooking JOIN DotKhoiHanh d ON d.MaDot=b.MaDot JOIN Tour tr ON tr.MaTour=d.MaTour
GROUP BY CAST(t.NgayThanhToan AS DATE),b.MaDot,tr.MaTour,tr.TenTour;
GO
CREATE OR ALTER VIEW vw_TopTour AS
SELECT tr.MaTour,tr.TenTour,COUNT(DISTINCT b.MaBooking) SoBooking,
 SUM(CASE WHEN b.TrangThai<>N'Đã hủy' THEN b.SoNguoiLon+b.SoTreEm ELSE 0 END) SoKhach
FROM Tour tr LEFT JOIN DotKhoiHanh d ON d.MaTour=tr.MaTour LEFT JOIN Booking b ON b.MaDot=d.MaDot GROUP BY tr.MaTour,tr.TenTour;
GO
CREATE OR ALTER VIEW vw_ThongKeDot AS
SELECT d.MaDot,tr.TenTour,d.NgayDi,d.NgayVe,d.SoChoToiDa,
 ISNULL(SUM(CASE WHEN b.TrangThai<>N'Đã hủy' THEN b.SoNguoiLon+b.SoTreEm ELSE 0 END),0) SoKhach,
 CAST(100.0*ISNULL(SUM(CASE WHEN b.TrangThai<>N'Đã hủy' THEN b.SoNguoiLon+b.SoTreEm ELSE 0 END),0)/NULLIF(d.SoChoToiDa,0) AS DECIMAL(6,2)) TyLeLapDay
FROM DotKhoiHanh d JOIN Tour tr ON tr.MaTour=d.MaTour LEFT JOIN Booking b ON b.MaDot=d.MaDot
GROUP BY d.MaDot,tr.TenTour,d.NgayDi,d.NgayVe,d.SoChoToiDa;
GO
CREATE INDEX IX_DotKhoiHanh_Ngay ON DotKhoiHanh(NgayDi,TrangThai);
CREATE INDEX IX_Booking_Dot_TrangThai ON Booking(MaDot,TrangThai);
CREATE INDEX IX_ThanhToan_Booking ON ThanhToan(MaBooking,NgayThanhToan);
CREATE INDEX IX_KhachHang_HoTen ON KhachHang(HoTen);
GO


USE TourManagerDB_New;
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


USE TourManagerDB_New;
GO
IF OBJECT_ID(N'dbo.YeuCauThanhToan',N'U') IS NULL
BEGIN
 CREATE TABLE dbo.YeuCauThanhToan(
  MaYeuCau BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  MaBooking BIGINT NOT NULL REFERENCES dbo.Booking(MaBooking),
  MaKhachHang INT NOT NULL REFERENCES dbo.KhachHang(MaKhachHang),
  SoTien DECIMAL(18,2) NOT NULL CHECK(SoTien>0),
  PhuongThuc NVARCHAR(30) NOT NULL CHECK(PhuongThuc IN(N'Chuyển khoản',N'Tiền mặt')),
  MaThamChieu NVARCHAR(100) NULL,
  TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Chờ xác nhận' CHECK(TrangThai IN(N'Chờ xác nhận',N'Đã xác nhận',N'Từ chối')),
  NgayTao DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
  NgayXuLy DATETIME2 NULL,
  MaNhanVienXuLy INT NULL REFERENCES dbo.TaiKhoan(MaTaiKhoan)
 );
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_KhachGuiYeuCauThanhToan
 @MaBooking BIGINT,@MaKhachHang INT,@SoTien DECIMAL(18,2),@PhuongThuc NVARCHAR(30),@MaThamChieu NVARCHAR(100)=NULL
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 BEGIN TRAN;
 DECLARE @Tong DECIMAL(18,2),@DaThu DECIMAL(18,2),@DangCho DECIMAL(18,2),@TrangThai NVARCHAR(20);
 SELECT @Tong=TongTien,@TrangThai=TrangThai FROM dbo.Booking WITH(UPDLOCK,HOLDLOCK)
 WHERE MaBooking=@MaBooking AND MaKhachHang=@MaKhachHang;
 IF @Tong IS NULL OR @TrangThai=N'Đã hủy' THROW 51001,N'Đơn hàng không hợp lệ hoặc không thuộc tài khoản.',1;
 SELECT @DaThu=ISNULL(SUM(SoTien),0) FROM dbo.ThanhToan WHERE MaBooking=@MaBooking;
 SELECT @DangCho=ISNULL(SUM(SoTien),0) FROM dbo.YeuCauThanhToan WHERE MaBooking=@MaBooking AND TrangThai=N'Chờ xác nhận';
 IF @SoTien<=0 OR @SoTien>@Tong-@DaThu-@DangCho THROW 51002,N'Số tiền vượt quá phần chưa thanh toán hoặc đã gửi yêu cầu.',1;
 IF @PhuongThuc NOT IN(N'Chuyển khoản',N'Tiền mặt') THROW 51003,N'Phương thức không hợp lệ.',1;
 INSERT dbo.YeuCauThanhToan(MaBooking,MaKhachHang,SoTien,PhuongThuc,MaThamChieu)
 VALUES(@MaBooking,@MaKhachHang,@SoTien,@PhuongThuc,NULLIF(LTRIM(RTRIM(@MaThamChieu)),N''));
 COMMIT;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_XuLyYeuCauThanhToan
 @MaYeuCau BIGINT,@MaNhanVien INT,@ChapNhan BIT
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.TaiKhoan WHERE MaTaiKhoan=@MaNhanVien AND VaiTro IN(N'Admin',N'BanHang') AND TrangThai=1)
  THROW 51011,N'Không có quyền xác nhận thanh toán.',1;
 BEGIN TRAN;
 DECLARE @MaBooking BIGINT,@SoTien DECIMAL(18,2),@PhuongThuc NVARCHAR(30),@TrangThai NVARCHAR(20);
 SELECT @MaBooking=MaBooking,@SoTien=SoTien,@PhuongThuc=PhuongThuc,@TrangThai=TrangThai
 FROM dbo.YeuCauThanhToan WITH(UPDLOCK,HOLDLOCK) WHERE MaYeuCau=@MaYeuCau;
 IF @TrangThai IS NULL OR @TrangThai<>N'Chờ xác nhận' THROW 51012,N'Yêu cầu đã được xử lý hoặc không tồn tại.',1;
 IF @ChapNhan=1
 BEGIN
  EXEC dbo.sp_GhiThanhToan @MaBooking=@MaBooking,@SoTien=@SoTien,@PhuongThuc=@PhuongThuc,
   @LoaiThanhToan=N'Khách tự gửi',@MaTaiKhoan=@MaNhanVien;
 END;
 UPDATE dbo.YeuCauThanhToan SET TrangThai=CASE WHEN @ChapNhan=1 THEN N'Đã xác nhận' ELSE N'Từ chối' END,
  NgayXuLy=SYSDATETIME(),MaNhanVienXuLy=@MaNhanVien WHERE MaYeuCau=@MaYeuCau;
 COMMIT;
END;
GO

USE TourManagerDB_New;
GO
INSERT dbo.GiaTour(MaTour,GiaNguoiLon,GiaTreEm,TuNgay,DenNgay)
SELECT t.MaTour, CAST(1850000 + (t.MaTour-1)*450000 AS DECIMAL(18,2)),
       CAST(950000 + (t.MaTour-1)*250000 AS DECIMAL(18,2)),
       CAST(GETDATE() AS DATE), NULL
FROM dbo.Tour t
WHERE NOT EXISTS(SELECT 1 FROM dbo.GiaTour g WHERE g.MaTour=t.MaTour);
GO
INSERT dbo.DotKhoiHanh(MaTour,NgayDi,NgayVe,SoChoToiDa,TrangThai)
SELECT t.MaTour, DATEADD(DAY, 15 + t.MaTour*7, CAST(GETDATE() AS DATETIME2)),
       DATEADD(DAY, 15 + t.MaTour*7 + t.SoNgay, CAST(GETDATE() AS DATETIME2)),
       30, N'Mở bán'
FROM dbo.Tour t
WHERE NOT EXISTS(SELECT 1 FROM dbo.DotKhoiHanh d WHERE d.MaTour=t.MaTour);
GO
