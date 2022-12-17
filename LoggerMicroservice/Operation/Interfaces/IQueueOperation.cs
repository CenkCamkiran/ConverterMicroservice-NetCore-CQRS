using Models;

namespace Operation.Interfaces
{
    public interface IQueueOperation
    {
        List<QueueMessage> ConsumeErrorLogsQueue(string queue);
        List<QueueMessage> ConsumeOtherLogsQueue(string queue);
    }
}
