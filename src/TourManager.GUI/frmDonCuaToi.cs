using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;

namespace TourManager.GUI
{
    public class frmDonCuaToi : Form
    {
        private readonly DonHangKhachBLL bll = new DonHangKhachBLL();
        private DataGridView grid, yeuCau;
        private readonly Color xanh = Color.FromArgb(0, 112, 205);
        public frmDonCuaToi()
        {
            PhanQuyen.YeuCau(PhanQuyen.KhachHang);
            Text = "Đơn hàng của tôi"; Font = new Font("Segoe UI", 9.5f); BackColor = Color.FromArgb(245,248,252);
            MinimumSize = new Size(900, 600); WindowState = FormWindowState.Maximized;
            var top = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = Color.White };
            top.Controls.Add(new Label { Text = "Đơn hàng của tôi", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = xanh, Location = new Point(24,12), AutoSize = true });
            top.Controls.Add(new Label { Text = "Theo dõi lịch khởi hành, số tiền đã xác nhận và yêu cầu thanh toán.", Location = new Point(26,54), AutoSize = true });
            var reload = Nut("Làm mới", 25, TaiDuLieu);
            var pay = Nut("Gửi yêu cầu thanh toán", 165, ThanhToan);
            top.Controls.Add(reload); top.Controls.Add(pay);
            grid = TaoBang(); yeuCau = TaoBang();
            var tabs = new TabControl { Dock = DockStyle.Fill };
            var t1 = new TabPage("Đơn đặt tour") { Padding = new Padding(8) }; t1.Controls.Add(grid);
            var t2 = new TabPage("Lịch sử yêu cầu thanh toán") { Padding = new Padding(8) }; t2.Controls.Add(yeuCau);
            tabs.TabPages.Add(t1); tabs.TabPages.Add(t2);
            Controls.Add(tabs); Controls.Add(top); TaiDuLieu();
        }
        private Button Nut(string title, int x, Action click)
        {
            var b = new Button { Text = title, Location = new Point(x,75), Width = title.Length>12 ? 205 : 125, Height = 30, FlatStyle = FlatStyle.Flat, BackColor = xanh, ForeColor = Color.White };
            b.FlatAppearance.BorderSize = 0; b.Click += (s,e) => click(); return b;
        }
        private DataGridView TaoBang()
        {
            var g = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
            g.RowTemplate.Height = 36; return g;
        }
        private void TaiDuLieu()
        {
            try
            {
                grid.DataSource = bll.DonCuaToi(); yeuCau.DataSource = bll.LichSuYeuCau();
                foreach (string name in new[] { "TongTien", "DaThanhToan", "DangChoXacNhan" })
                    if (grid.Columns.Contains(name)) grid.Columns[name].DefaultCellStyle.Format = "N0";
                if (yeuCau.Columns.Contains("SoTien")) yeuCau.Columns["SoTien"].DefaultCellStyle.Format = "N0";
                if (grid.Columns.Contains("MaBooking")) grid.Columns["MaBooking"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Không thể tải đơn hàng. Kiểm tra migration 05 và kết nối SQL Server.\n" + ex.Message); }
        }
        private void ThanhToan()
        {
            if (grid.CurrentRow == null || !grid.Columns.Contains("MaBooking") ||
                !grid.Columns.Contains("TrangThai") || !grid.Columns.Contains("TongTien") ||
                !grid.Columns.Contains("DaThanhToan") || !grid.Columns.Contains("DangChoXacNhan"))
            { MessageBox.Show("Chọn một đơn hàng hợp lệ trước."); return; }
            var r = grid.CurrentRow;
            if (Convert.ToString(r.Cells["TrangThai"].Value) == "Đã hủy") { MessageBox.Show("Không thể thanh toán đơn đã hủy."); return; }
            decimal con = Convert.ToDecimal(r.Cells["TongTien"].Value) - Convert.ToDecimal(r.Cells["DaThanhToan"].Value) - Convert.ToDecimal(r.Cells["DangChoXacNhan"].Value);
            if (con <= 0) { MessageBox.Show("Không còn số tiền có thể gửi yêu cầu."); return; }
            using (var f = new frmGuiYeuCauThanhToan(con)) if (f.ShowDialog(this) == DialogResult.OK)
            {
                try { bll.GuiYeuCau(Convert.ToInt64(r.Cells["MaBooking"].Value), f.SoTien, f.PhuongThuc, f.ThamChieu);
                    MessageBox.Show("Đã gửi yêu cầu. Nhân viên sẽ kiểm tra khoản thanh toán trước khi xác nhận."); TaiDuLieu(); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Chưa gửi được yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
        }
    }
    internal class frmGuiYeuCauThanhToan : Form
    {
        private NumericUpDown amount; private ComboBox method; private TextBox reference;
        public decimal SoTien { get { return amount.Value; } }
        public string PhuongThuc { get { return Convert.ToString(method.SelectedItem); } }
        public string ThamChieu { get { return reference.Text.Trim(); } }
        public frmGuiYeuCauThanhToan(decimal max)
        {
            Text = "Thông báo thanh toán"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;
            ClientSize = new Size(450,290); MaximizeBox = false; MinimizeBox = false; Font = new Font("Segoe UI", 9.5f);
            Controls.Add(new Label { Text = "Số tiền muốn thông báo (còn tối đa " + max.ToString("N0") + " đ)", Location = new Point(20,20), AutoSize = true });
            amount = new NumericUpDown { Location = new Point(20,48), Width = 400, Minimum = 1, Maximum = max, Value = max, ThousandsSeparator = true }; Controls.Add(amount);
            Controls.Add(new Label { Text = "Phương thức", Location = new Point(20,89), AutoSize = true });
            method = new ComboBox { Location = new Point(20,115), Width = 400, DropDownStyle = ComboBoxStyle.DropDownList };
            method.Items.AddRange(new object[] { "Chuyển khoản", "Tiền mặt" }); method.SelectedIndex = 0; Controls.Add(method);
            Controls.Add(new Label { Text = "Mã giao dịch / ghi chú đối chiếu (nếu có)", Location = new Point(20,154), AutoSize = true });
            reference = new TextBox { Location = new Point(20,180), Width = 400, MaxLength = 100 }; Controls.Add(reference);
            Controls.Add(new Label { Text = "Đây là thông báo, chưa phải thanh toán đã xác nhận.", Location = new Point(20,211), AutoSize = true, ForeColor = Color.DarkOrange });
            var ok = new Button { Text = "Gửi yêu cầu", Location = new Point(290,242), Width = 130, Height = 34, BackColor = Color.FromArgb(0,112,205), ForeColor = Color.White };
            ok.Click += (s,e) => { DialogResult = DialogResult.OK; Close(); }; Controls.Add(ok);
        }
    }
}
