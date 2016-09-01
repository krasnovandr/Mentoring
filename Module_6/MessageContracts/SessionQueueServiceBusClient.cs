using System;
using System.IO;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusClient
{
    public class SessionQueueServiceBusClient
    {
        private readonly QueueClient _queueClient;
        private const string QueueName = "filequeue";
        private const int SubMessageBodySize = 192 * 1024;

        public SessionQueueServiceBusClient()
        {
            _queueClient = QueueClient.Create(QueueName);
        }

        public void CreateQueue()
        {
            var namespaceManager = NamespaceManager.Create();
            if (namespaceManager.QueueExists(QueueName) == false)
            {
                var description = new QueueDescription(QueueName)
                {
                    RequiresSession = true
                };
                namespaceManager.CreateQueue(description);
            }

        }

        public void SendFile(Stream file)
        {
            BrokeredMessage message = new BrokeredMessage(file, true);
            SplitAndSend(message);
            //queueClient.Close();
        }

        private void SplitAndSend(BrokeredMessage message)
        {
            // Calculate the number of sub messages required.
            long messageBodySize = message.Size;

            // Create a unique session Id.
            string sessionId = Guid.NewGuid().ToString();

            var bodyStream = message.GetBody<Stream>();
            for (int streamOffest = 0; streamOffest < messageBodySize; streamOffest += SubMessageBodySize)
            {
                // Get the stream chunk from the large message
                long arraySize = (messageBodySize - streamOffest) > SubMessageBodySize
                    ? SubMessageBodySize : messageBodySize - streamOffest;
                var subMessageBytes = new byte[arraySize];
                var result = bodyStream.Read(subMessageBytes, 0, (int)arraySize);
                var subMessageStream = new MemoryStream(subMessageBytes);

                var subMessage = new BrokeredMessage(subMessageStream, true)
                {
                    SessionId = sessionId
                };

                _queueClient.Send(subMessage);
            }
        }

        public void Receive()
        {
            MemoryStream largeMessage = AgregateAndReceive();

            //Stream largeMessageStream = largeMessage.GetBody<Stream>();

            largeMessage.Seek(0, SeekOrigin.Begin);
            var documentName = string.Format("result_{0}.pdf", DateTime.Now.ToString("yyyyMMdd_hh_mm_ssfff"));

            var fileOut = new FileStream(documentName, FileMode.Create);
            largeMessage.CopyTo(fileOut);
            fileOut.Close();
        }

        public MemoryStream AgregateAndReceive()
        {
            // Create a memory stream to store the large message body.
            var largeMessageStream = new MemoryStream();

            // Accept a message session from the queue.
            MessageSession session = _queueClient.AcceptMessageSession();
            //Console.WriteLine("Message session Id: " + session.SessionId);
            //Console.Write("Receiving sub messages");

            while (true)
            {
                // Receive a sub message
                BrokeredMessage subMessage = session.Receive(TimeSpan.FromSeconds(5));

                if (subMessage != null)
                {
                    // Copy the sub message body to the large message stream.
                    Stream subMessageStream = subMessage.GetBody<Stream>();
                    subMessageStream.CopyTo(largeMessageStream);

                    // Mark the message as complete.
                    subMessage.Complete();
                    //Console.Write(".");
                }
                else
                {
                    // The last message in the sequence is our completeness criteria.
                    //Console.WriteLine("Done!");
                    break;
                }
            }

            // Create an aggregated message from the large message stream.
            return largeMessageStream;
        }

    }
}