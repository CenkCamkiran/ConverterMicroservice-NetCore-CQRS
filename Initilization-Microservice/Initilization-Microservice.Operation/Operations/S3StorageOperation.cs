using Initilization_Microservice.Operation.Interfaces;
using Initilization_Microservice.Repository.Interfaces;

namespace Initilization_Microservice.Operation.Operations
{
    public class S3StorageOperation : IS3StorageOperation
    {

        private readonly IS3StorageRepository _s3StorageRepository;

        public S3StorageOperation(IS3StorageRepository s3StorageRepository)
        {
            _s3StorageRepository = s3StorageRepository;
        }

        public Task<bool> ConfigureS3Async(string bucketName)
        {
            return _s3StorageRepository.ConfigureS3Async(bucketName);
        }
    }
}
