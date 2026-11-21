using System;
using System.Linq;

namespace TourManager.BLL
{
    /// <summary>Kiểm tra quyền theo vai trò (NFR03). Mọi thao tác nghiệp vụ gọi YeuCau() ở lớp BLL.</summary>
    public static class PhanQuyen
    {
        public const string Admin = "Admin";
        public const string BanHang = "BanHang";
        public const string DieuHanh = "DieuHanh";
        public const string HDV = "HDV";
        public const string KhachHang = "KhachHang";

        public static bool CoQuyen(params string[] cacVaiTro)
        {
            return Session.DaDangNhap && cacVaiTro.Contains(Session.TaiKhoan.VaiTro);
        }

        public static void YeuCau(params string[] cacVaiTro)
        {
            if (!CoQuyen(cacVaiTro))
            {
                AuditLogger.Ghi("Từ chối", "PhanQuyen", "Thao tác trái quyền");
                throw new BusinessException("Bạn không có quyền thực hiện thao tác này.");
            }
        }

        public static string TenVaiTro(string vaiTro)
        {
            switch (vaiTro)
            {
                case Admin: return "Quản trị";
                case BanHang: return "Bán hàng";
                case DieuHanh: return "Điều hành";
                case HDV: return "Hướng dẫn viên";
                case KhachHang: return "Khách hàng";
                default: return vaiTro;
            }
        }
    }
}
