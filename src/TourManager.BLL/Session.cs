using TourManager.DTO;

namespace TourManager.BLL
{
    /// <summary>Thông tin phiên làm việc của người dùng đang đăng nhập.</summary>
    public static class Session
    {
        public static TaiKhoanDTO TaiKhoan { get; private set; }
        public static bool DaDangNhap { get { return TaiKhoan != null; } }

        public static void DangNhap(TaiKhoanDTO tk) { TaiKhoan = tk; }
        public static void Xoa() { TaiKhoan = null; }
    }
}
