using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;
using TourManager.DAL;

namespace TourManager.GUI
{
    /// <summary>Tra cứu booking, ghi nhận thanh toán và xử lý hủy theo chính sách.</summary>
    public class frmQuanLyBooking : Form
    {
        private DataGridView grid;
        private TextBox txtTim;
        public frmQuanLyBooking()
        {
            Text = "Booking & thanh toán"; Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(245, 248, 252); WindowState = FormWindowState.Maximized;
            var top = new Panel { Dock = DockStyle.Top, Height = 95, BackColor = Color.White, Padding = new Padding(18) };
            var title = new Label { Text = "Đơn đặt tour", Font = new Font("Segoe UI", 19, FontStyle.Bold), AutoSize = true, Location = new Point(20, 8), ForeColor = Color.FromArgb(29, 49, 74) };
            txtTim = new TextBox { Location = new Point(22, 54), Width = 300, Font = new Font("Segoe UI", 10) }; txtTim.TextChanged += (s, e) => Loc();
            var refresh = ButtonOf("Làm mới", 340, TaiDuLieu); var pay = ButtonOf("Ghi thanh toán", 445, GhiThanhToan); var cancel = ButtonOf("Hủy booking", 590, HuyBooking);
            top.Controls.AddRange(new Control[] { title, txtTim, refresh, pay, cancel });
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 241, 250); grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold); grid.EnableHeadersVisualStyles = false;
            Controls.Add(grid); Controls.Add(top); TaiDuLieu();
        }
        private Button ButtonOf(string text, int x, Action action) { var b = new Button { Text = text, Location = new Point(x, 51), Size = new Size(130, 32), BackColor = Color.FromArgb(0, 112, 205), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; b.FlatAppearance.BorderSize = 0; b.Click += (s, e) => action(); return b; }
        private void TaiDuLieu()
        {
            try { grid.DataSource = DbHelper.ExecuteDataTable(@"SELECT b.MaBooking,b.MaCode,k.HoTen,k.SoDienThoai,t.TenTour,d.NgayDi,b.SoNguoiLon,b.SoTreEm,b.TongTien,
                ISNULL((SELECT SUM(SoTien) FROM ThanhToan x WHERE x.MaBooking=b.MaBooking),0) DaThanhToan,
                b.TongTien-ISNULL((SELECT SUM(SoTien) FROM ThanhToan x WHERE x.MaBooking=b.MaBooking),0) ConPhaiThu,b.TrangThai,b.NgayDat
                FROM Booking b JOIN KhachHang k ON k.MaKhachHang=b.MaKhachHang JOIN DotKhoiHanh d ON d.MaDot=b.MaDot JOIN Tour t ON t.MaTour=d.MaTour ORDER BY b.NgayDat DESC");
                foreach (string c in new[] { "MaBooking", "MaCode", "HoTen", "SoDienThoai", "TenTour", "NgayDi", "SoNguoiLon", "SoTreEm", "TongTien", "DaThanhToan", "ConPhaiThu", "TrangThai", "NgayDat" }) if (grid.Columns.Contains(c)) grid.Columns[c].HeaderText = new System.Collections.Generic.Dictionary<string,string> { {"MaBooking","Mã"},{"MaCode","Mã booking"},{"HoTen","Khách hàng"},{"SoDienThoai","Điện thoại"},{"TenTour","Tour"},{"NgayDi","Khởi hành"},{"SoNguoiLon","Người lớn"},{"SoTreEm","Trẻ em"},{"TongTien","Tổng tiền"},{"DaThanhToan","Đã thu"},{"ConPhaiThu","Còn phải thu"},{"TrangThai","Trạng thái"},{"NgayDat","Ngày đặt"} }[c];
                foreach (string c in new[] { "TongTien", "DaThanhToan", "ConPhaiThu" }) if (grid.Columns.Contains(c)) grid.Columns[c].DefaultCellStyle.Format = "N0";
                foreach (string c in new[] { "NgayDi", "NgayDat" }) if (grid.Columns.Contains(c)) grid.Columns[c].DefaultCellStyle.Format = "dd/MM/yyyy";
                Loc();
            } catch (Exception ex) { MessageBox.Show("Không tải được booking: " + ex.Message, "TourManager", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void Loc() { if (grid.DataSource is DataTable dt) { string q = (txtTim.Text ?? "").Trim().Replace("'", "''").Replace("[", "[[]").Replace("]", "[]]").Replace("%", "[%]").Replace("*", "[*]"); dt.DefaultView.RowFilter = q == "" ? "" : "Convert(MaCode,'System.String') LIKE '%" + q + "%' OR Convert(HoTen,'System.String') LIKE '%" + q + "%' OR Convert(SoDienThoai,'System.String') LIKE '%" + q + "%'"; } }
        private long? MaDangChon()
        {
            if (grid.CurrentRow == null || !grid.Columns.Contains("MaBooking") ||
                grid.CurrentRow.Cells["MaBooking"].Value == null ||
                grid.CurrentRow.Cells["MaBooking"].Value == DBNull.Value) return null;
            return Convert.ToInt64(grid.CurrentRow.Cells["MaBooking"].Value);
        }
        private void GhiThanhToan()
        {
            var ma = MaDangChon(); if (!ma.HasValue) { MessageBox.Show("Chọn một booking trước."); return; }
            if (!grid.Columns.Contains("ConPhaiThu") || !grid.Columns.Contains("TrangThai"))
            { MessageBox.Show("Thiếu dữ liệu thanh toán, vui lòng tải lại."); return; }
            if (Convert.ToString(grid.CurrentRow.Cells["TrangThai"].Value) == "Đã hủy")
            { MessageBox.Show("Không thể thanh toán booking đã hủy."); return; }
            decimal con = Convert.ToDecimal(grid.CurrentRow.Cells["ConPhaiThu"].Value); if (con <= 0) { MessageBox.Show("Booking này đã thanh toán đủ hoặc đã hủy."); return; }
            using (var f = new frmNhapThanhToan(con)) if (f.ShowDialog(this) == DialogResult.OK)
            {
                try { using (var cn = DbHelper.TaoKetNoi()) using (var cmd = new SqlCommand("sp_GhiThanhToan", cn)) { cmd.CommandType = CommandType.StoredProcedure; cmd.Parameters.AddWithValue("@MaBooking", ma.Value); cmd.Parameters.AddWithValue("@SoTien", f.SoTien); cmd.Parameters.AddWithValue("@PhuongThuc", f.PhuongThuc); cmd.Parameters.AddWithValue("@LoaiThanhToan", f.SoTien >= con ? "Toàn bộ" : "Đặt cọc"); cmd.Parameters.AddWithValue("@MaTaiKhoan", Session.TaiKhoan.MaTaiKhoan); cn.Open(); cmd.ExecuteNonQuery(); } MessageBox.Show("Đã ghi nhận thanh toán."); TaiDuLieu(); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Thanh toán không thành công", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
        }
        private void HuyBooking()
        {
            var ma = MaDangChon(); if (!ma.HasValue) { MessageBox.Show("Chọn một booking trước."); return; }
            if (!grid.Columns.Contains("TrangThai") || Convert.ToString(grid.CurrentRow.Cells["TrangThai"].Value) == "Đã hủy")
            { MessageBox.Show("Booking đã hủy hoặc dữ liệu không hợp lệ."); return; }
            if (MessageBox.Show("Xác nhận hủy booking đang chọn? Phí hủy được tính theo chính sách hệ thống.", "Hủy booking", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { using (var cn = DbHelper.TaoKetNoi()) using (var cmd = new SqlCommand("sp_HuyBooking", cn)) { cmd.CommandType = CommandType.StoredProcedure; cmd.Parameters.AddWithValue("@MaBooking", ma.Value); cmd.Parameters.AddWithValue("@MaTaiKhoan", Session.TaiKhoan.MaTaiKhoan); cn.Open(); using (var rd = cmd.ExecuteReader()) if (rd.Read()) MessageBox.Show("Đã hủy booking.\nPhí hủy: " + Convert.ToDecimal(rd["PhiHuy"]).ToString("N0") + " đ\nSố tiền dự kiến hoàn: " + Convert.ToDecimal(rd["SoTienHoan"]).ToString("N0") + " đ"); } TaiDuLieu(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
    internal class frmNhapThanhToan : Form
    {
        private NumericUpDown amount; private ComboBox method;
        public decimal SoTien { get { return amount.Value; } } public string PhuongThuc { get { return Convert.ToString(method.SelectedItem); } }
        public frmNhapThanhToan(decimal max)
        {
            Text = "Ghi nhận thanh toán"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; ClientSize = new Size(390, 220); Font = new Font("Segoe UI", 9.5F);
            Controls.Add(new Label { Text = "Số tiền (tối đa " + max.ToString("N0") + " đ)", Location = new Point(22, 25), AutoSize = true }); amount = new NumericUpDown { Location = new Point(22, 52), Width = 330, Maximum = max, Minimum = 1, DecimalPlaces = 0, ThousandsSeparator = true, Value = max }; Controls.Add(amount);
            Controls.Add(new Label { Text = "Phương thức", Location = new Point(22, 94), AutoSize = true }); method = new ComboBox { Location = new Point(22, 120), Width = 330, DropDownStyle = ComboBoxStyle.DropDownList }; method.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ" }); method.SelectedIndex = 0; Controls.Add(method);
            var ok = new Button { Text = "Xác nhận", Location = new Point(230, 165), Size = new Size(120, 34), BackColor = Color.FromArgb(0,112,205), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; ok.Click += (s,e) => { DialogResult = DialogResult.OK; Close(); }; Controls.Add(ok);
        }
    }
}
