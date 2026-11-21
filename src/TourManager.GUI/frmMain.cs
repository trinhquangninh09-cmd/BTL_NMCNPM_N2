using System;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;

namespace TourManager.GUI
{
    /// <summary>Dashboard chính theo phong cách ứng dụng du lịch hiện đại.</summary>
    public class frmMain : Form
    {
        private readonly Color nen = Color.FromArgb(245, 248, 252);
        private Panel sidebar, content, header;
        private Label lblTieuDe, lblPhuDe;
        public bool DangXuat { get; private set; }

        public frmMain() { KhoiTao(); }

        private void KhoiTao()
        {
            Text = "TourManager | Nền tảng quản lý và đặt tour";
            Font = new Font("Segoe UI", 9.5F);
            BackColor = nen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1050, 680);

            sidebar = new Panel { Dock = DockStyle.Left, Width = 235, BackColor = Color.FromArgb(13, 43, 79) };
            var brand = new Label { Text = "✈  TOURMANAGER", ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), Dock = DockStyle.Top, Height = 82, TextAlign = ContentAlignment.MiddleCenter };
            sidebar.Controls.Add(brand);

            content = new Panel { Dock = DockStyle.Fill, BackColor = nen, Padding = new Padding(28), AutoScroll = true };
            header = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.White, Padding = new Padding(28, 14, 28, 10) };
            lblTieuDe = new Label { Text = "Tổng quan", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(29, 49, 74), AutoSize = true, Location = new Point(28, 12) };
            var tk = Session.TaiKhoan;
            if (tk == null) throw new InvalidOperationException("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
            lblPhuDe = new Label { Text = "Xin chào, " + tk.HoTen + "  •  " + PhanQuyen.TenVaiTro(tk.VaiTro), ForeColor = Color.FromArgb(105, 120, 140), AutoSize = true, Location = new Point(31, 52) };
            header.Controls.Add(lblTieuDe); header.Controls.Add(lblPhuDe);

            AddNav("Tổng quan", "⌂", () => HienTongQuan());
            AddNav("Khám phá & đặt tour", "✈", () => MoForm<frmKhachHang>("Khám phá & đặt tour"));
            if (PhanQuyen.CoQuyen(PhanQuyen.KhachHang))
                AddNav("Đơn hàng của tôi", "▧", () => MoForm<frmDonCuaToi>("Đơn hàng của tôi"));
            if (PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang))
                AddNav("Duyệt thanh toán", "✓", () => MoForm<frmDuyetThanhToan>("Duyệt thanh toán"));
            if (PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang, PhanQuyen.DieuHanh))
                AddNav("Quản lý danh mục tour", "▣", () => MoForm<frmTour>("Quản lý tour"));
            if (PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang))
                AddNav("Booking & thanh toán", "▧", () => MoForm<frmQuanLyBooking>("Booking & thanh toán"));
            if (PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang, PhanQuyen.DieuHanh))
                AddNav("Báo cáo", "▤", () => MoForm<frmBaoCao>("Báo cáo hoạt động"));
            AddNav("Đăng xuất", "⇥", DangXuatHeThong, true);
            Controls.Add(content); Controls.Add(header); Controls.Add(sidebar);
            HienTongQuan();
        }

        private void AddNav(string text, string icon, Action action, bool bottom = false)
        {
            var b = new Button { Text = "  " + icon + "     " + text, Dock = bottom ? DockStyle.Bottom : DockStyle.Top, Height = 52, FlatStyle = FlatStyle.Flat, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(18, 0, 0, 0), ForeColor = Color.FromArgb(226, 236, 248), BackColor = Color.FromArgb(13, 43, 79), Font = new Font("Segoe UI", 9.5F) };
            b.FlatAppearance.BorderSize = 0; b.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 75, 125); b.Click += (s, e) => action();
            sidebar.Controls.Add(b); b.BringToFront();
        }

        private void HienTongQuan()
        {
            content.Controls.Clear(); lblTieuDe.Text = "Tổng quan";
            var intro = new Label { Text = "Mọi hành trình tuyệt vời bắt đầu từ đây.", Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(100, 117, 139), AutoSize = true, Location = new Point(30, 22) };
            content.Controls.Add(intro);
            AddCard("KHÁM PHÁ TOUR", "Tìm điểm đến và lịch khởi hành", "Tìm tour ngay  →", 30, 85, Color.FromArgb(0, 112, 205), () => MoForm<frmKhachHang>("Khám phá & đặt tour"));
            if (PhanQuyen.CoQuyen(PhanQuyen.KhachHang))
                AddCard("ĐƠN HÀNG CỦA TÔI", "Theo dõi booking và yêu cầu thanh toán", "Xem đơn hàng  →", 30, 285, Color.FromArgb(19, 145, 126), () => MoForm<frmDonCuaToi>("Đơn hàng của tôi"));
            if (PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang, PhanQuyen.DieuHanh))
                AddCard("DANH MỤC TOUR", "Cập nhật thông tin các hành trình", "Quản lý tour  →", 30, 285, Color.FromArgb(19, 145, 126), () => MoForm<frmTour>("Quản lý tour"));
            if (PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang))
                AddCard("BOOKING & THANH TOÁN", "Tra cứu đơn, ghi nhận thanh toán và hủy tour", "Quản lý booking  →", 30, 485, Color.FromArgb(105, 91, 190), () => MoForm<frmQuanLyBooking>("Booking & thanh toán"));
            if (PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang, PhanQuyen.DieuHanh))
                AddCard("BÁO CÁO", "Theo dõi doanh thu và hiệu quả khởi hành", "Xem báo cáo  →", 30, 685, Color.FromArgb(218, 135, 48), () => MoForm<frmBaoCao>("Báo cáo hoạt động"));
        }

        private void AddCard(string title, string desc, string action, int x, int y, Color accent, Action click)
        {
            var card = new Panel { Location = new Point(x, y), Size = new Size(Math.Max(520, content.ClientSize.Width - 100), 155), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            var stripe = new Panel { Dock = DockStyle.Left, Width = 7, BackColor = accent };
            var t = new Label { Text = title, Location = new Point(28, 23), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = accent };
            var d = new Label { Text = desc, Location = new Point(28, 57), AutoSize = true, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(35, 53, 75) };
            var b = new Button { Text = action, Location = new Point(28, 101), Size = new Size(150, 34), BackColor = accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            b.FlatAppearance.BorderSize = 0; b.Click += (s, e) => click(); card.Controls.Add(stripe); card.Controls.Add(t); card.Controls.Add(d); card.Controls.Add(b); content.Controls.Add(card);
        }

        private void MoForm<T>(string title) where T : Form, new()
        {
            foreach (Form opened in Application.OpenForms) { if (opened is T) { opened.Activate(); return; } }
            var child = new T { Text = title, StartPosition = FormStartPosition.CenterScreen, WindowState = FormWindowState.Maximized };
            child.Show(this);
        }

        private void DangXuatHeThong()
        {
            if (MessageBox.Show("Bạn có muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            new TaiKhoanBLL().DangXuat(); DangXuat = true; Close();
        }
    }
}
