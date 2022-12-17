using DataAccess.Repository;
using Models;
using Operation.Operations;

//Run below 2 functions as parallel (Async or thread?) 
QueueOperation queueOperation = new QueueOperation();
List<QueueMessage> errorLogMessageList = queueOperation.ConsumeErrorLogsQueue("errorlogs");
List<QueueMessage> otherLogMessageList = queueOperation.ConsumeOtherLogsQueue("otherlogs");

LoggingRepository<OtherLog> loggingRepository = new LoggingRepository<OtherLog>();
//await loggingRepository.IndexDocAsync("");

