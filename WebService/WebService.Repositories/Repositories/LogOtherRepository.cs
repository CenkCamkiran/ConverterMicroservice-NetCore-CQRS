using WebService.Common.Constants;
using WebService.Models;
using WebService.Repositories.Interfaces;

namespace WebService.Repositories.Repositories
{
    public partial class LogOtherRepository : ILogOtherRepository
    {
        private readonly ILogRepository<QueueLog> _loggingRepositoryQueue;
        private readonly ILogRepository<ObjectStorageLog> _loggingRepositoryObjStorageLog;

        public LogOtherRepository(ILogRepository<QueueLog> loggingRepositoryQueue, ILogRepository<ObjectStorageLog> loggingRepositoryObjStorageLog)
        {
            _loggingRepositoryQueue = loggingRepositoryQueue;
            _loggingRepositoryObjStorageLog = loggingRepositoryObjStorageLog;
        }

        public async Task LogQueueOther(QueueLog queueLog)
        {
            await _loggingRepositoryQueue.IndexDocAsync(ProjectConstants.QueueLogsIndex, queueLog);

            string logText = $"Exchange: {queueLog.ExchangeName} - Queue: {queueLog.QueueName} - Routing Key: {queueLog.RoutingKey} - Message: (fileGuid: {queueLog.Message.fileGuid} && email: {queueLog.Message.email})";
        }

        public async Task LogStorageOther(ObjectStorageLog objectStorageLog)
        {
            await _loggingRepositoryObjStorageLog.IndexDocAsync(ProjectConstants.ObjectStorageLogsIndex, objectStorageLog);

            string logText = $"BucketName: {objectStorageLog.BucketName} - ObjectName: {objectStorageLog.ObjectName} - Content Type: {objectStorageLog.ContentType}";
        }
    }
}
