using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLRIMOA.Library.Enums;
//using TLRIMOA.Library.ViewModels.AnimalManagement.GrassManagement;
//using X.PagedList;

namespace TLRIMOA.Library.ViewModels
{
	/// <summary>
	/// 結果模型
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ResultModel<T>
	{
		/// <summary>
		/// 建構函數
		/// </summary>
		/// <param name="errorCode"></param>
		public ResultModel(ErrorCodeType? errorCode = null)
		{
			this.Message = new ResultMessageModel<T>();

			if (errorCode != null)
			{
				this.SetErrorCodeType(errorCode.Value);
			}
		}
		public ResultModel()
		{
			this.Message = new ResultMessageModel<T>();
		}
		public int Error { get; set; }
		public ErrorCodeType ErrorCode { get; set; }
		public string? ErrorMessage {  get; set; }
        public ResultMessageModel<T> Message { get; set; }

		public static implicit operator bool(ResultModel<T> vm)
		{
			return vm.Error == 0;
		}

		/// <summary>
		/// 設定錯誤代碼
		/// </summary>
		/// <param name="errorCode"></param>
		public void SetErrorCodeType(ErrorCodeType errorCode)
		{
			this.Error = 1;
			if (this.Message == null)
			{
				this.Message = new ResultMessageModel<T>();
			}
			this.ErrorCode = errorCode;
            this.Message.Code = (int)errorCode;
		}
        /// <summary>
        /// 設定錯誤代碼
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="data"></param>
        public void SetError(ErrorCodeType errorCode, string errorMessage)
		{
            this.Error = 1;
            this.ErrorCode = errorCode;
            this.Message.Code = (int)errorCode;
            this.ErrorMessage = errorMessage;
        }
        /// <summary>
        /// 設定錯誤代碼
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="data"></param>
        public void SetErrorCodeType(ErrorCodeType errorCode, T data)
        {
            this.Error = 1;
            if (this.Message == null)
            {
                this.Message = new ResultMessageModel<T>();
            }

            this.ErrorCode = errorCode;
            this.Message.Code = (int)errorCode;
            if (data != null)
            {
                this.Message.Data = data;
            }
        }
        /// <summary>
        /// 設定資料
        /// </summary>
        /// <param name="value">資料</param>
        public void SetData(T value)
		{
			this.Message = new ResultMessageModel<T>()
			{
				Data = value,
			};
		}
	}
	/// <summary>
	/// 結果訊息-模型
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ResultMessageModel<T>
	{
		/// <summary>
		/// 資料
		/// </summary>
		public T? Data { get; set; } = default;
		/// <summary>
		/// 錯誤代碼
		/// </summary>
		public int Code { get; set; } = default;
	}
}
