using System;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;

namespace TourManager.GUI
{
    /// <summary>UC01 – Đăng nhập.</summary>
    public class frmDangNhap : Form
    {
        private TextBox txtTenDangNhap;
        private TextBox txtMatKhau;
        private CheckBox chkHienMatKhau;
        private Button btnDangNhap;
        private Button btnThoat;
        private Button btnDangKy;
        private Label lblLoi;

        private readonly TaiKhoanBLL _bll = new TaiKhoanBLL();

        public frmDangNhap()
        {
            KhoiTao();
        }

        private void KhoiTao()
        {
            Text = "Đăng nhập hệ thống – TourManager";
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(460, 350);

            var lblTieuDe = new Label
            {
                Text = "QUẢN LÝ ĐẶT TOUR DU LỊCH",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(43, 87, 154),
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 20, 460, 40)
            };
            var lblPhu = new Label
            {
                Text = "Vui lòng đăng nhập để tiếp tục",
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 62, 460, 24)
            };

            var lblTen = new Label { Text = "Tên đăng nhập", Location = new Point(40, 112), AutoSize = true };
            txtTenDangNhap = new TextBox { Location = new Point(170, 108), Width = 250 };

            var lblMk = new Label { Text = "Mật khẩu", Location = new Point(40, 152), AutoSize = true };
            txtMatKhau = new TextBox { Location = new Point(170, 148), Width = 250, UseSystemPasswordChar = true };

            chkHienMatKhau = new CheckBox { Text = "Hiện mật khẩu", Location = new Point(170, 182), AutoSize = true };
            chkHienMatKhau.CheckedChanged += (s, e) => txtMatKhau.UseSystemPasswordChar = !chkHienMatKhau.Checked;

            lblLoi = new Label
            {
                ForeColor = Color.Firebrick,
                Bounds = new Rectangle(40, 205, 380, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnDangNhap = new Button { Text = "Đăng nhập", Location = new Point(170, 258), Size = new Size(110, 34) };
            btnDangNhap.BackColor = Color.FromArgb(43, 87, 154);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.Click += btnDangNhap_Click;

            btnThoat = new Button { Text = "Thoát", Location = new Point(310, 258), Size = new Size(110, 34) };
            btnThoat.DialogResult = DialogResult.Cancel;

            btnDangKy = new Button { Text = "Chưa có tài khoản? Đăng ký khách hàng", Location = new Point(75, 303), Size = new Size(310, 30), FlatStyle = FlatStyle.Flat, ForeColor = Color.FromArgb(0, 112, 205), BackColor = Color.White };
            btnDangKy.FlatAppearance.BorderSize = 0;
            btnDangKy.Click += (s, e) => { using (var f = new frmDangKy()) { if (f.ShowDialog(this) == DialogResult.OK) { txtTenDangNhap.Text = f.TenDangNhap; txtMatKhau.Clear(); txtMatKhau.Focus(); MessageBox.Show("Đăng ký thành công. Hãy đăng nhập để tiếp tục.", "TourManager", MessageBoxButtons.OK, MessageBoxIcon.Information); } } };

            Controls.AddRange(new Control[]
            {
                lblTieuDe, lblPhu, lblTen, txtTenDangNhap, lblMk, txtMatKhau,
                chkHienMatKhau, lblLoi, btnDangNhap, btnThoat, btnDangKy
            });
            AcceptButton = btnDangNhap;
            CancelButton = btnThoat;
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            lblLoi.Text = "";
            try
            {
                _bll.DangNhap(txtTenDangNhap.Text, txtMatKhau.Text);
                DialogResult = DialogResult.OK;
            }
            catch (BusinessException ex)
            {
                lblLoi.Text = ex.Message;
                txtMatKhau.Clear();
                txtMatKhau.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được cơ sở dữ liệu. Hãy kiểm tra chuỗi kết nối trong App.config.\n\n" + ex.Message,
                    "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
