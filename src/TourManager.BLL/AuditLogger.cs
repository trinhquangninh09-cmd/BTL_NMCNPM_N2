using System;
using TourManager.DAL;

namespace TourManager.BLL
{
    /// <summary>Ghi nhật ký thao tác (NFR10). Lỗi khi ghi nhật ký không làm hỏng thao tác chính.</summary>
    public static class AuditLogger
    {
        private static readonly NhatKyDAL _dal = new NhatKyDAL();

        public static void Ghi(string hanhDong, string doiTuong, string noiDung)
        {
            if (!Session.DaDangNhap) return;
            Ghi(Session.TaiKhoan.MaTaiKhoan, hanhDong, doiTuong, noiDung);
        }

        public static void Ghi(int maTaiKhoan, string hanhDong, string doiTuong, string noiDung)
        {
            try { _dal.Ghi(maTaiKhoan, hanhDong, doiTuong, noiDung); }
            catch (Exception) { /* bỏ qua */ }
        }
    }
}
