using System;
using System.Windows.Forms;
using TourManager.BLL;

namespace TourManager.GUI
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var dangNhap = new frmDangNhap())
                {
                    if (dangNhap.ShowDialog() != DialogResult.OK) break;
                }

                var chinh = new frmMain();
                Application.Run(chinh);
                if (!chinh.DangXuat) break;     // đóng cửa sổ chính = thoát; chọn Đăng xuất = quay lại đăng nhập
                Session.Xoa();
            }
        }
    }
}
