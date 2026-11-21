using System;
using System.Data;
using System.Data.SqlClient;

namespace TourManager.DAL
{
    public class DangKyDAL
    {
        public void TaoTaiKhoanKhachHang(string tenDangNhap, string hoTen, string cccd, string sdt, string email, byte[] hash, byte[] salt)
        {
            using (var cn = DbHelper.TaoKetNoi())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        int maKhachHang;
                        using (var cmd = new SqlCommand(@"IF EXISTS(SELECT 1 FROM TaiKhoan WHERE TenDangNhap=@TenDangNhap) THROW 50001, N'Tên đăng nhập đã được sử dụng.', 1;
IF EXISTS(SELECT 1 FROM KhachHang WHERE CCCD=@CCCD) THROW 50002, N'Số CCCD/hộ chiếu đã được đăng ký.', 1;
INSERT INTO KhachHang(HoTen,CCCD,SoDienThoai,Email) VALUES(@HoTen,@CCCD,@SDT,@Email);
SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap); cmd.Parameters.AddWithValue("@HoTen", hoTen);
                            cmd.Parameters.AddWithValue("@CCCD", cccd); cmd.Parameters.AddWithValue("@SDT", sdt);
                            cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value); maKhachHang = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        using (var cmd = new SqlCommand(@"INSERT INTO TaiKhoan(TenDangNhap,HoTen,MatKhauHash,Salt,VaiTro,MaKhachHang)
VALUES(@TenDangNhap,@HoTen,@Hash,@Salt,N'KhachHang',@MaKhachHang);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap); cmd.Parameters.AddWithValue("@HoTen", hoTen);
                            cmd.Parameters.Add("@Hash", SqlDbType.VarBinary, 32).Value = hash; cmd.Parameters.Add("@Salt", SqlDbType.VarBinary, 16).Value = salt;
                            cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang); cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                    catch { tx.Rollback(); throw; }
                }
            }
        }
    }
}
