using System;
using System.Security.Cryptography;
using System.Text;
using SkiaSharp;
using ERP.Library.Enums;
using ERP.Library.ViewModels;

namespace ERP.Service.API
{
    public interface ICaptchaService
    {
        Task<WebCaptchaModel> GenerateAsync(CancellationToken cancellationToken = default);

        Task<ResultModel<string>> VerifyAsync(string? captchaId, string? captchaCode, CancellationToken cancellationToken = default);
    }

    public sealed class CaptchaService : ICaptchaService
    {
        private const string RedisKeyPrefix = "hispark:web:captcha:";
        private const string CharacterSet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string CaptchaAudioResourcePrefix = "hccg_hispark.Service.Assets.CaptchaAudio";
        private const int ImageWidth = 160;
        private const int ImageHeight = 52;
        private const int AudioSampleRate = 22050;
        private const short AudioChannels = 1;
        private const short AudioBitsPerSample = 16;
        private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(3);
        private static readonly SKColor BackgroundColor = SKColor.Parse("#F8FAFC");
        private static readonly SKColor BorderColor = SKColor.Parse("#CBD5E1");
        private static readonly SKColor PrimaryTextColor = SKColor.Parse("#1F2937");
        private static readonly SKColor AccentTextColor = SKColor.Parse("#0F766E");
        private static readonly SKColor NoiseColor = SKColor.Parse("#94A3B8");

        private readonly IRedisService _redisHelper;

        public CaptchaService(IRedisService redisHelper)
        {
            _redisHelper = redisHelper;
        }

        public async Task<WebCaptchaModel> GenerateAsync(CancellationToken cancellationToken = default)
        {
            var captchaCode = GenerateCode(5);
            var captchaId = Guid.NewGuid().ToString("N");
            var payload = new CaptchaCacheItem
            {
                CodeHash = ComputeHash(captchaCode),
                ExpiresAtUtc = DateTime.UtcNow.Add(Expiration)
            };

            await _redisHelper.SetAsync(BuildRedisKey(captchaId), payload, Expiration);

            return new WebCaptchaModel
            {
                CaptchaId = captchaId,
                CaptchaImage = BuildPngDataUri(captchaCode),
                CaptchaAudio = BuildWavDataUri(captchaCode),
                ExpiresInSeconds = (int)Expiration.TotalSeconds
            };
        }

        public async Task<ResultModel<string>> VerifyAsync(string? captchaId, string? captchaCode, CancellationToken cancellationToken = default)
        {
            var normalizedId = captchaId?.Trim();
            var normalizedCode = NormalizeCode(captchaCode);
            if (string.IsNullOrWhiteSpace(normalizedId) || string.IsNullOrWhiteSpace(normalizedCode))
            {
                return ResultModel.Error(ErrorCodeType.CaptchaRequired, "請先輸入驗證碼。");
            }

            var cacheKey = BuildRedisKey(normalizedId);
            var cacheItem = await _redisHelper.GetAsync<CaptchaCacheItem>(cacheKey);
            if (cacheItem == null)
            {
                return ResultModel.Error(ErrorCodeType.CaptchaExpired, "驗證碼已失效，請重新取得。");
            }

            var inputHash = ComputeHash(normalizedCode);
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(cacheItem.CodeHash),
                    Encoding.UTF8.GetBytes(inputHash)))
            {
                return ResultModel.Error(ErrorCodeType.InvalidCaptcha, "驗證碼錯誤。");
            }

