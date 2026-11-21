using System;

namespace TourManager.DTO
{
    /// <summary>Dữ liệu một tài khoản đăng nhập.</summary>
    public class TaiKhoanDTO
    {
        public int MaTaiKhoan { get; set; }
        public string TenDangNhap { get; set; }
        public string HoTen { get; set; }
        public byte[] MatKhauHash { get; set; }
        public byte[] Salt { get; set; }
        public string VaiTro { get; set; }
        public bool TrangThai { get; set; }
        public int SoLanSai { get; set; }
        public DateTime? KhoaDen { get; set; }
        public int? MaHDV { get; set; }
        public int? MaKhachHang { get; set; }
    }
}
