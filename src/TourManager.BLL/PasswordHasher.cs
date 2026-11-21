using System;
using System.Security.Cryptography;
using System.Text;

namespace TourManager.BLL
{
    /// <summary>Băm mật khẩu: SHA-256(salt + mật khẩu UTF-8). Salt 16 byte ngẫu nhiên (NFR02).</summary>
    public static class PasswordHasher
    {
        public static byte[] TaoSalt()
        {
            var salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] Bam(string matKhau, byte[] salt)
        {
            byte[] mk = Encoding.UTF8.GetBytes(matKhau ?? "");
            var tatCa = new byte[salt.Length + mk.Length];
            Buffer.BlockCopy(salt, 0, tatCa, 0, salt.Length);
            Buffer.BlockCopy(mk, 0, tatCa, salt.Length, mk.Length);
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(tatCa);
            }
        }

        public static bool KiemTra(string matKhau, byte[] salt, byte[] hashLuuTru)
        {
            byte[] hash = Bam(matKhau, salt);
            if (hashLuuTru == null || hash.Length != hashLuuTru.Length) return false;
            int khac = 0;                       // so sánh thời gian không đổi
            for (int i = 0; i < hash.Length; i++) khac |= hash[i] ^ hashLuuTru[i];
            return khac == 0;
        }
    }
}
