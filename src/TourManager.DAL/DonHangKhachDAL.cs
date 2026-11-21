using System;
using System.Data;
using System.Data.SqlClient;

namespace TourManager.DAL
{
    public class DonHangKhachDAL
    {
        public DataTable LayDon(int maKhach)
        {
            return DbHelper.ExecuteDataTable(@"SELECT b.MaBooking,b.MaCode,t.TenTour,d.NgayDi,b.SoNguoiLon,b.SoTreEm,b.TongTien,
 ISNULL((SELECT SUM(p.SoTien) FROM ThanhToan p WHERE p.MaBooking=b.MaBooking),0) AS DaThanhToan,
 ISNULL((SELECT SUM(y.SoTien) FROM YeuCauThanhToan y WHERE y.MaBooking=b.MaBooking AND y.TrangThai=N'Chờ xác nhận'),0) AS DangChoXacNhan,
 b.TrangThai,b.NgayDat FROM Booking b JOIN DotKhoiHanh d ON d.MaDot=b.MaDot
 JOIN Tour t ON t.MaTour=d.MaTour WHERE b.MaKhachHang=@MaKhach ORDER BY b.NgayDat DESC",
                new SqlParameter("@MaKhach", maKhach));
        }
        public DataTable LayYeuCau(int maKhach)
        {
            return DbHelper.ExecuteDataTable(@"SELECT y.MaYeuCau,b.MaCode,y.SoTien,y.PhuongThuc,y.MaThamChieu,y.TrangThai,y.NgayTao,y.NgayXuLy
 FROM YeuCauThanhToan y JOIN Booking b ON b.MaBooking=y.MaBooking WHERE y.MaKhachHang=@MaKhach ORDER BY y.NgayTao DESC",
                new SqlParameter("@MaKhach", maKhach));
        }
        public void GuiThanhToan(long maBooking, int maKhach, decimal soTien, string phuongThuc, string thamChieu)
        {
            using (var cn = DbHelper.TaoKetNoi()) using (var cmd = new SqlCommand("sp_KhachGuiYeuCauThanhToan", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaBooking", maBooking); cmd.Parameters.AddWithValue("@MaKhachHang", maKhach);
                cmd.Parameters.AddWithValue("@SoTien", soTien); cmd.Parameters.AddWithValue("@PhuongThuc", phuongThuc);
                cmd.Parameters.AddWithValue("@MaThamChieu", (object)thamChieu ?? DBNull.Value);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }
        public DataTable LayChoDuyet()
        {
            return DbHelper.ExecuteDataTable(@"SELECT y.MaYeuCau,b.MaCode,k.HoTen,y.SoTien,y.PhuongThuc,y.MaThamChieu,y.NgayTao
 FROM YeuCauThanhToan y JOIN Booking b ON b.MaBooking=y.MaBooking JOIN KhachHang k ON k.MaKhachHang=y.MaKhachHang
 WHERE y.TrangThai=N'Chờ xác nhận' ORDER BY y.NgayTao");
        }
        public void Duyet(long maYeuCau, int maNhanVien, bool chapNhan)
        {
            using (var cn = DbHelper.TaoKetNoi()) using (var cmd = new SqlCommand("sp_XuLyYeuCauThanhToan", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure; cmd.Parameters.AddWithValue("@MaYeuCau", maYeuCau);
                cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien); cmd.Parameters.AddWithValue("@ChapNhan", chapNhan);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }
    }
}
