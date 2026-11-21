# TourManager – WinForms + SQL Server

Ứng dụng C# WinForms (.NET Framework 4.8, C# 7.3), kiến trúc GUI–BLL–DAL–DTO, sử dụng SQL Server.

## Các phần đã có trong source hiện tại

- Đăng nhập, đăng xuất, phân quyền theo vai trò Admin / Bán hàng / Điều hành / HDV.
- Quản lý danh mục tour cơ bản.
- Dashboard WinForms giao diện xanh-trắng, điều hướng theo vai trò.
- Màn hình Khám phá tour: tìm theo tên/điểm đến, xem đợt khởi hành còn chỗ và giá người lớn.
- Tạo booking từ màn hình tour: nhập khách hàng, số người, mã khuyến mãi; gọi `sp_TaoBooking` để kiểm tra số chỗ và tính tiền.
- Màn hình Booking: tra cứu đơn, ghi nhận thanh toán qua `sp_GhiThanhToan`, hủy qua `sp_HuyBooking`.
- Báo cáo doanh thu, tour được quan tâm và tỷ lệ lấp đầy đợt khởi hành.
- Script nâng cấp database `database/03_upgrade_uc01_uc30.sql` bổ sung các bảng, view và stored procedure nền tảng.

## Cài đặt database

1. Mở SQL Server Management Studio và kết nối SQL Server.
2. Nếu tạo database mới: chạy `database/01_schema.sql`, sau đó `database/02_seed.sql`.
3. Chạy `database/03_upgrade_uc01_uc30.sql` để bổ sung nghiệp vụ tour, lịch khởi hành, khách hàng, booking, thanh toán và báo cáo.
4. **Nếu đang có database chứa dữ liệu:** không chạy lại `01_schema.sql` vì script khởi tạo cũ có lệnh DROP các bảng. Chỉ sao lưu database trước, sau đó kiểm tra và chạy script nâng cấp `03_upgrade_uc01_uc30.sql`.
5. Mở `src/TourManager.GUI/App.config`, sửa `Server=...` đúng SQL Server trên máy.
6. Mở `TourManager.sln` bằng Visual Studio có hỗ trợ .NET Framework 4.8, build và chạy.

## Luồng sử dụng hiện có

1. Đăng nhập bằng tài khoản nhân viên đã được tạo trong database.
2. Chọn **Khám phá & đặt tour** để tìm tour và đợt khởi hành.
3. Chọn một đợt còn chỗ, nhấn **Đặt tour**, nhập thông tin khách hàng.
4. Tài khoản Admin/Bán hàng vào **Booking & thanh toán** để ghi nhận tiền cọc/thanh toán hoặc hủy booking.
5. Xem số liệu trong **Báo cáo**.

## Trạng thái và giới hạn cần biết

Đây là bản triển khai giao diện và luồng nghiệp vụ đầu tiên, chưa phải bản hoàn thiện toàn bộ UC01–UC30. Phân hệ khách hàng hiện là màn hình khám phá/đặt tour được thao tác trong ứng dụng nội bộ và booking được gắn với tài khoản nhân viên đang đăng nhập; chưa có tài khoản khách hàng tự đăng nhập, lịch sử đơn riêng hay cổng thanh toán trực tuyến. Màn hình quản trị HDV, phương tiện, phân công, danh sách đoàn, cấu hình giá/lịch trình chi tiết, in vé và xuất Excel vẫn cần triển khai tiếp.

Môi trường làm việc hiện tại không có MSBuild/.NET Framework compiler và SQL Server, do đó source và SQL chưa được build/chạy kiểm chứng thực tế tại đây. Cần kiểm thử trên Visual Studio + SQL Server trước khi sử dụng dữ liệu thật.


## Đăng ký tài khoản khách hàng (v0.4)
1. Chạy lần lượt `03_upgrade_uc01_uc30.sql` và `04_dang_ky_khach_hang.sql` trên `TourManagerDB`.
2. Mở ứng dụng, tại màn hình đăng nhập chọn `Chưa có tài khoản? Đăng ký khách hàng`.
3. Điền họ tên, tên đăng nhập, mật khẩu, CCCD/hộ chiếu và số điện thoại.
4. Đăng nhập bằng tài khoản vừa tạo để vào phân hệ khám phá tour.

Mật khẩu được lưu dạng SHA-256 kèm salt ngẫu nhiên theo cơ chế hiện có của dự án. Migration không xóa dữ liệu.

## v0.5 – Khách hàng tự phục vụ (WinForms)

Chạy migration `database/05_thanh_toan_khach_hang.sql` sau `03_upgrade_uc01_uc30.sql` và `04_dang_ky_khach_hang.sql`.

- Khách hàng đăng ký ở màn hình đăng nhập (v0.4), sau đó đăng nhập bằng vai trò `KhachHang`.
- Khách có thể khám phá, đặt tour; hồ sơ đặt tour lấy từ tài khoản đang đăng nhập, không cho đổi CCCD trong màn hình đặt.
- Menu **Đơn hàng của tôi** chỉ tải booking của `MaKhachHang` hiện tại; có lịch sử yêu cầu thanh toán.
- Khách có thể gửi thông báo đã chuyển khoản/đề nghị thanh toán tiền mặt; **không** tự tạo dòng `ThanhToan` và **không** tự chuyển trạng thái booking sang `Đã thanh toán`.
- Nhân viên `Admin`/`BanHang` vào **Duyệt thanh toán**, kiểm tra thực tế rồi xác nhận hoặc từ chối. Chỉ khi xác nhận, `sp_GhiThanhToan` mới cập nhật khoản thu và trạng thái booking.

**Giới hạn:** Chưa tích hợp cổng thanh toán trực tuyến, QR ngân hàng hay đối soát ngân hàng tự động. Khách chỉ gửi yêu cầu xác nhận thanh toán, không chuyển tiền trong ứng dụng. Chưa kiểm thử thực tế trên SQL Server/Windows .NET Framework 4.8; cần build và kiểm tra trên máy Windows có SQL Server.


## v0.6 – Sửa lỗi và gia cố giao diện (20/11/2026)

- Sửa/giữ đúng điều kiện kiểm tra cột trong `frmKhachHang.DinhDangCot`: chỉ định dạng khi cột tồn tại.
- Kiểm tra cột và giá trị `null` trước khi hiển thị chi tiết tour, đặt tour, thao tác đơn hàng và duyệt thanh toán.
- Escape ký tự đặc biệt trong bộ lọc tour/booking để tránh lỗi biểu thức `DataView.RowFilter`.
- Chặn thao tác ghi thanh toán trên booking đã hủy ngay ở giao diện (SQL vẫn là lớp bảo vệ cuối cùng).
- Giữ nguyên database/migration 01–05 và luồng đăng ký, đăng nhập, đơn hàng, duyệt thanh toán. **Không chạy lại `01_schema.sql` trên database có dữ liệu.**
- Chưa thể build hoặc chạy SQL Server trong môi trường đóng gói. Vui lòng build trên Windows bằng Visual Studio và kiểm thử luồng nghiệp vụ thực tế.
