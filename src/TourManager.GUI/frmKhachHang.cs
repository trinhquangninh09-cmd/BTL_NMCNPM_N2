using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;
using TourManager.DAL;

namespace TourManager.GUI
{
    /// <summary>Phân hệ khám phá tour và tạo booking từ thông tin khách hàng.</summary>
    public class frmKhachHang : Form
    {
        private TextBox txtTim;
        private DataGridView grid;
        private Label lblChiTiet;
        private DataTable duLieu;
        private readonly Color xanh = Color.FromArgb(0, 112, 205);

        public frmKhachHang() { KhoiTao(); TaiDuLieu(); }

        private void KhoiTao()
        {
            Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(245, 248, 252); MinimumSize = new Size(850, 560);
            var top = new Panel { Dock = DockStyle.Top, Height = 118, BackColor = Color.White, Padding = new Padding(20) };
            var title = new Label { Text = "Khám phá hành trình", Font = new Font("Segoe UI", 19, FontStyle.Bold), ForeColor = Color.FromArgb(29, 49, 74), Location = new Point(22, 12), AutoSize = true };
            var sub = new Label { Text = "Chọn tour yêu thích, xem lịch khởi hành và tạo yêu cầu đặt chỗ.", ForeColor = Color.Gray, Location = new Point(25, 49), AutoSize = true };
            txtTim = new TextBox { Location = new Point(25, 78), Width = 330, Height = 28, Font = new Font("Segoe UI", 10) };
            txtTim.TextChanged += (s, e) => LocDuLieu();
            var refresh = new Button { Text = "Làm mới", Location = new Point(365, 75), Size = new Size(95, 32), BackColor = xanh, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            refresh.FlatAppearance.BorderSize = 0; refresh.Click += (s, e) => TaiDuLieu();
            top.Controls.AddRange(new Control[] { title, sub, txtTim, refresh });

            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, EnableHeadersVisualStyles = false };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 241, 250); grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(40, 62, 86); grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold); grid.RowTemplate.Height = 38;
            grid.SelectionChanged += (s, e) => HienChiTiet();
            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 150, BackColor = Color.White, Padding = new Padding(18) };
            lblChiTiet = new Label { Text = "Chọn một đợt khởi hành để xem chi tiết.", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(75, 91, 110) };
            var book = new Button { Text = "ĐẶT TOUR", Dock = DockStyle.Right, Width = 150, BackColor = xanh, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            book.FlatAppearance.BorderSize = 0; book.Click += (s, e) => TaoBooking(); bottom.Controls.Add(lblChiTiet); bottom.Controls.Add(book);
            Controls.Add(grid); Controls.Add(bottom); Controls.Add(top);
        }

        private void TaiDuLieu()
        {
            try
            {
                const string sql = @"SELECT d.MaDot, t.MaTour, t.TenTour, t.DiemDen, t.SoNgay, t.SoDem,
                    d.NgayDi, d.NgayVe, d.SoChoToiDa, v.SoChoCon,
                    (SELECT TOP 1 g.GiaNguoiLon FROM GiaTour g WHERE g.MaTour=t.MaTour AND g.TuNgay<=CAST(d.NgayDi AS DATE) AND (g.DenNgay IS NULL OR g.DenNgay>=CAST(d.NgayDi AS DATE)) ORDER BY g.TuNgay DESC) GiaNguoiLon,
                    t.MoTa FROM DotKhoiHanh d JOIN Tour t ON t.MaTour=d.MaTour
                    JOIN vw_DotConCho v ON v.MaDot=d.MaDot
                    WHERE t.TrangThai=1 AND d.TrangThai=N'Mở bán' AND d.NgayDi>SYSDATETIME() AND v.SoChoCon>0
                    ORDER BY d.NgayDi";
                duLieu = DbHelper.ExecuteDataTable(sql); grid.DataSource = duLieu;
                DinhDangCot("MaDot", "Mã đợt", 65); DinhDangCot("MaTour", "Mã tour", 60); DinhDangCot("TenTour", "Tên hành trình", 190); DinhDangCot("DiemDen", "Điểm đến", 110);
                DinhDangCot("SoNgay", "Ngày", 45); DinhDangCot("SoDem", "Đêm", 45); DinhDangCot("NgayDi", "Khởi hành", 100, "dd/MM/yyyy"); DinhDangCot("NgayVe", "Kết thúc", 100, "dd/MM/yyyy"); DinhDangCot("SoChoCon", "Còn chỗ", 65); DinhDangCot("GiaNguoiLon", "Giá người lớn", 110, "N0");
                foreach (string c in new[] { "SoChoToiDa", "MoTa" }) if (grid.Columns.Contains(c)) grid.Columns[c].Visible = false;
                LocDuLieu();
            }
            catch (Exception ex) { MessageBox.Show("Chưa tải được danh sách tour. Hãy chạy database/03_upgrade_uc01_uc30.sql trước.\n\n" + ex.Message, "TourManager", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void DinhDangCot(string name, string header, int width, string format = null)
        {
            if (grid == null || grid.Columns == null || !grid.Columns.Contains(name)) return;
            DataGridViewColumn c = grid.Columns[name];
            if (c == null) return;
            c.HeaderText = header;
            c.Width = width;
            if (!string.IsNullOrEmpty(format)) c.DefaultCellStyle.Format = format;
        }
        private void LocDuLieu()
        {
            if (duLieu == null || txtTim == null) return;
            string key = (txtTim.Text ?? "").Trim();
            // Escape special DataView.RowFilter LIKE characters, not only apostrophes.
            key = key.Replace("'", "''").Replace("[", "[[]").Replace("]", "[]]")
                     .Replace("%", "[%]").Replace("*", "[*]");
            try
            {
                duLieu.DefaultView.RowFilter = string.IsNullOrEmpty(key) ? "" :
                    "Convert(TenTour, 'System.String') LIKE '%" + key +
                    "%' OR Convert(DiemDen, 'System.String') LIKE '%" + key + "%'";
            }
            catch (EvaluateException)
            {
                duLieu.DefaultView.RowFilter = "";
            }
        }

        private void HienChiTiet()
        {
            if (lblChiTiet == null || grid == null || grid.CurrentRow == null ||
                grid.CurrentRow.IsNewRow) return;
            foreach (string col in new[] { "MaDot", "TenTour", "DiemDen", "SoNgay", "SoDem", "NgayDi", "SoChoCon", "GiaNguoiLon", "MoTa" })
                if (!grid.Columns.Contains(col)) return;
            var r = grid.CurrentRow;
            object ngay = r.Cells["NgayDi"].Value;
            string ngayDi = ngay == null || ngay == DBNull.Value ? "Chưa rõ" : Convert.ToDateTime(ngay).ToString("dd/MM/yyyy");
            object gia = r.Cells["GiaNguoiLon"].Value;
            string giaText = gia == null || gia == DBNull.Value ? "Chưa cấu hình" : Convert.ToDecimal(gia).ToString("N0") + " đ";
            lblChiTiet.Text = "Mã đợt: " + r.Cells["MaDot"].Value + "     |     " + r.Cells["TenTour"].Value + "\n" +
                "Điểm đến: " + r.Cells["DiemDen"].Value + "     |     Thời lượng: " + r.Cells["SoNgay"].Value + " ngày " + r.Cells["SoDem"].Value + " đêm\n" +
                "Khởi hành: " + ngayDi + "     |     Còn " + r.Cells["SoChoCon"].Value + " chỗ     |     Giá người lớn: " + giaText + "\n" +
                Convert.ToString(r.Cells["MoTa"].Value);
        }

        private void TaoBooking()
        {
            if (!PhanQuyen.CoQuyen(PhanQuyen.Admin, PhanQuyen.BanHang, PhanQuyen.KhachHang)) { MessageBox.Show("Tài khoản hiện tại chưa được cấp quyền đặt tour.", "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (grid.CurrentRow == null || !grid.Columns.Contains("MaDot") || grid.CurrentRow.Cells["MaDot"].Value == null) { MessageBox.Show("Vui lòng chọn đợt khởi hành."); return; }
            if (Session.TaiKhoan == null) { MessageBox.Show("Phiên đăng nhập không hợp lệ."); return; }
            using (var f = new frmThongTinDatTour())
            {
                if (PhanQuyen.CoQuyen(PhanQuyen.KhachHang))
                {
                    if (!Session.TaiKhoan.MaKhachHang.HasValue) { MessageBox.Show("Tài khoản chưa có hồ sơ khách hàng."); return; }
                    try
                    {
                        var info = DbHelper.ExecuteDataTable("SELECT HoTen,CCCD,SoDienThoai,Email FROM KhachHang WHERE MaKhachHang=@Id",
                            new SqlParameter("@Id", Session.TaiKhoan.MaKhachHang.Value));
                        if (info.Rows.Count == 0) { MessageBox.Show("Không tìm thấy hồ sơ khách hàng."); return; }
                        f.GanKhachHang(info.Rows[0]);
                    }
                    catch (Exception ex) { MessageBox.Show("Không tải được hồ sơ: " + ex.Message); return; }
                }
                if (f.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    long maBooking;
                    using (var cn = DbHelper.TaoKetNoi())
                    {
                        cn.Open(); using (var tx = cn.BeginTransaction())
                        {
                            try
                            {
                                int maKhach;
                                if (Session.TaiKhoan.MaKhachHang.HasValue)
                                {
                                    maKhach = Session.TaiKhoan.MaKhachHang.Value;

                                }
                                else using (var cmd = new SqlCommand(@"IF EXISTS(SELECT 1 FROM KhachHang WHERE CCCD=@CCCD) UPDATE KhachHang SET HoTen=@Ten,SoDienThoai=@SDT,Email=@Email WHERE CCCD=@CCCD;
                                    ELSE INSERT KhachHang(HoTen,CCCD,SoDienThoai,Email) VALUES(@Ten,@CCCD,@SDT,@Email);
                                    SELECT MaKhachHang FROM KhachHang WHERE CCCD=@CCCD;", cn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@Ten", f.HoTen); cmd.Parameters.AddWithValue("@CCCD", f.CCCD); cmd.Parameters.AddWithValue("@SDT", f.SoDienThoai); cmd.Parameters.AddWithValue("@Email", (object)f.Email ?? DBNull.Value); maKhach = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                using (var cmd = new SqlCommand("sp_TaoBooking", cn, tx))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure; cmd.Parameters.AddWithValue("@MaDot", Convert.ToInt32(grid.CurrentRow.Cells["MaDot"].Value)); cmd.Parameters.AddWithValue("@MaKhachHang", maKhach); cmd.Parameters.AddWithValue("@MaTaiKhoan", Session.TaiKhoan.MaTaiKhoan); cmd.Parameters.AddWithValue("@SoNguoiLon", f.SoNguoiLon); cmd.Parameters.AddWithValue("@SoTreEm", f.SoTreEm); cmd.Parameters.AddWithValue("@PhuThu", 0m); cmd.Parameters.AddWithValue("@MaCode", (object)f.MaKhuyenMai ?? DBNull.Value); cmd.Parameters.AddWithValue("@GhiChu", "Tạo từ màn hình Khám phá & đặt tour");
                                    using (var rd = cmd.ExecuteReader()) { if (!rd.Read()) throw new Exception("Không nhận được mã booking."); maBooking = Convert.ToInt64(rd["MaBooking"]); }
                                }
                                tx.Commit();
                            }
                            catch { tx.Rollback(); throw; }
                        }
                    }
                    MessageBox.Show("Đặt tour thành công!\nMã booking: " + maBooking + "\nTrạng thái: Giữ chỗ\nKhách hàng có thể vào Đơn hàng của tôi để gửi yêu cầu thanh toán.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information); TaiDuLieu();
                }
                catch (SqlException ex) { MessageBox.Show(ex.Message, "Không thể đặt tour", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                catch (Exception ex) { MessageBox.Show("Không thể tạo booking: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }

    internal class frmThongTinDatTour : Form
    {
        private TextBox ten, cccd, sdt, email, code;
        private NumericUpDown nguoiLon, treEm;
        public string HoTen { get { return ten.Text.Trim(); } } public string CCCD { get { return cccd.Text.Trim(); } }
        public string SoDienThoai { get { return sdt.Text.Trim(); } } public string Email { get { return email.Text.Trim(); } }
        public string MaKhuyenMai { get { return string.IsNullOrWhiteSpace(code.Text) ? null : code.Text.Trim(); } }
        public int SoNguoiLon { get { return (int)nguoiLon.Value; } } public int SoTreEm { get { return (int)treEm.Value; } }
        public frmThongTinDatTour()
        {
            Text = "Thông tin đặt tour"; Font = new Font("Segoe UI", 9.5F); StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; ClientSize = new Size(470, 440);
            ten = AddText("Họ và tên", 25, 25); cccd = AddText("CCCD / hộ chiếu", 25, 75); sdt = AddText("Số điện thoại", 25, 125); email = AddText("Email (không bắt buộc)", 25, 175); code = AddText("Mã khuyến mãi", 25, 225);
            AddLabel("Người lớn", 25, 275); nguoiLon = new NumericUpDown { Location = new Point(190, 270), Minimum = 1, Maximum = 50, Value = 1, Width = 90 };
            AddLabel("Trẻ em", 25, 315); treEm = new NumericUpDown { Location = new Point(190, 310), Minimum = 0, Maximum = 50, Width = 90 };
            var ok = new Button { Text = "XÁC NHẬN ĐẶT", Location = new Point(190, 365), Size = new Size(150, 38), BackColor = Color.FromArgb(0, 112, 205), ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; ok.FlatAppearance.BorderSize = 0; ok.Click += (s, e) => { if (HoTen.Length < 2 || CCCD.Length < 5 || SoDienThoai.Length < 8) { MessageBox.Show("Vui lòng nhập họ tên, CCCD/hộ chiếu và số điện thoại hợp lệ."); return; } if (SoNguoiLon + SoTreEm < 1) return; DialogResult = DialogResult.OK; Close(); };
            Controls.Add(nguoiLon); Controls.Add(treEm); Controls.Add(ok);
        }
        public void GanKhachHang(DataRow row)
        {
            ten.Text = Convert.ToString(row["HoTen"]); cccd.Text = Convert.ToString(row["CCCD"]);
            sdt.Text = Convert.ToString(row["SoDienThoai"]); email.Text = Convert.ToString(row["Email"]);
            foreach (var t in new[] { ten, cccd, sdt, email }) t.ReadOnly = true;
        }
        private TextBox AddText(string label, int x, int y) { AddLabel(label, x, y); var t = new TextBox { Location = new Point(190, y - 4), Width = 245 }; Controls.Add(t); if (label.StartsWith("Họ")) ten = t; else if (label.StartsWith("CCCD")) cccd = t; else if (label.StartsWith("Số")) sdt = t; else if (label.StartsWith("Email")) email = t; else code = t; return t; }
        private void AddLabel(string text, int x, int y) { Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true }); }
    }
}