            await _redisHelper.DeleteAsync(cacheKey);
            return ResultModel.Ok();
        }

        private static string BuildRedisKey(string captchaId)
        {
            return $"{RedisKeyPrefix}{captchaId}";
        }

        private static string GenerateCode(int length)
        {
            var chars = new char[length];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = CharacterSet[RandomNumberGenerator.GetInt32(CharacterSet.Length)];
            }

            return new string(chars);
        }

        private static string? NormalizeCode(string? captchaCode)
        {
            return string.IsNullOrWhiteSpace(captchaCode)
                ? null
                : captchaCode.Trim().ToUpperInvariant();
        }

        private static string ComputeHash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        private static string BuildPngDataUri(string captchaCode)
        {
            using var bitmap = new SKBitmap(ImageWidth, ImageHeight);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(BackgroundColor);

            using var borderPaint = new SKPaint
            {
                Color = BorderColor,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f
            };
            canvas.DrawRoundRect(new SKRoundRect(new SKRect(1, 1, ImageWidth - 1, ImageHeight - 1), 8, 8), borderPaint);

            DrawNoise(canvas);
            DrawCode(canvas, captchaCode);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return $"data:image/png;base64,{Convert.ToBase64String(data.ToArray())}";
        }

        private static string BuildWavDataUri(string captchaCode)
        {
            var samples = new List<short>(AudioSampleRate * 8);
            AppendSilence(samples, 180);

            foreach (var character in captchaCode)
            {
                samples.AddRange(ReadCharacterAudioSamples(character));
                AppendSilence(samples, 320);
            }

            return $"data:audio/wav;base64,{Convert.ToBase64String(BuildWaveFile(samples))}";
        }

        private static short[] ReadCharacterAudioSamples(char character)
        {
            var resourceName = $"{CaptchaAudioResourcePrefix}.{character}.wav";
            using var stream = typeof(CaptchaService).Assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return BuildFallbackTone(character);
            }

            return ReadPcm16MonoWaveSamples(stream);
        }

        private static short[] ReadPcm16MonoWaveSamples(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);
            if (ReadAscii(reader, 4) != "RIFF")
            {
                throw new InvalidDataException("Captcha audio asset is not a valid WAV file.");
            }

            reader.ReadInt32();
            if (ReadAscii(reader, 4) != "WAVE")
            {
                throw new InvalidDataException("Captcha audio asset is not a valid WAV file.");
            }

            short? audioFormat = null;
            short? channels = null;
            int? sampleRate = null;
            short? bitsPerSample = null;

            while (stream.Position + 8 <= stream.Length)
            {
                var chunkId = ReadAscii(reader, 4);
                var chunkSize = reader.ReadInt32();
                var nextChunkPosition = stream.Position + chunkSize + (chunkSize % 2);

                if (chunkId == "fmt ")
                {
                    audioFormat = reader.ReadInt16();
                    channels = reader.ReadInt16();
                    sampleRate = reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt16();
                    bitsPerSample = reader.ReadInt16();
                }
                else if (chunkId == "data")
                {
                    if (audioFormat != 1 ||
                        channels != AudioChannels ||
                        sampleRate != AudioSampleRate ||
                        bitsPerSample != AudioBitsPerSample)
                    {
                        throw new InvalidDataException("Captcha audio asset must be 22.05 kHz, 16-bit, mono PCM WAV.");
                    }

                    var data = reader.ReadBytes(chunkSize);
                    var samples = new short[data.Length / sizeof(short)];
                    Buffer.BlockCopy(data, 0, samples, 0, samples.Length * sizeof(short));
                    return TrimSilence(samples);
                }

                stream.Position = nextChunkPosition;
            }

            throw new InvalidDataException("Captcha audio asset does not contain a data chunk.");
        }

        private static short[] TrimSilence(short[] samples)
        {
            const int threshold = 180;
            var start = 0;
            while (start < samples.Length && Math.Abs(samples[start]) < threshold)
            {
                start++;
            }

            var end = samples.Length - 1;
            while (end > start && Math.Abs(samples[end]) < threshold)
            {
                end--;
            }

            if (start >= end)
            {
                return samples;
            }

            var trimmed = new short[end - start + 1];
            Array.Copy(samples, start, trimmed, 0, trimmed.Length);
            return trimmed;
        }

        private static void AppendSilence(List<short> samples, int durationMs)
        {
            var sampleCount = AudioSampleRate * durationMs / 1000;
            for (var i = 0; i < sampleCount; i++)
            {
                samples.Add(0);
            }
        }

        private static short[] BuildFallbackTone(char character)
        {
            var frequency = 420 + (CharacterSet.IndexOf(character) * 18);
            var sampleCount = AudioSampleRate * 260 / 1000;
            var samples = new short[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                var progress = i / (double)sampleCount;
                var envelope = Math.Sin(Math.PI * progress);
                var value = Math.Sin(2 * Math.PI * frequency * i / AudioSampleRate) * envelope * 0.28;
                samples[i] = (short)(value * short.MaxValue);
            }

            return samples;
        }

        private static byte[] BuildWaveFile(List<short> samples)
        {
            var dataSize = samples.Count * sizeof(short);
            var byteRate = AudioSampleRate * AudioChannels * AudioBitsPerSample / 8;
            var blockAlign = AudioChannels * AudioBitsPerSample / 8;

            using var stream = new MemoryStream(44 + dataSize);
            using var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true);
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataSize);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write(AudioChannels);
            writer.Write(AudioSampleRate);
            writer.Write(byteRate);
            writer.Write((short)blockAlign);
            writer.Write(AudioBitsPerSample);
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(dataSize);

            foreach (var sample in samples)
            {
                writer.Write(sample);
            }

            writer.Flush();
            return stream.ToArray();
        }

        private static string ReadAscii(BinaryReader reader, int count)
        {
            return Encoding.ASCII.GetString(reader.ReadBytes(count));
        }

        private static void DrawNoise(SKCanvas canvas)
        {
            using var linePaint = new SKPaint
            {
                Color = NoiseColor.WithAlpha(140),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.2f
            };

            for (var i = 0; i < 7; i++)
            {
                var start = new SKPoint(
                    RandomNumberGenerator.GetInt32(0, ImageWidth),
                    RandomNumberGenerator.GetInt32(0, ImageHeight));
                var control = new SKPoint(
                    RandomNumberGenerator.GetInt32(0, ImageWidth),
                    RandomNumberGenerator.GetInt32(0, ImageHeight));
                var end = new SKPoint(
                    RandomNumberGenerator.GetInt32(0, ImageWidth),
                    RandomNumberGenerator.GetInt32(0, ImageHeight));

                using var path = new SKPath();
                path.MoveTo(start);
                path.QuadTo(control, end);
                canvas.DrawPath(path, linePaint);
            }

            using var dotPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            for (var i = 0; i < 24; i++)
            {
                dotPaint.Color = (i % 2 == 0 ? PrimaryTextColor : AccentTextColor).WithAlpha((byte)RandomNumberGenerator.GetInt32(55, 110));
                var x = RandomNumberGenerator.GetInt32(5, ImageWidth - 5);
                var y = RandomNumberGenerator.GetInt32(5, ImageHeight - 5);
                var radius = RandomNumberGenerator.GetInt32(1, 3);
                canvas.DrawCircle(x, y, radius, dotPaint);
            }
        }

        private static void DrawCode(SKCanvas canvas, string captchaCode)
        {
            var baseX = 18f;

            for (var i = 0; i < captchaCode.Length; i++)
            {
                var glyph = captchaCode[i].ToString();
                var x = baseX + (i * 26f) + RandomNumberGenerator.GetInt32(-2, 3);
                var y = 35f + RandomNumberGenerator.GetInt32(-4, 5);
                var rotation = RandomNumberGenerator.GetInt32(-20, 21);
                var scaleX = 0.95f + (RandomNumberGenerator.GetInt32(0, 16) / 100f);
                var scaleY = 0.95f + (RandomNumberGenerator.GetInt32(0, 16) / 100f);

                using var shadowPaint = new SKPaint
                {
                    IsAntialias = true,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
                    TextSize = 26,
                    Color = SKColors.White.WithAlpha(110)
                };

                using var textPaint = new SKPaint
                {
                    IsAntialias = true,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
                    TextSize = 26,
                    Color = i % 2 == 0 ? PrimaryTextColor : AccentTextColor
                };

                canvas.Save();
                canvas.Translate(x, y);
                canvas.RotateDegrees(rotation);
                canvas.Scale(scaleX, scaleY);
                canvas.DrawText(glyph, 1.5f, 1.5f, shadowPaint);
                canvas.DrawText(glyph, 0, 0, textPaint);
                canvas.Restore();
            }
        }

        private sealed class CaptchaCacheItem
        {
            public string CodeHash { get; set; } = string.Empty;

            public DateTime ExpiresAtUtc { get; set; }
        }
    }
}
