-- Chạy sau 03 và 04. Không xóa dữ liệu hiện có.
USE TourManagerDB;
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
