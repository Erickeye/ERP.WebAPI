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

    public interface IDownloadFileModel
    {
        /// <summary>
        /// 下載類型
        /// </summary>
        [SwaggerSchema("下載類型")]
        public int SaveFormat { get; set; }
    }

    /// <summary>
    /// 下載Word類-模型
    /// </summary>
    public class DownloadAsposeWordFile
    {
        /// <summary>
        /// 下載類型(Aspose.Words.SaveFormat)
        /// </summary>
        [SwaggerSchema("下載類型[Docx:20;Pdf:40，預設:Pdf]")]
        public int SaveFormat { get; set; } = (int)Aspose.Words.SaveFormat.Pdf;
    }
    /// <summary>
    /// 下載Xslx類-模型
    /// </summary>
    public class DownloadAsposeXlsxFile
    {
        /// <summary>
        /// 下載類型(Aspose.Cells.SaveFormat)
        /// </summary>
        [SwaggerSchema("下載類型(Pdf:13;Xlsx:6;預設為Xlsx)")]
        public int SaveFormat { get; set; } = (int)Aspose.Cells.SaveFormat.Xlsx;
    }
}
