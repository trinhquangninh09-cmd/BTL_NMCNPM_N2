using System.Collections.Generic;
using TourManager.DAL;
using TourManager.DTO;

namespace TourManager.BLL
{
    /// <summary>UC05 – Quản lý danh mục tour (FR06). Chỉ Admin được thêm/sửa/ẩn; Admin, Bán hàng, Điều hành được xem.</summary>
    public class TourBLL
    {
        private readonly TourDAL _dal = new TourDAL();

        public List<TourDTO> LayDanhSach(string tuKhoa, bool chiDangBan)
        {
            PhanQuyen.YeuCau(PhanQuyen.Admin, PhanQuyen.BanHang, PhanQuyen.DieuHanh);
            return _dal.LayDanhSach(tuKhoa, chiDangBan);
        }

        public int Them(TourDTO t)
        {
            PhanQuyen.YeuCau(PhanQuyen.Admin);
            KiemTraHopLe(t);
            int ma = _dal.Them(t);
            AuditLogger.Ghi("Thêm", "Tour", "MaTour=" + ma + "; " + t.TenTour);
            return ma;
        }

        public void Sua(TourDTO t)
        {
            PhanQuyen.YeuCau(PhanQuyen.Admin);
            KiemTraHopLe(t);
            _dal.Sua(t);
            AuditLogger.Ghi("Sửa", "Tour", "MaTour=" + t.MaTour + "; " + t.TenTour);
        }

        public void DatTrangThai(int maTour, bool dangBan)
        {
            PhanQuyen.YeuCau(PhanQuyen.Admin);
            _dal.DatTrangThai(maTour, dangBan);
            AuditLogger.Ghi("Sửa", "Tour", "MaTour=" + maTour + (dangBan ? "; hiện tour" : "; ẩn tour"));
        }

        private static void KiemTraHopLe(TourDTO t)
        {
            if (t == null) throw new BusinessException("Thiếu dữ liệu tour.");
            t.TenTour = (t.TenTour ?? "").Trim();
            t.DiemDen = (t.DiemDen ?? "").Trim();
            if (t.TenTour.Length == 0) throw new BusinessException("Vui lòng nhập tên tour.");
            if (t.TenTour.Length > 150) throw new BusinessException("Tên tour tối đa 150 ký tự.");
            if (t.DiemDen.Length == 0) throw new BusinessException("Vui lòng nhập điểm đến.");
            if (t.DiemDen.Length > 100) throw new BusinessException("Điểm đến tối đa 100 ký tự.");
            if (t.SoNgay < 1 || t.SoNgay > 60) throw new BusinessException("Số ngày phải từ 1 đến 60.");
            if (t.SoDem < 0 || t.SoDem > t.SoNgay) throw new BusinessException("Số đêm phải từ 0 đến số ngày.");
            if (!string.IsNullOrEmpty(t.PhuongTienChinh) && t.PhuongTienChinh.Length > 50)
                throw new BusinessException("Phương tiện chính tối đa 50 ký tự.");
        }
    }
}
