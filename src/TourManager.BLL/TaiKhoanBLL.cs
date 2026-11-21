using System;
using TourManager.DAL;
using TourManager.DTO;

namespace TourManager.BLL
{
    public class TaiKhoanBLL
    {
        public const int SO_LAN_SAI_TOI_DA = 5;
        public const int PHUT_KHOA = 15;
        private const string SAI_THONG_TIN = "Tên đăng nhập hoặc mật khẩu không đúng.";

        private readonly TaiKhoanDAL _dal = new TaiKhoanDAL();

        /// <summary>UC01 – Đăng nhập. Thành công thì lưu vào Session.</summary>
        public TaiKhoanDTO DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrEmpty(matKhau))
                throw new BusinessException("Vui lòng nhập tên đăng nhập và mật khẩu.");

            TaiKhoanDTO tk = _dal.LayTheoTenDangNhap(tenDangNhap.Trim());
            if (tk == null) throw new BusinessException(SAI_THONG_TIN);

            if (!tk.TrangThai)
                throw new BusinessException("Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
            if (tk.KhoaDen.HasValue && tk.KhoaDen.Value > DateTime.Now)
                throw new BusinessException("Tài khoản đang bị khóa tạm thời đến " + tk.KhoaDen.Value.ToString("HH:mm dd/MM/yyyy") + ".");

            if (!PasswordHasher.KiemTra(matKhau, tk.Salt, tk.MatKhauHash))
            {
                bool biKhoa = _dal.GhiNhanDangNhapSai(tk.MaTaiKhoan, SO_LAN_SAI_TOI_DA, PHUT_KHOA);
                AuditLogger.Ghi(tk.MaTaiKhoan, "Từ chối", "DangNhap", "Sai mật khẩu");
                if (biKhoa)
                    throw new BusinessException("Bạn đã nhập sai " + SO_LAN_SAI_TOI_DA + " lần. Tài khoản bị khóa " + PHUT_KHOA + " phút.");
                throw new BusinessException(SAI_THONG_TIN);
            }

            _dal.DatLaiDangNhap(tk.MaTaiKhoan);
            Session.DangNhap(tk);
            AuditLogger.Ghi("Đăng nhập", "DangNhap", null);
            return tk;
        }

        public void DangXuat()
        {
            AuditLogger.Ghi("Đăng xuất", "DangNhap", null);
            Session.Xoa();
        }
    }
}
