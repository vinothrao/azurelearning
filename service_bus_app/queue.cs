using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace service_bus_app
{
    class queue
    {
        static QueueClient  queueClient ;

        static queue()
        {
            queueClient = new QueueClient("Connection String","queue=name");
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("1.Send");
            Console.WriteLine("2.Print");
       
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await SendMessage();
                    break;
                case "2":
                    await RecieveMessage();
                    break;             

            }
        }


        static async Task SendMessage()
        {
            var message = new Message(Encoding.UTF8.GetBytes("Hi from service bus"));
            await queueClient.SendAsync(message);
            await queueClient.CloseAsync();
        }

        static async Task RecieveMessage()
        {
            queueClient.RegisterMessageHandler(MessageHandler, ExceptionHandler);

            Console.ReadKey();

            await queueClient.CloseAsync();
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private static async Task MessageHandler(Message message, CancellationToken token)
        {
            Console.WriteLine(Encoding.UTF8.GetString(message.Body));
            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

        }
    }
}
