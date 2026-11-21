using System;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;
using TourManager.DTO;

namespace TourManager.GUI
{
    /// <summary>UC05 – Quản lý danh mục tour (FR06).</summary>
    public class frmTour : Form
    {
        private readonly TourBLL _bll = new TourBLL();

        private TextBox txtTimKiem, txtMaTour, txtTenTour, txtDiemDen, txtMoTa;
        private CheckBox chkChiDangBan, chkDangBan;
        private DataGridView dgvTour;
        private NumericUpDown nudSoNgay, nudSoDem;
        private ComboBox cboPhuongTien;
        private Button btnTim, btnThemMoi, btnLuu, btnAn;
        private Label lblQuyen;

        private TourDTO _hienTai;      // null = đang thêm mới
        private bool _dangNap;
        private bool _duocSua;

        public frmTour()
        {
            _duocSua = PhanQuyen.CoQuyen(PhanQuyen.Admin);
            KhoiTao();
            Load += (s, e) => NapDanhSach(0);
        }

        private void KhoiTao()
        {
            Text = "Quản lý danh mục tour";
            Font = new Font("Segoe UI", 10F);
            Size = new Size(1020, 600);

            // Thanh tìm kiếm
            var pnlTren = new Panel { Dock = DockStyle.Top, Height = 48 };
            pnlTren.Controls.Add(new Label { Text = "Tìm kiếm", Location = new Point(12, 14), AutoSize = true });
            txtTimKiem = new TextBox { Location = new Point(86, 10), Width = 240 };
            btnTim = new Button { Text = "Tìm", Location = new Point(336, 8), Size = new Size(70, 30) };
            btnTim.Click += (s, e) => NapDanhSach(0);
            chkChiDangBan = new CheckBox { Text = "Chỉ tour đang bán", Location = new Point(420, 12), AutoSize = true };
            chkChiDangBan.CheckedChanged += (s, e) => NapDanhSach(0);
            pnlTren.Controls.AddRange(new Control[] { txtTimKiem, btnTim, chkChiDangBan });
            AcceptButton = btnTim;

            // Lưới danh sách
            dgvTour = new DataGridView
            {
                Dock = DockStyle.Left,
                Width = 520,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvTour.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaTour", HeaderText = "Mã", FillWeight = 12 });
            dgvTour.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenTour", HeaderText = "Tên tour", FillWeight = 45 });
            dgvTour.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayDemText", HeaderText = "Số ngày", FillWeight = 18 });
            dgvTour.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThaiText", HeaderText = "Trạng thái", FillWeight = 25 });
            dgvTour.SelectionChanged += dgvTour_SelectionChanged;

            // Vùng nhập liệu
            var pnlChiTiet = new Panel { Dock = DockStyle.Fill };
            pnlChiTiet.Controls.Add(Nhan("Mã tour", 20, 24));
            txtMaTour = new TextBox { Location = new Point(160, 20), Width = 120, ReadOnly = true };
            pnlChiTiet.Controls.Add(txtMaTour);

            pnlChiTiet.Controls.Add(Nhan("Tên tour (*)", 20, 62));
            txtTenTour = new TextBox { Location = new Point(160, 58), Width = 300, MaxLength = 150 };
            pnlChiTiet.Controls.Add(txtTenTour);

            pnlChiTiet.Controls.Add(Nhan("Điểm đến (*)", 20, 100));
            txtDiemDen = new TextBox { Location = new Point(160, 96), Width = 300, MaxLength = 100 };
            pnlChiTiet.Controls.Add(txtDiemDen);

            pnlChiTiet.Controls.Add(Nhan("Số ngày", 20, 138));
            nudSoNgay = new NumericUpDown { Location = new Point(160, 134), Width = 70, Minimum = 1, Maximum = 60, Value = 1 };
            pnlChiTiet.Controls.Add(nudSoNgay);
            pnlChiTiet.Controls.Add(Nhan("Số đêm", 250, 138));
            nudSoDem = new NumericUpDown { Location = new Point(320, 134), Width = 70, Minimum = 0, Maximum = 60, Value = 0 };
            pnlChiTiet.Controls.Add(nudSoDem);

            pnlChiTiet.Controls.Add(Nhan("Phương tiện chính", 20, 176));
            cboPhuongTien = new ComboBox { Location = new Point(160, 172), Width = 300, DropDownStyle = ComboBoxStyle.DropDown, MaxLength = 50 };
            cboPhuongTien.Items.AddRange(new object[] { "Xe du lịch", "Máy bay", "Máy bay + xe du lịch", "Xe giường nằm", "Tàu hỏa" });
            pnlChiTiet.Controls.Add(cboPhuongTien);

            chkDangBan = new CheckBox { Text = "Đang bán", Location = new Point(160, 210), AutoSize = true, Checked = true };
            pnlChiTiet.Controls.Add(chkDangBan);

            pnlChiTiet.Controls.Add(Nhan("Mô tả", 20, 246));
            txtMoTa = new TextBox { Location = new Point(160, 242), Size = new Size(300, 100), Multiline = true, ScrollBars = ScrollBars.Vertical };
            pnlChiTiet.Controls.Add(txtMoTa);

            btnThemMoi = new Button { Text = "Thêm mới", Location = new Point(20, 366), Size = new Size(100, 34) };
            btnThemMoi.Click += (s, e) => ThemMoi();
            btnLuu = new Button { Text = "Lưu", Location = new Point(130, 366), Size = new Size(100, 34) };
            btnLuu.BackColor = Color.FromArgb(43, 87, 154);
            btnLuu.ForeColor = Color.White;
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Click += btnLuu_Click;
            btnAn = new Button { Text = "Ẩn tour", Location = new Point(240, 366), Size = new Size(100, 34) };
            btnAn.Click += btnAn_Click;
            pnlChiTiet.Controls.AddRange(new Control[] { btnThemMoi, btnLuu, btnAn });

            lblQuyen = new Label
            {
                Text = "Bạn chỉ có quyền xem danh mục tour.",
                ForeColor = Color.Firebrick,
                Location = new Point(20, 416),
                AutoSize = true,
                Visible = !_duocSua
            };
            pnlChiTiet.Controls.Add(lblQuyen);

            Controls.Add(pnlChiTiet);
            Controls.Add(dgvTour);
            Controls.Add(pnlTren);
            pnlChiTiet.BringToFront();   // vùng Fill phải nằm trên cùng thứ tự docking

            if (!_duocSua)
            {
                foreach (Control c in new Control[] { txtTenTour, txtDiemDen, nudSoNgay, nudSoDem, cboPhuongTien, chkDangBan, txtMoTa })
                    c.Enabled = false;
                btnThemMoi.Enabled = false;
                btnLuu.Enabled = false;
                btnAn.Enabled = false;
            }
        }

        private static Label Nhan(string text, int x, int y)
        {
            return new Label { Text = text, Location = new Point(x, y), AutoSize = true };
        }

        // ---------- Nạp dữ liệu ----------
        private void NapDanhSach(int maCanChon)
        {
            try
            {
                var ds = _bll.LayDanhSach(txtTimKiem.Text, chkChiDangBan.Checked);
                _dangNap = true;
                dgvTour.DataSource = ds;
                _dangNap = false;

                if (ds.Count == 0) { ThemMoi(); return; }

                DataGridViewRow chon = dgvTour.Rows[0];
                if (maCanChon > 0)
                {
                    foreach (DataGridViewRow r in dgvTour.Rows)
                    {
                        if (((TourDTO)r.DataBoundItem).MaTour == maCanChon) { chon = r; break; }
                    }
                }
                dgvTour.ClearSelection();
                chon.Selected = true;
                dgvTour.CurrentCell = chon.Cells[0];
                HienThi((TourDTO)chon.DataBoundItem);
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách tour:\n" + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTour_SelectionChanged(object sender, EventArgs e)
        {
            if (_dangNap) return;
            if (dgvTour.CurrentRow == null) return;
            var t = dgvTour.CurrentRow.DataBoundItem as TourDTO;
            if (t != null) HienThi(t);
        }

        private void HienThi(TourDTO t)
        {
            _hienTai = t;
            txtMaTour.Text = t.MaTour.ToString();
            txtTenTour.Text = t.TenTour;
            txtDiemDen.Text = t.DiemDen;
            nudSoNgay.Value = Math.Min(Math.Max(t.SoNgay, (int)nudSoNgay.Minimum), (int)nudSoNgay.Maximum);
            nudSoDem.Value = Math.Min(Math.Max(t.SoDem, (int)nudSoDem.Minimum), (int)nudSoDem.Maximum);
            cboPhuongTien.Text = t.PhuongTienChinh ?? "";
            chkDangBan.Checked = t.TrangThai;
            txtMoTa.Text = t.MoTa ?? "";
            btnAn.Text = t.TrangThai ? "Ẩn tour" : "Hiện tour";
            btnAn.Enabled = _duocSua;
        }

        private void ThemMoi()
        {
            _hienTai = null;
            txtMaTour.Text = "(tự sinh)";
            txtTenTour.Clear();
            txtDiemDen.Clear();
            nudSoNgay.Value = 1;
            nudSoDem.Value = 0;
            cboPhuongTien.Text = "";
            chkDangBan.Checked = true;
            txtMoTa.Clear();
            btnAn.Text = "Ẩn tour";
            btnAn.Enabled = false;
            dgvTour.ClearSelection();
            if (_duocSua) txtTenTour.Focus();
        }

        // ---------- Lưu, ẩn/hiện ----------
        private void btnLuu_Click(object sender, EventArgs e)
        {
            var t = new TourDTO
            {
                MaTour = _hienTai == null ? 0 : _hienTai.MaTour,
                TenTour = txtTenTour.Text,
                DiemDen = txtDiemDen.Text,
                SoNgay = (int)nudSoNgay.Value,
                SoDem = (int)nudSoDem.Value,
                PhuongTienChinh = cboPhuongTien.Text.Trim(),
                TrangThai = chkDangBan.Checked,
                MoTa = txtMoTa.Text
            };
            try
            {
                int ma;
                if (_hienTai == null) ma = _bll.Them(t);
                else { _bll.Sua(t); ma = t.MaTour; }
                NapDanhSach(ma);
                MessageBox.Show("Đã lưu tour.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu tour:\n" + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAn_Click(object sender, EventArgs e)
        {
            if (_hienTai == null) return;
            bool dangBanMoi = !_hienTai.TrangThai;
            string hoi = dangBanMoi ? "Hiện lại tour này?" : "Ẩn tour này? Tour vẫn được giữ lại để không mất lịch sử booking.";
            if (MessageBox.Show(hoi, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                _bll.DatTrangThai(_hienTai.MaTour, dangBanMoi);
                NapDanhSach(_hienTai.MaTour);
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống:\n" + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
