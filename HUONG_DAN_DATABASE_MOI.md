# TourManager v0.7 - Cài database mới (WinForms)

## Cài đặt
1. Mở SQL Server Management Studio (SSMS), kết nối SQL Server.
2. Mở `database/00_CAI_MOI_TAT_CA.sql` và nhấn Execute (F5) **một lần**.
3. Script tạo database **TourManagerDB_New** độc lập, giữ nguyên **TourManagerDB** cũ.
4. Mở `TourManager.sln` bằng Visual Studio; sửa `src/TourManager.GUI/App.config` nếu tên server của bạn không phải `.\SQLEXPRESS`. Database đã được đổi thành `TourManagerDB_New`.
5. Build Solution rồi chạy.

## Các bảng và tính năng
TaiKhoan, KhachHang, HuongDanVien, Tour, NhatKy, LichTrinhTour, GiaTour, DotKhoiHanh, PhuongTien, KhuyenMai, CauHinhNghiepVu, Booking, ThanhToan, PhanCongHDV, PhanCongXe, TrangThaiDoan, ChinhSachHuy, YeuCauThanhToan.
Bao gồm các stored procedure và view phục vụ tìm tour, booking, thanh toán, yêu cầu thanh toán, báo cáo. Có tour/giá/đợt khởi hành mẫu để thử đặt tour.

## Lưu ý
- Script cài mới KHÔNG dành cho database đã có dữ liệu và KHÔNG nên chạy lại trên `TourManagerDB_New`.
- Tài khoản mẫu ở phần `02_seed.sql` (admin / Admin@123; banhang01 / BanHang@123); chỉ dùng thử nghiệm và đổi mật khẩu sau khi đăng nhập.
- Thanh toán khách hàng vẫn là **yêu cầu chờ nhân viên xác nhận**, không phải cổng thanh toán trực tuyến.
- Chưa có môi trường SQL Server/Visual Studio để chạy kiểm thử thực tế; nếu SQL Server báo lỗi hãy gửi ảnh thông báo để sửa tiếp.
