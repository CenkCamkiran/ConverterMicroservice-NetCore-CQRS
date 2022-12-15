using DataAccess;
using Minio.DataModel;
using Models;
using System;

QueueHandler queueHandler = new QueueHandler();
ObjectStorageHandler objectStorageHandler = new ObjectStorageHandler();
ConverterHandler converterHandler = new ConverterHandler();

QueueMessage message = await queueHandler.ConsumeQueueAsync("converter");

if (message != null)
{
    SelectResponseStream response = await objectStorageHandler.GetFileAsync("videos", message.fileGuid);

    try
    {
        using (MemoryStream ms = new MemoryStream())
        {
            await response.Payload.CopyToAsync(ms);

            string guid = Guid.NewGuid().ToString();
            await objectStorageHandler.StoreFileAsync("audios", guid, ms, "audio/mp3");

            QueueMessage msg = new QueueMessage()
            {
                email = message.email,
                fileGuid = guid
            };

            await queueHandler.QueueMessageDirectAsync(msg, "notification", "notification_exchange.direct", "mp4_to_notif");

        }
    }
    catch (Exception exception)
    {
        ElkLogging<ConsumerExceptionModel> logging = new ElkLogging<ConsumerExceptionModel>();

        ConsumerExceptionModel exceptionModel = new ConsumerExceptionModel()
        {
            ErrorMessage = exception.Message.ToString()
        };

        await logging.IndexExceptionAsync("converter_logs", exceptionModel);
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
