using System;
using System.Drawing;
using System.Windows.Forms;
using TourManager.BLL;

namespace TourManager.GUI
{
    public class frmDuyetThanhToan : Form
    {
        private readonly DonHangKhachBLL bll = new DonHangKhachBLL();
        private DataGridView grid;
        public frmDuyetThanhToan()
        {
            PhanQuyen.YeuCau(PhanQuyen.Admin, PhanQuyen.BanHang);
            Text = "Xác nhận thanh toán"; WindowState = FormWindowState.Maximized; MinimumSize = new Size(850,500);
            Font = new Font("Segoe UI", 9.5f);
            var top = new Panel { Dock = DockStyle.Top, Height = 105, BackColor = Color.White };
            top.Controls.Add(new Label { Text = "Yêu cầu thanh toán chờ xác nhận", Location = new Point(20,12), AutoSize = true, Font = new Font("Segoe UI",18,FontStyle.Bold) });
            var reload = Nut("Làm mới",20, () => TaiDuLieu());
            var approve = Nut("Xác nhận đã nhận tiền",145, () => XuLy(true));
            var reject = Nut("Từ chối",355, () => XuLy(false));
            top.Controls.AddRange(new Control[] { reload, approve, reject });
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false, BackgroundColor = Color.White };
            Controls.Add(grid); Controls.Add(top); TaiDuLieu();
        }
        private Button Nut(string text, int x, Action action)
        {
            var b = new Button { Text = text, Location = new Point(x,60), Width = text.Length>12 ? 195 : 115, Height = 32,
                BackColor = Color.FromArgb(0,112,205), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            b.Click += (s,e) => action(); return b;
        }
        private void TaiDuLieu()
        {
            try { grid.DataSource = bll.ChoDuyet(); if (grid.Columns.Contains("SoTien")) grid.Columns["SoTien"].DefaultCellStyle.Format = "N0"; }
            catch (Exception ex) { MessageBox.Show("Không tải được yêu cầu: " + ex.Message); }
        }
        private void XuLy(bool chapNhan)
        {
            if (grid.CurrentRow == null || !grid.Columns.Contains("MaYeuCau") ||
                grid.CurrentRow.Cells["MaYeuCau"].Value == null)
            { MessageBox.Show("Chọn yêu cầu hợp lệ trước."); return; }
            string msg = chapNhan ? "Bạn đã kiểm tra và thực sự nhận được tiền?" : "Từ chối yêu cầu này?";
            if (MessageBox.Show(msg,"Xác nhận",MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { bll.XuLy(Convert.ToInt64(grid.CurrentRow.Cells["MaYeuCau"].Value),chapNhan); TaiDuLieu(); }
            catch (Exception ex) { MessageBox.Show(ex.Message,"Lỗi xử lý",MessageBoxButtons.OK,MessageBoxIcon.Warning); }
        }
    }
}
