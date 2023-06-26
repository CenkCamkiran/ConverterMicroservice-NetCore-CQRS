using Initilization_Microservice.Common.Events;
using Initilization_Microservice.Helpers;
using Initilization_Microservice.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Minio;

namespace Initilization_Microservice.Repository.Repositories
{
    public class S3StorageRepository : IS3StorageRepository
    {

        private readonly ILogger<S3StorageRepository> _logger;
        private readonly IMinioClient _minioClient;

        public S3StorageRepository(ILogger<S3StorageRepository> logger, IMinioClient minioClient)
        {
            _logger = logger;
            _minioClient = minioClient;
        }

        public async Task<bool> ConfigureS3Async(string bucketname)
        {
            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketname);
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketname);
                    await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                _logger.LogInformation(LogEvents.StorageBucketCreationPhase, LogEvents.StorageBucketCreationPhaseMessage);
                return await Task.FromResult(true);

            }
            catch (Exception exception)
            {
                _logger.LogError(LogEvents.QueueExchangeBindingPhaseError, exception.Message.ToString());
                throw new JobInitializerException(exception.Message.ToString());
            }
        }

    }
}
