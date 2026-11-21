using System;
using System.Data;
using TourManager.DAL;

namespace TourManager.BLL
{
    public class DonHangKhachBLL
    {
        private readonly DonHangKhachDAL dal = new DonHangKhachDAL();
        private int MaKhach()
        {
            PhanQuyen.YeuCau(PhanQuyen.KhachHang);
            if (!Session.TaiKhoan.MaKhachHang.HasValue) throw new BusinessException("Tài khoản chưa liên kết hồ sơ khách hàng.");
            return Session.TaiKhoan.MaKhachHang.Value;
        }
        public DataTable DonCuaToi() { return dal.LayDon(MaKhach()); }
        public DataTable LichSuYeuCau() { return dal.LayYeuCau(MaKhach()); }
        public void GuiYeuCau(long maBooking, decimal soTien, string phuongThuc, string thamChieu)
        {
            int maKhach = MaKhach();
            if (soTien <= 0) throw new BusinessException("Số tiền phải lớn hơn 0.");
            if (phuongThuc != "Chuyển khoản" && phuongThuc != "Tiền mặt") throw new BusinessException("Phương thức không hợp lệ.");
            dal.GuiThanhToan(maBooking, maKhach, soTien, phuongThuc, thamChieu);
        }
        public DataTable ChoDuyet() { PhanQuyen.YeuCau(PhanQuyen.Admin, PhanQuyen.BanHang); return dal.LayChoDuyet(); }
        public void XuLy(long maYeuCau, bool chapNhan)
        {
            PhanQuyen.YeuCau(PhanQuyen.Admin, PhanQuyen.BanHang);
            dal.Duyet(maYeuCau, Session.TaiKhoan.MaTaiKhoan, chapNhan);
        }
    }
}
