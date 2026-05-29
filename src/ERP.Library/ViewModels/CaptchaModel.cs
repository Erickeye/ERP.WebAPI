using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels
{
    /// <summary>
    /// 驗證碼資料。
    /// </summary>
    public sealed class WebCaptchaModel
    {
        [SwaggerSchema("驗證碼識別碼")]
        public string CaptchaId { get; set; } = string.Empty;

        [SwaggerSchema("驗證碼圖片，data URI 格式")]
        public string CaptchaImage { get; set; } = string.Empty;

        [SwaggerSchema("驗證碼語音，data URI 格式")]
        public string CaptchaAudio { get; set; } = string.Empty;

        [SwaggerSchema("驗證碼有效秒數")]
        public int ExpiresInSeconds { get; set; }
    }
}
