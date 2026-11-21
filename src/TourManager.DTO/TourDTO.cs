namespace TourManager.DTO
{
    /// <summary>Dữ liệu một tour trong danh mục.</summary>
    public class TourDTO
    {
        public int MaTour { get; set; }
        public string TenTour { get; set; }
        public string DiemDen { get; set; }
        public int SoNgay { get; set; }
        public int SoDem { get; set; }
        public string MoTa { get; set; }
        public string PhuongTienChinh { get; set; }
        public bool TrangThai { get; set; }

        // Thuộc tính chỉ để hiển thị trên lưới
        public string NgayDemText { get { return SoNgay + "N" + SoDem + "Đ"; } }
        public string TrangThaiText { get { return TrangThai ? "Đang bán" : "Đã ẩn"; } }
    }
}
