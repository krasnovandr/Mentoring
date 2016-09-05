using System;
using System.IO;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusClient
{
    public class FileQueueServiceBusClient
    {
        private const string FileQueueName = "filequeue";
        private const int SubMessageBodySize = 192 * 1024;

        private readonly QueueClient _fileQueueClient;

        public FileQueueServiceBusClient()
        {
            _fileQueueClient = QueueClient.Create(FileQueueName);

        }

        public void CreateQueue()
        {
            var namespaceManager = NamespaceManager.Create();
            if (namespaceManager.QueueExists(FileQueueName) == false)
            {
                var description = new QueueDescription(FileQueueName)
                {
                    RequiresSession = true
                };
                namespaceManager.CreateQueue(description);
            }
     
        }

        public void SendFile(Stream file)
        {
            var message = new BrokeredMessage(file, true);
            SplitAndSend(message);
        }

        private void SplitAndSend(BrokeredMessage message)
        {
            long messageBodySize = message.Size;

            string sessionId = Guid.NewGuid().ToString();

            var bodyStream = message.GetBody<Stream>();
            for (int streamOffest = 0; streamOffest < messageBodySize; streamOffest += SubMessageBodySize)
            {
                long arraySize = (messageBodySize - streamOffest) > SubMessageBodySize
                    ? SubMessageBodySize : messageBodySize - streamOffest;
                var subMessageBytes = new byte[arraySize];
                var result = bodyStream.Read(subMessageBytes, 0, (int)arraySize);
                var subMessageStream = new MemoryStream(subMessageBytes);

                var subMessage = new BrokeredMessage(subMessageStream, true)
                {
                    SessionId = sessionId
                };

                _fileQueueClient.Send(subMessage);
            }
        }

        public MemoryStream CheckAndReceiveFile()
        {

            MessageSession session;

            try
            {
                session = _fileQueueClient.AcceptMessageSession(TimeSpan.FromTicks(1));
            }
            catch (TimeoutException exception)
            {
                return null;
            }

            MemoryStream largeMessage = AgregateAndReceive(session);



            largeMessage.Seek(0, SeekOrigin.Begin);

            return largeMessage;

        }

        private MemoryStream AgregateAndReceive(MessageSession session)
        {
            var largeMessageStream = new MemoryStream();

            while (true)
            {
                // Receive a sub message
                BrokeredMessage subMessage = session.Receive(TimeSpan.FromSeconds(5));

                if (subMessage != null)
                {
                    var subMessageStream = subMessage.GetBody<Stream>();
                    subMessageStream.CopyTo(largeMessageStream);

                    subMessage.Complete();
                }
                else
                {
                    break;
                }
            }

            return largeMessageStream;
        }
    }
}
