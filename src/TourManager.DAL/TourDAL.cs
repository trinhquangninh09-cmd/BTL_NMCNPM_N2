using System;
using System.Collections.Generic;
using System.Data;
using TourManager.DTO;

namespace TourManager.DAL
{
    public class TourDAL
    {
        public List<TourDTO> LayDanhSach(string tuKhoa, bool chiDangBan)
        {
            const string sql =
                "SELECT MaTour, TenTour, DiemDen, SoNgay, SoDem, MoTa, PhuongTienChinh, TrangThai " +
                "FROM Tour " +
                "WHERE (TenTour LIKE @Like OR DiemDen LIKE @Like) AND (@ChiDangBan = 0 OR TrangThai = 1) " +
                "ORDER BY MaTour";
            DataTable dt = DbHelper.ExecuteDataTable(sql,
                DbHelper.Param("@Like", "%" + (tuKhoa ?? "").Trim() + "%"),
                DbHelper.Param("@ChiDangBan", chiDangBan ? 1 : 0));
            var ds = new List<TourDTO>();
            foreach (DataRow r in dt.Rows) ds.Add(Map(r));
            return ds;
        }

        public int Them(TourDTO t)
        {
            const string sql =
                "INSERT INTO Tour (TenTour, DiemDen, SoNgay, SoDem, MoTa, PhuongTienChinh, TrangThai) " +
                "VALUES (@Ten, @DiemDen, @SoNgay, @SoDem, @MoTa, @PhuongTien, @TrangThai); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";
            object kq = DbHelper.ExecuteScalar(sql,
                DbHelper.Param("@Ten", t.TenTour),
                DbHelper.Param("@DiemDen", t.DiemDen),
                DbHelper.Param("@SoNgay", t.SoNgay),
                DbHelper.Param("@SoDem", t.SoDem),
                DbHelper.Param("@MoTa", t.MoTa),
                DbHelper.Param("@PhuongTien", t.PhuongTienChinh),
                DbHelper.Param("@TrangThai", t.TrangThai));
            return Convert.ToInt32(kq);
        }

        public void Sua(TourDTO t)
        {
            const string sql =
                "UPDATE Tour SET TenTour = @Ten, DiemDen = @DiemDen, SoNgay = @SoNgay, SoDem = @SoDem, " +
                "MoTa = @MoTa, PhuongTienChinh = @PhuongTien, TrangThai = @TrangThai WHERE MaTour = @Ma";
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.Param("@Ma", t.MaTour),
                DbHelper.Param("@Ten", t.TenTour),
                DbHelper.Param("@DiemDen", t.DiemDen),
                DbHelper.Param("@SoNgay", t.SoNgay),
                DbHelper.Param("@SoDem", t.SoDem),
                DbHelper.Param("@MoTa", t.MoTa),
                DbHelper.Param("@PhuongTien", t.PhuongTienChinh),
                DbHelper.Param("@TrangThai", t.TrangThai));
        }

        public void DatTrangThai(int maTour, bool dangBan)
        {
            DbHelper.ExecuteNonQuery("UPDATE Tour SET TrangThai = @TrangThai WHERE MaTour = @Ma",
                DbHelper.Param("@Ma", maTour),
                DbHelper.Param("@TrangThai", dangBan));
        }

        private static TourDTO Map(DataRow r)
        {
            return new TourDTO
            {
                MaTour = Convert.ToInt32(r["MaTour"]),
                TenTour = (string)r["TenTour"],
                DiemDen = (string)r["DiemDen"],
                SoNgay = Convert.ToInt32(r["SoNgay"]),
                SoDem = Convert.ToInt32(r["SoDem"]),
                MoTa = r["MoTa"] as string,
                PhuongTienChinh = r["PhuongTienChinh"] as string,
                TrangThai = (bool)r["TrangThai"]
            };
        }
    }
}
