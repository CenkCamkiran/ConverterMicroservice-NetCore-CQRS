using Configuration;
using Minio;
using Minio.DataModel;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Net;

namespace DataAccess
{
    public class ObjectStorageHandler
    {
        public MinioClient ConnectMinio()
        {
            EnvVariablesHandler envVariablesHandler = new EnvVariablesHandler();
            MinioConfiguration minioConfiguration = envVariablesHandler.GetMinioEnvVariables();

            MinioClient minioClient = new MinioClient()
                                    .WithEndpoint(minioConfiguration.MinioHost)
                                    .WithCredentials(minioConfiguration.MinioAccessKey, minioConfiguration.MinioSecretKey)
                                    .WithSSL(false);

            return minioClient;
        }

        public async Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        {

            MinioClient minioClient = ConnectMinio();

            ServerSideEncryption? sse = null;
            stream.Position = 0;

            Dictionary<string, string> metadata = new Dictionary<string, string>()
            {
                {
                    "Id", objectName
                },
                {
                    "FileLength", stream.Length.ToString()
                },
                {
                    "ContentType", "video/mp4"
                }
            };

            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType("video/mp4")
                    .WithHeaders(metadata)
                    .WithServerSideEncryption(sse);
                await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                //await _minioClient.Build().PutObjectAsync(bucketName, objectName, stream, stream.Length, contentType).ConfigureAwait(false);

            }
            catch (Exception exception)
            {
                ElkLogging logging = new ElkLogging();

                ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
                {
                    ErrorMessage = exception.Message.ToString()
                };

                await logging.IndexExceptionAsync("converter_logs", exceptionModel);
            }

        }

        //public async Task GetFileAsync(string bucketName, string objectName, Stream stream, string contentType)
        //{
        //    ServerSideEncryption? sse = null;
        //    stream.Position = 0;

        //    Dictionary<string, string> metadata = new Dictionary<string, string>()
        //    {
        //        {
        //            "Id", objectName
        //        },
        //        {
        //            "FileLength", stream.Length.ToString()
        //        },
        //        {
        //            "ContentType", "video/mp4"
        //        }
        //    };

        //    try
        //    {
        //        var beArgs = new BucketExistsArgs()
        //            .WithBucket(bucketName);
        //        bool found = await _minioClient.Build().BucketExistsAsync(beArgs).ConfigureAwait(false);
        //        if (!found)
        //        {
        //            var mbArgs = new MakeBucketArgs()
        //                .WithBucket(bucketName);
        //            await _minioClient.Build().MakeBucketAsync(mbArgs).ConfigureAwait(false);
        //        }

        //        var putObjectArgs = new PutObjectArgs()
        //            .WithBucket(bucketName)
        //            .WithObject(objectName)
        //            .WithStreamData(stream)
        //            .WithObjectSize(stream.Length)
        //            .WithContentType("video/mp4")
        //            .WithHeaders(metadata)
        //            .WithServerSideEncryption(sse);
        //        await _minioClient.Build().PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        //        //await _minioClient.Build().PutObjectAsync(bucketName, objectName, stream, stream.Length, contentType).ConfigureAwait(false);

        //    }
        //    catch (Exception exception)
        //    {
        //        WebServiceErrors error = new WebServiceErrors();
        //        error.ErrorMessage = exception.Message.ToString();
        //        error.ErrorCode = (int)HttpStatusCode.InternalServerError;

        //        throw new Helpers.WebServiceException(JsonConvert.SerializeObject(error));
        //    }

        //}
    }
}
