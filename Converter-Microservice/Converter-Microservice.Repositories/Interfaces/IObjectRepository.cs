using ConverterMicroservice.Models;

namespace Converter_Microservice.Repositories.Interfaces
{
    public interface IObjectRepository
    {
        Task<bool> PutObjectAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectData> GetObjectAsync(string bucketName, string objectName);
    }
}
