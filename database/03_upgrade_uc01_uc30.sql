-- TourManagerDB upgrade: UC01-UC30 foundation. Run AFTER 01_schema.sql and 02_seed.sql.
-- Non-destructive migration: existing TaiKhoan, Tour, HuongDanVien, NhatKy are preserved.
USE TourManagerDB;
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
-- UC13-15: transaction + UPDLOCK/HOLDLOCK prevents concurrent overbooking.
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
