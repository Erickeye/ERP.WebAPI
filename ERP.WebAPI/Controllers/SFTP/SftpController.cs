using ERP.Service.Sftp;
using ERP.Library.ViewModels;
using ERP.Library.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ERP.WebAPI.CustomAttributes;

namespace ERP.WebAPI.Controllers.SFTP
{
    [SwaggerTag("SFTP檔案")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "_1000Company")]
    public class SftpController : ControllerBase
    {
        private readonly ISftpService _sftpService;

        public SftpController(ISftpService sftpService)
        {
            _sftpService = sftpService;
        }


        [SwaggerOperation("上傳檔案")]
        [HttpPost,Route("Upload")]
        [Log(OperationActionType.Export,"上傳檔案")]
        public IActionResult UploadFile(IFormFile file)
        {
            var result = new ResultModel<string>();
            if (file == null || file.Length == 0)
            {
                result.SetError(ErrorCodeType.ImgNotFound);
                return Ok(result);
            }

            using var stream = file.OpenReadStream();
            result = _sftpService.UploadFile(file);
            return Ok(result);
        }

        [SwaggerOperation("下載檔案")]
        [HttpGet("download/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var result = _sftpService.DownloadFile("/" + fileName);

            if (!result.IsSuccess || result.Data == null)
            {
                return BadRequest(new ResultModel<string>
                {
                    ErrorCode = ErrorCodeType.NotFoundData,
                    Data = "找不到檔案"
                });
            }

            return File(result.Data, "application/octet-stream", fileName);
        }

        [SwaggerOperation("刪除檔案")]
        [HttpDelete("delete/{fileName}")]
        public IActionResult DeleteFile(string fileName)
        {
            var remotePath = "/" + fileName;
            var result = _sftpService.DeleteFile(remotePath);
            return Ok(result);
        }

        [SwaggerOperation("檢視檔案清單")]
        [HttpGet,Route("list")]
        public IActionResult ListFiles()
        {
            var result = _sftpService.ListFiles();
            return Ok(result);
        }
    }
}
