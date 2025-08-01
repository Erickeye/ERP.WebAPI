using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.Sftp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
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
        ResultModel<List<string>> ListFiles();
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
            var result = new ResultModel<string>();
            try
            {
                using var client = CreateClient();
                client.Connect();

                // 將檔案存到 root 目錄 `/` 下
                var remoteFilePath = $"/{file.FileName}";

                using var stream = file.OpenReadStream();
                client.UploadFile(stream, remoteFilePath);

                client.Disconnect();
                result.SetSuccess($"檔案成功上傳至 {remoteFilePath}");
            }
            catch (SftpPermissionDeniedException ex)
            {
                result.SetError(ErrorCodeType.PermissionDenied, $"權限被拒絕：{ex.Message}");
            }
            catch (Exception ex)
            {
                result.SetError(ErrorCodeType.Exception, $"上傳失敗：{ex.Message}");
            }
            return result;
        }

        public ResultModel<Stream?> DownloadFile(string remotePath)
        {
            var result = new ResultModel<Stream?>();
            using var client = CreateClient();
            try
            {
                client.Connect();

                if (!client.Exists(remotePath))
                {
                    client.Disconnect();
                    result.SetError(ErrorCodeType.NotFoundData);
                    return result;
                }

                var memoryStream = new MemoryStream();
                client.DownloadFile(remotePath, memoryStream);
                memoryStream.Position = 0;
                result.Data = memoryStream;
            }
            catch(Exception ex)
            {
                result.SetError(ErrorCodeType.Exception,ex.Message);
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
            
            client.Disconnect();
            return result;
        }

        public ResultModel<string> DeleteFile(string remotePath)
        {
            var result = new ResultModel<string>();
            using var client = CreateClient();
            try
            {
                client.Connect();
                if (!client.Exists(remotePath))
                {
                    client.Disconnect();
                    result.SetError(ErrorCodeType.NotFoundData);
                    return result;
                }

                client.DeleteFile(remotePath);
                result.SetSuccess("檔案已成功刪除");
            }
            catch(Exception ex)
            {
                result.SetError(ErrorCodeType.Exception, ex.Message);
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
            
            return result;
        }

        public ResultModel<List<string>> ListFiles()
        {
            var result = new ResultModel<List<string>>();
            using var client = CreateClient();
            client.Connect();

            if (!client.Exists("/"))
            {
                result.SetError(ErrorCodeType.NotFoundData, "遠端目錄不存在");
                return result;
            }

            var files = client.ListDirectory("/")
                              .Where(f => !f.IsDirectory && !f.Name.StartsWith("."))
                              .Select(f => f.Name)
                              .ToList();

            client.Disconnect();

            result.SetSuccess(files);

            return result;
        }
    }
}
