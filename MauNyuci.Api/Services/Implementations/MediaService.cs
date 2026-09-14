using Amazon.S3;
using Amazon.S3.Transfer;
using MauNyuci.Api.Services.Interfaces;

namespace MauNyuci.Api.Services.Implementations
{
    public class MediaService : IMediaService
    {
        private readonly IConfiguration _config;
        private readonly AmazonS3Client _s3Client;

        public MediaService(IConfiguration config)
        {
            _config = config;

            var accessKey = _config["CloudflareR2:AccessKey"];
            var secretKey = _config["CloudflareR2:SecretKey"];
            var serviceUrl = _config["CloudflareR2:ServiceUrl"];

            var s3Config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File gambar tidak ditemukan atau kosong.");

            // Validasi ekstensi
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                throw new Exception("Hanya file JPG, JPEG, dan PNG yang diperbolehkan.");

            var bucketName = _config["CloudflareR2:BucketName"];
            var publicUrl = _config["CloudflareR2:PublicUrl"];

            // Buat nama file unik (contoh: profile/123e4567-e89b...-foto.jpg)
            var uniqueFileName = $"{folderName}/{Guid.NewGuid()}{ext}";

            using (var newMemoryStream = new MemoryStream())
            {
                await file.CopyToAsync(newMemoryStream);
                newMemoryStream.Position = 0;

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = uniqueFileName,
                    BucketName = bucketName,
                    ContentType = file.ContentType,
                    DisablePayloadSigning = true // Rekomendasi untuk kompatibilitas R2
                };

                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);
            }

            // Kembalikan URL publik agar bisa langsung diakses dari Flutter
            return $"{publicUrl}/{uniqueFileName}";
        }
    }
}