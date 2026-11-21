using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TourManager.DAL
{
    /// <summary>Tạo kết nối và thực thi câu lệnh SQL có tham số.</summary>
    public static class DbHelper
    {
        private static string ChuoiKetNoi
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["TourManagerDB"];
                if (cs == null)
                    throw new InvalidOperationException("Thiếu chuỗi kết nối 'TourManagerDB' trong App.config.");
                return cs.ConnectionString;
            }
        }

        public static SqlConnection TaoKetNoi()
        {
            return new SqlConnection(ChuoiKetNoi);
        }

        /// <summary>Tạo tham số; giá trị null được chuyển thành DBNull.</summary>
        public static SqlParameter Param(string ten, object giaTri)
        {
            return new SqlParameter(ten, giaTri ?? DBNull.Value);
        }

        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] ps)
        {
            using (var cn = TaoKetNoi())
            using (var cmd = new SqlCommand(sql, cn))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            using (var cn = TaoKetNoi())
            using (var cmd = new SqlCommand(sql, cn))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using (var cn = TaoKetNoi())
            using (var cmd = new SqlCommand(sql, cn))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                cn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
