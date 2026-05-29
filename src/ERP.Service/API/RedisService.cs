using StackExchange.Redis;
using System.Text.Json;

namespace ERP.Service.API
{
    /// <summary>
    /// Redis 操作介面
    /// 提供基本 CRUD 快取操作
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// 新增或覆蓋快取
        /// </summary>
        /// <typeparam name="T">資料型別</typeparam>
        /// <param name="key">Redis Key</param>
        /// <param name="value">要儲存的物件</param>
        /// <param name="expiry">過期時間 (可為 null 表示不過期)</param>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// 取得快取資料
        /// </summary>
        /// <typeparam name="T">資料型別</typeparam>
        /// <param name="key">Redis Key</param>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// 刪除快取
        /// </summary>
        /// <param name="key">Redis Key</param>
        Task<bool> DeleteAsync(string key);

        /// <summary>
        /// 判斷 Key 是否存在
        /// </summary>
        /// <param name="key">Redis Key</param>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 更新快取 (若不存在則回傳 false)
        /// </summary>
        /// <typeparam name="T">資料型別</typeparam>
        /// <param name="key">Redis Key</param>
        /// <param name="value">更新資料</param>
        /// <param name="expiry">過期時間</param>
        Task<bool> UpdateAsync<T>(string key, T value, TimeSpan? expiry = null);
    }

    /// <summary>
    /// RedisHelper 實作類
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="connection">IConnectionMultiplexer</param>
        public RedisService(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        /// <summary>
        /// 新增或覆蓋資料
        /// </summary>
        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);

            return await _db.StringSetAsync(
                key,
                json,
                expiry,
                When.Always,
                CommandFlags.None
            );
        }

        /// <summary>
        /// 讀取資料
        /// </summary>
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);

            if (!value.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
        }

        /// <summary>
        /// 更新資料 (必須存在)
        /// </summary>
        public async Task<bool> UpdateAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (!await ExistsAsync(key))
                return false;

            return await SetAsync(key, value, expiry);
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        public async Task<bool> DeleteAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 判斷 Key 是否存在
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }
    }
}
