using System;
using System.Text.RegularExpressions;
using TourManager.DAL;

namespace TourManager.BLL
{
    public class DangKyBLL
    {
        private readonly DangKyDAL _dal = new DangKyDAL();
        public void DangKyKhachHang(string hoTen, string tenDangNhap, string matKhau, string xacNhan, string cccd, string sdt, string email)
        {
            hoTen = (hoTen ?? "").Trim(); tenDangNhap = (tenDangNhap ?? "").Trim(); cccd = (cccd ?? "").Trim(); sdt = (sdt ?? "").Trim(); email = (email ?? "").Trim();
            if (hoTen.Length < 2) throw new BusinessException("Vui lòng nhập họ tên hợp lệ.");
            if (!Regex.IsMatch(tenDangNhap, @"^[a-zA-Z0-9_.]{4,50}$")) throw new BusinessException("Tên đăng nhập phải có 4–50 ký tự: chữ, số, dấu chấm hoặc gạch dưới.");
            if (string.IsNullOrEmpty(matKhau) || matKhau.Length < 8) throw new BusinessException("Mật khẩu phải có ít nhất 8 ký tự.");
            if (matKhau != xacNhan) throw new BusinessException("Mật khẩu xác nhận không khớp.");
            if (cccd.Length < 5) throw new BusinessException("Vui lòng nhập CCCD hoặc hộ chiếu hợp lệ.");
            if (!Regex.IsMatch(sdt, @"^[0-9+ ]{8,20}$")) throw new BusinessException("Số điện thoại không hợp lệ.");
            if (email.Length > 0 && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new BusinessException("Email không hợp lệ.");
            byte[] salt = PasswordHasher.TaoSalt();
            _dal.TaoTaiKhoanKhachHang(tenDangNhap, hoTen, cccd, sdt, email.Length == 0 ? null : email, PasswordHasher.Bam(matKhau, salt), salt);
        }
    }
}
