using DataAccess.Repository;
using Models;
using Operation.Operations;
using RabbitMQ.Client;
using System.Threading.Channels;

QueueOperation<object> queueOperation = new QueueOperation<object>();
ObjectStorageOperation objectStorageOperation = new ObjectStorageOperation();
ConverterOperation ConverterOperation = new ConverterOperation();

List<QueueMessage> messageList = queueOperation.ConsumeQueue("converter");

foreach (var message in messageList)
{
    ObjectDataModel objModel = await objectStorageOperation.GetFileAsync("videos", message.fileGuid);
    await ConverterOperation.ConvertMP4_to_MP3(objModel, message);
}

Console.ReadLine();

//Convert to MP3

/* ************************************************************************************************* */

//Consume queue (get guid and email from message)
//Convert mp4 to mp3 (get mp4 object from videos bucket using guid)
//Get stream data of mp3
//Put stream data of mp3 as object to minio (audio bucket)
//send message to notification queue (message consists of guid of mp3 file and email)
//Write notif microservice!

/* ************************************************************************************************* */
