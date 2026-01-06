using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels
{
    public class FileModel
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        [SwaggerSchema("檔案名稱")]
        public string? FileName { get; set; }
        /// <summary>
        /// 檔案內容，格式為data url
        /// </summary>
        [SwaggerSchema("檔案內容，格式為data url")]
        public string? DataUrl { get; set; }
    }
}
