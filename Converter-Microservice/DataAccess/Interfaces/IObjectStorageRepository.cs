﻿using ConverterMicroservice.Models;

namespace ConverterMicroservice.DataAccessLayer.Interfaces
{
    public interface IObjectStorageRepository
    {
        Task StoreFileAsync(string bucketName, string objectName, Stream stream, string contentType);
        Task<ObjectData> GetFileAsync(string bucketName, string objectName);
    }
}
