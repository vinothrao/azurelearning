using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace service_bus_app
{
    static class topic
    {
        static TopicClient topicClient;
        static SubscriptionClient subscriptionClient;

        static topic()
        {
            topicClient = new TopicClient("ConnectionString", "chatmessage");

           

            subscriptionClient = new SubscriptionClient(new ServiceBusConnectionStringBuilder(),"subscription name");

        }

        public static async Task SendTopicMessage()
        {
            subscriptionClient.RegisterMessageHandler(HandleMessage, ExceptionMessage);

            for (int i = 0; i <= 10; i++)
            {
                await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(i.ToString())));
            }
            await topicClient.CloseAsync();
            await subscriptionClient.CloseAsync();


            Console.ReadLine();
        }

        private static Task ExceptionMessage(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private static async Task HandleMessage(Message arg1, CancellationToken arg2)
        {
            Console.WriteLine(Encoding.UTF8.GetString(arg1.Body));
            Console.ReadLine();
        }
    }
}
