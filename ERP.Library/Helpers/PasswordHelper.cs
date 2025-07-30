using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new();
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        public static bool IsPasswordValid(string pwd, string hashpwd)
        {
            return HashPassword(pwd) == hashpwd;
        }
    }
}
