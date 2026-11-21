using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TourManager.DAL;

namespace TourManager.GUI
{
    /// <summary>Báo cáo doanh thu, tour phổ biến và tình hình lấp đầy các đợt.</summary>
    public class frmBaoCao : Form
    {
        private TabControl tabs;
        public frmBaoCao()
        {
            Text = "Báo cáo hoạt động"; Font = new Font("Segoe UI", 9.5F); BackColor = Color.FromArgb(245, 248, 252); WindowState = FormWindowState.Maximized;
            tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(16, 6) };
            tabs.TabPages.Add(TaoTab("Doanh thu", "SELECT Ngay, TenTour, DoanhThu, SoBooking FROM vw_BaoCaoDoanhThu ORDER BY Ngay DESC"));
            tabs.TabPages.Add(TaoTab("Tour được quan tâm", "SELECT MaTour,TenTour,SoBooking,SoKhach FROM vw_TopTour ORDER BY SoKhach DESC"));
            tabs.TabPages.Add(TaoTab("Tình hình khởi hành", "SELECT MaDot,TenTour,NgayDi,NgayVe,SoChoToiDa,SoKhach,TyLeLapDay FROM vw_ThongKeDot ORDER BY NgayDi DESC"));
            Controls.Add(tabs);
        }
        private TabPage TaoTab(string title, string sql)
        {
            var page = new TabPage(title) { BackColor = Color.White, Padding = new Padding(12) };
            var bar = new Panel { Dock = DockStyle.Top, Height = 48 };
            var reload = new Button { Text = "↻  Làm mới dữ liệu", Dock = DockStyle.Left, Width = 160, BackColor = Color.FromArgb(0, 112, 205), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            reload.FlatAppearance.BorderSize = 0;
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            reload.Click += (s, e) => Nap(grid, sql); bar.Controls.Add(reload); page.Controls.Add(grid); page.Controls.Add(bar); page.Tag = grid; Nap(grid, sql); return page;
        }
        private void Nap(DataGridView grid, string sql)
        {
            try { grid.DataSource = DbHelper.ExecuteDataTable(sql); foreach (DataGridViewColumn c in grid.Columns) { if (c.Name.Contains("DoanhThu")) c.DefaultCellStyle.Format = "N0"; if (c.Name.Contains("Ngay")) c.DefaultCellStyle.Format = "dd/MM/yyyy"; if (c.Name.Contains("TyLe")) c.DefaultCellStyle.Format = "N1"; } }
            catch (Exception ex) { MessageBox.Show("Không tải được báo cáo. Hãy chạy script nâng cấp database 03_upgrade_uc01_uc30.sql.\n\n" + ex.Message, "Báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}
