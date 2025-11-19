using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.Sftp;
using ERP.Library.ViewModels.UserInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.Sftp
{
    public interface ISftpService
    {
        ResultModel<string> UploadFile(IFormFile file);
        ResultModel<Stream?> DownloadFile(string remotePath);
        ResultModel<string> DeleteFile(string remotePath);
        ResultModel<ListResult<string>> ListFiles();
    }
    public class SftpService : ISftpService
    {
        private readonly SftpConfig _config;

        public SftpService(IOptions<SftpConfig> options)
        {
            _config = options.Value;
        }
        private SftpClient CreateClient()
        {
            return new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password);
        }


        public ResultModel<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ResultModel.Error(ErrorCodeType.ImgNotFound);
            }

            try
            {
                using var client = CreateClient();
                client.Connect();

                // 將檔案存到 root 目錄 `/` 下
                var remoteFilePath = $"/{file.FileName}";

                using var stream = file.OpenReadStream();
                client.UploadFile(stream, remoteFilePath);

                client.Disconnect();
                return ResultModel.Ok($"檔案成功上傳至 {remoteFilePath}");
            }
            catch (SftpPermissionDeniedException ex)
            {
                return ResultModel.Error(ErrorCodeType.PermissionDenied, $"權限被拒絕：{ex.Message}");
            }
            catch (Exception ex)
            {
                return ResultModel.Error(ErrorCodeType.Exception, $"上傳失敗：{ex.Message}");
            }
        }

        public ResultModel<Stream?> DownloadFile(string remotePath)
        {
            using var client = CreateClient();
            Stream? memoryStream = null;
            try
            {
                client.Connect();

                if (!client.Exists(remotePath))
                {
                    client.Disconnect();
                    return ResultModel.Error(ErrorCodeType.NotFoundData);
                }

                memoryStream = new MemoryStream();
                client.DownloadFile(remotePath, memoryStream);
                memoryStream.Position = 0;
            }
            catch(Exception ex)
            {
                return ResultModel.Error(ErrorCodeType.Exception, ex.Message);
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
            
            client.Disconnect();
            return ResultModel.Ok(memoryStream)!;
        }

        public ResultModel<string> DeleteFile(string remotePath)
        {
            using var client = CreateClient();
            try
            {
                client.Connect();
                if (!client.Exists(remotePath))
                {
                    client.Disconnect();
                    return ResultModel.Error(ErrorCodeType.NotFoundData);
                }

                client.DeleteFile(remotePath);
                return ResultModel.Ok("檔案已成功刪除");
            }
            catch(Exception ex)
            {
                return ResultModel.Error(ErrorCodeType.Exception, ex.Message);
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
        }

        public ResultModel<ListResult<string>> ListFiles()
        {
            var result = new ResultModel<List<string>>();
            using var client = CreateClient();
            client.Connect();

            if (!client.Exists("/"))
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "遠端目錄不存在");
            }

            var files = client.ListDirectory("/")
                              .Where(f => !f.IsDirectory && !f.Name.StartsWith("."))
                              .Select(f => f.Name)
                              .ToList();

            client.Disconnect();

            return ResultModel.Ok(files);
        }
    }
}
