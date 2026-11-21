using System;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;

namespace TourManager.GUI
{
    public class frmDangKy : Form
    {
        private TextBox hoTen, tenDangNhap, matKhau, xacNhan, cccd, sdt, email;
        private Label loi;
        public string TenDangNhap { get { return tenDangNhap.Text.Trim(); } }
        public frmDangKy()
        {
            Text = "Đăng ký tài khoản khách hàng – TourManager"; StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            ClientSize = new Size(520, 510); BackColor = Color.White; Font = new Font("Segoe UI", 9.5F);
            var title = new Label { Text = "TẠO TÀI KHOẢN KHÁCH HÀNG", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(0, 91, 170), Location = new Point(28, 20), AutoSize = true }; Controls.Add(title);
            hoTen = AddField("Họ và tên", 75); tenDangNhap = AddField("Tên đăng nhập", 125); matKhau = AddField("Mật khẩu", 175); matKhau.UseSystemPasswordChar = true; xacNhan = AddField("Nhập lại mật khẩu", 225); xacNhan.UseSystemPasswordChar = true; cccd = AddField("CCCD / hộ chiếu", 275); sdt = AddField("Số điện thoại", 325); email = AddField("Email (không bắt buộc)", 375);
            loi = new Label { Location = new Point(30, 420), Size = new Size(460, 34), ForeColor = Color.Firebrick }; Controls.Add(loi);
            var cancel = new Button { Text = "Hủy", Location = new Point(275, 462), Size = new Size(90, 34), DialogResult = DialogResult.Cancel };
            var ok = new Button { Text = "ĐĂNG KÝ", Location = new Point(375, 462), Size = new Size(115, 34), BackColor = Color.FromArgb(0, 112, 205), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; ok.FlatAppearance.BorderSize = 0; ok.Click += DangKy; Controls.Add(cancel); Controls.Add(ok); AcceptButton = ok; CancelButton = cancel;
        }
        private TextBox AddField(string label, int y) { Controls.Add(new Label { Text = label, Location = new Point(32, y + 4), AutoSize = true }); var t = new TextBox { Location = new Point(205, y), Width = 285 }; Controls.Add(t); return t; }
        private void DangKy(object sender, EventArgs e)
        {
            loi.Text = "";
            try { new DangKyBLL().DangKyKhachHang(hoTen.Text, tenDangNhap.Text, matKhau.Text, xacNhan.Text, cccd.Text, sdt.Text, email.Text); DialogResult = DialogResult.OK; Close(); }
            catch (BusinessException ex) { loi.Text = ex.Message; }
            catch (Exception ex) { MessageBox.Show("Không thể đăng ký. Kiểm tra đã chạy migration 04_dang_ky_khach_hang.sql và kết nối SQL Server.\n\n" + ex.Message, "Lỗi đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}
