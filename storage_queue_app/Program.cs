using Microsoft.Azure.Storage;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;

namespace storage_queue_app
{
    class Program
    {
        static CloudStorageAccount storageAccount;

        static Program()
        {
            storageAccount = CloudStorageAccount.Parse("Connection String");

        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("1.Insert Message");
            Console.WriteLine("2.Peek Message");
            Console.WriteLine("3.Get Message");
            Console.WriteLine("4.Update Message");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await PostMessage();
                    break;
                case "2":
                    await PeekMessage();
                    break;
                case "3":
                    await GetMessage();
                    break;
                case "4":
                    await UpdateMessage();
                    break;

            }

        }


        static async Task<CloudQueue> GetQueueClient()
        {
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queueReference = queueClient.GetQueueReference("message");
            await queueReference.CreateIfNotExistsAsync();
            return queueReference;
        }


        static async Task PostMessage()
        {
            var queue = await GetQueueClient();
            var message = new CloudQueueMessage("this is new message");
            await queue.AddMessageAsync(message);
            Console.ReadLine();
        }
        static async Task UpdateMessage()
        {
            var queue = await GetQueueClient();
            var fromMinutes = TimeSpan.FromMinutes(1);

            var message = await queue.GetMessageAsync(visibilityTimeout: fromMinutes, new QueueRequestOptions(), null);
          
            message.SetMessageContent2("update message",true);
            queue.UpdateMessage(message, TimeSpan.FromSeconds(10), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
            
        }

        static async Task PeekMessage()
        {
            var queue = await GetQueueClient();

            var messages = await queue.PeekMessagesAsync(10);
            
            foreach (var message in messages)
            {
              
                Console.WriteLine(message.AsString);
            }
            Console.ReadLine();
        }

        static async Task GetMessage()
        {
            var queue = await GetQueueClient();
            var message = queue.GetMessage();
            Console.WriteLine(message.AsString);
            await queue.DeleteMessageAsync(message);
            Console.ReadLine();
        }
    }
}
