using Helpers.Interfaces;
using Models.Health;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class HealthService : IHealthService
    {

        private IPingHelper _pingHelper;

        public HealthService(IPingHelper pingHelper)
        {
            _pingHelper = pingHelper;
        }

        public HealthResponse CheckHealthStatus(string RabbitMQHost, string ElasticHost, string StorageHost)
        {
            PingReply rabbitMQStatus = _pingHelper.PingRabbitMQ(RabbitMQHost);
            PingReply elasticSearchStatus = _pingHelper.PingElasticSearch(ElasticHost);
            PingReply storageStatus = _pingHelper.PingStorage(ElasticHost);

            HealthResponse healthResponse = new HealthResponse();

            healthResponse.StorageStatus = storageStatus.Status == (int)IPStatus.Success ? "S3 Storage is working!" : "S3 Storage is not working!";
            healthResponse.RabbitMQStatus = rabbitMQStatus.Status == (int)IPStatus.Success ? "RabbitMQ is working!" : "RabbitMQ is not working!";
            healthResponse.ElasticSearchStatus = elasticSearchStatus.Status == (int)IPStatus.Success ? "ElasticSearch is working!" : "ElasticSearch is not working!";

            return healthResponse;

        }
    }
}
