using System;
using System.Data;
using TourManager.DTO;

namespace TourManager.DAL
{
    public class TaiKhoanDAL
    {
        public TaiKhoanDTO LayTheoTenDangNhap(string tenDangNhap)
        {
            DataTable dt = DbHelper.ExecuteDataTable(
                "SELECT MaTaiKhoan, TenDangNhap, HoTen, MatKhauHash, Salt, VaiTro, TrangThai, SoLanSai, KhoaDen, MaHDV, MaKhachHang " +
                "FROM TaiKhoan WHERE TenDangNhap = @Ten",
                DbHelper.Param("@Ten", tenDangNhap));
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        /// <summary>Tăng số lần sai; nếu đạt ngưỡng thì khóa tạm thời. Trả về true nếu tài khoản đang bị khóa.</summary>
        public bool GhiNhanDangNhapSai(int maTaiKhoan, int soLanToiDa, int phutKhoa)
        {
            const string sql =
                "UPDATE TaiKhoan SET SoLanSai = SoLanSai + 1 WHERE MaTaiKhoan = @Ma; " +
                "UPDATE TaiKhoan SET KhoaDen = DATEADD(MINUTE, @Phut, GETDATE()), SoLanSai = 0 " +
                "WHERE MaTaiKhoan = @Ma AND SoLanSai >= @Max; " +
                "SELECT CASE WHEN KhoaDen > GETDATE() THEN 1 ELSE 0 END FROM TaiKhoan WHERE MaTaiKhoan = @Ma;";
            object kq = DbHelper.ExecuteScalar(sql,
                DbHelper.Param("@Ma", maTaiKhoan),
                DbHelper.Param("@Phut", phutKhoa),
                DbHelper.Param("@Max", soLanToiDa));
            return Convert.ToInt32(kq) == 1;
        }

        public void DatLaiDangNhap(int maTaiKhoan)
        {
            DbHelper.ExecuteNonQuery(
                "UPDATE TaiKhoan SET SoLanSai = 0, KhoaDen = NULL WHERE MaTaiKhoan = @Ma",
                DbHelper.Param("@Ma", maTaiKhoan));
        }

        private static TaiKhoanDTO Map(DataRow r)
        {
            return new TaiKhoanDTO
            {
                MaTaiKhoan = Convert.ToInt32(r["MaTaiKhoan"]),
                TenDangNhap = (string)r["TenDangNhap"],
                HoTen = (string)r["HoTen"],
                MatKhauHash = (byte[])r["MatKhauHash"],
                Salt = (byte[])r["Salt"],
                VaiTro = (string)r["VaiTro"],
                TrangThai = (bool)r["TrangThai"],
                SoLanSai = Convert.ToInt32(r["SoLanSai"]),
                KhoaDen = r["KhoaDen"] == DBNull.Value ? (DateTime?)null : (DateTime)r["KhoaDen"],
                MaHDV = r["MaHDV"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaHDV"]),
                MaKhachHang = r["MaKhachHang"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaKhachHang"])
            };
        }
    }
}
