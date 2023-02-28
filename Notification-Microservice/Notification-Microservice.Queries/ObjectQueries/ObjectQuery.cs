﻿using MediatR;
using NotificationMicroservice.Models;

namespace Notification_Microservice.Queries.ObjectQueries
{
    public class ObjectQuery : IRequest<ObjectData>
    {
        public string BucketName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;

        public ObjectQuery(string bucketName, string objectName)
        {
            BucketName = bucketName;
            ObjectName = objectName;
        }
    }
}