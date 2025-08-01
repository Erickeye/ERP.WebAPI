using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels.Sftp
{
    public class SftpConfig
    {
        public string Host { get; set; } = string.Empty;           // 伺服器 IP 或網域
        public int Port { get; set; } = 22;                         // SFTP: 22，FTPS 通常是 21 或 990
        public string Username { get; set; } = string.Empty;        // 帳號
        public string Password { get; set; } = string.Empty;        // 密碼
        public string RemotePath { get; set; } = "/";               // 要上傳到的遠端資料夾路徑
        public string? PrivateKeyPath { get; set; }                 // 如果使用金鑰驗證，可以放路徑
        public string? Passphrase { get; set; }                     // 金鑰密碼（如果有）
        public bool UseFTPS { get; set; } = false;                  // 若為 FTPS 請設為 true
    }
}
