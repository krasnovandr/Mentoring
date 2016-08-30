using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace MainService
{
    public class MessageHandler
    {
        private const string queueName = "filequeue";
        private Thread workThread;
        private QueueClient client;

        public MessageHandler()
        {
            workThread = new Thread(WorkProcedure);
            client = QueueClient.Create(queueName);
        }

        private void WorkProcedure()
        {
            do
            {
                //File.AppendAllText("Results.txt", OnMessage() + Environment.NewLine);
                OnMessage();
                Thread.Sleep(2000);
            }
            while (true);
        }


        public void Start()
        {
            //client.OnMessage(OnMessage);
             workThread.Start();
        }

        private void OnMessage()
        {
            BrokeredMessage largeMessage;
            try
            {
                largeMessage = Receive();

            }
            catch (Exception ex)
            {
                
                throw;
            }
            Stream largeMessageStream = largeMessage.GetBody<Stream>();

            largeMessageStream.Seek(0, SeekOrigin.Begin);
            var documentName = string.Format("result_{0}.pdf", DateTime.Now.ToString("yyyyMMdd_hh_mm_ssfff"));

            FileStream fileOut = new FileStream(documentName, FileMode.Create);
            largeMessageStream.CopyTo(fileOut);
            fileOut.Close();

            //int BufferSize = 8192;
            //var buffer = new byte[BufferSize];
            //using (var fileStream = File.Open(documentName, FileMode.Create, FileAccess.Write, FileShare.None))
            //{
            //    int bytesRead;
            //    while ((bytesRead = largeMessageStream.Read(buffer, 0, BufferSize)) > 0)
            //    {
            //        fileStream.Write(buffer, 0, bytesRead);
            //    }
            //}
        }

        public BrokeredMessage Receive()
        {
            // Create a memory stream to store the large message body.
            MemoryStream largeMessageStream = new MemoryStream();

            // Accept a message session from the queue.
            MessageSession session = client.AcceptMessageSession();
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
            BrokeredMessage largeMessage = new BrokeredMessage(largeMessageStream, true);
            return largeMessage;
        }

        public void Stop()
        {
        }
    }

}
