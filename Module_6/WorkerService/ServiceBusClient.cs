using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace WorkerService
{
    public class ServiceBusClient
    {
        private QueueClient queueClient;
        private const string queueName = "filequeue";
        private static int SubMessageBodySize = 192*1024;


        public ServiceBusClient()
        {
            queueClient = QueueClient.Create(queueName, ReceiveMode.ReceiveAndDelete);

        }

        public void Send(Stream file)
        {
            var message = new BrokeredMessage(file, true);
            try
            {
                Send(message);

            }
            catch (Exception ex)
            {
                
                throw;
            }
            //queueClient.Close();
        }

        private void Send(BrokeredMessage message)
        {
            // Calculate the number of sub messages required.
            long messageBodySize = message.Size;
            int nrSubMessages = (int) (messageBodySize/SubMessageBodySize);
            if (messageBodySize%SubMessageBodySize != 0)
            {
                nrSubMessages++;
            }

            // Create a unique session Id.
            string sessionId = Guid.NewGuid().ToString();
            //Console.WriteLine("Message session Id: " + sessionId);
            //Console.Write("Sending {0} sub-messages", nrSubMessages);

            Stream bodyStream = message.GetBody<Stream>();
            for (int streamOffest = 0;
                streamOffest < messageBodySize;

                streamOffest += SubMessageBodySize)
            {
                // Get the stream chunk from the large message
                long arraySize = (messageBodySize - streamOffest) > SubMessageBodySize
                    ? SubMessageBodySize
                    : messageBodySize - streamOffest;
                byte[] subMessageBytes = new byte[arraySize];
                int result = bodyStream.Read(subMessageBytes, 0, (int) arraySize);
                MemoryStream subMessageStream = new MemoryStream(subMessageBytes);

                // Create a new message
                BrokeredMessage subMessage = new BrokeredMessage(subMessageStream, true);
                subMessage.SessionId = sessionId;

                // Send the message
                queueClient.Send(subMessage);
                //Console.Write(".");
            }
            //Console.WriteLine("Done!");
        }
    }
}