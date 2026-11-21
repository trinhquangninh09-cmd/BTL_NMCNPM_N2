namespace TourManager.DAL
{
    public class NhatKyDAL
    {
        public void Ghi(int maTaiKhoan, string hanhDong, string doiTuong, string noiDung)
        {
            DbHelper.ExecuteNonQuery(
                "INSERT INTO NhatKy (MaTaiKhoan, HanhDong, DoiTuong, NoiDung) VALUES (@Ma, @HanhDong, @DoiTuong, @NoiDung)",
                DbHelper.Param("@Ma", maTaiKhoan),
                DbHelper.Param("@HanhDong", hanhDong),
                DbHelper.Param("@DoiTuong", doiTuong),
                DbHelper.Param("@NoiDung", noiDung));
        }
    }
}
