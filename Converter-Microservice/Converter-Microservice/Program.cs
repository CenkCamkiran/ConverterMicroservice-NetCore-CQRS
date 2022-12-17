using DataAccess.Repository;
using Minio.DataModel;
using Models;

QueueRepository<object> queueHandler = new QueueRepository<object>();
ObjectStorageRepository objectStorageHandler = new ObjectStorageRepository();
ConverterRepository converterHandler = new ConverterRepository();

List<QueueMessage> messageList = queueHandler.ConsumeQueue("converter");

if (messageList.Any())
{
    foreach (var message in messageList)
    {
        try
        {
            ObjectDataModel objModel = await objectStorageHandler.GetFileAsync("videos", message.fileGuid);

            string guid = Guid.NewGuid().ToString();
            string ConvertToFilePath = Path.Combine(Path.GetTempPath(), guid + ".mp3");
            var conversionResult = converterHandler.ConvertMP4_to_MP3(objModel.FileFullPath, ConvertToFilePath);
            await Task.WhenAll(conversionResult);

            using(FileStream fs = File.OpenRead(ConvertToFilePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await fs.CopyToAsync(ms);
                    await objectStorageHandler.StoreFileAsync("audios", guid, ms, "audio/mp3");

                    QueueMessage msg = new QueueMessage()
                    {
                        email = message.email,
                        fileGuid = guid
                    };

                    queueHandler.QueueMessageDirect(msg, "notification", "notification_exchange.direct", "mp4_to_notif");
                }
            }

        }
        catch (Exception exception)
        {

            ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
            {
                ErrorMessage = exception.Message.ToString()
            };
            queueHandler.QueueMessageDirect(exceptionModel, "errorlogs", "log_exchange.direct", "error_log");

        }
    }
}

//Convert to MP3

/* ************************************************************************************************* */

//Consume queue (get guid and email from message)
//Convert mp4 to mp3 (get mp4 object from videos bucket using guid)
//Get stream data of mp3
//Put stream data of mp3 as object to minio (audio bucket)
//send message to notification queue (message consists of guid of mp3 file and email)
//Write notif microservice!

/* ************************************************************************************************* */
