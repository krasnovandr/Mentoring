using System;
using System.Threading;
using ServiceBusClient;

namespace MainService
{
    public class MessageHandler
    {
        private Thread workThread;

        private readonly SessionQueueServiceBusClient _sessionQueueServiceBusClient;
        private readonly TopicServiceBusClient _topicServiceBusClient;
        public MessageHandler()
        {
            workThread = new Thread(WorkProcedure);
            _sessionQueueServiceBusClient = new SessionQueueServiceBusClient();
            _sessionQueueServiceBusClient.CreateQueue();

            _topicServiceBusClient = new TopicServiceBusClient(null,null);
            _topicServiceBusClient.CreateTopics();
        }

        private void WorkProcedure()
        {
            do
            {
                try
                {
                    _topicServiceBusClient.SendSettings("New Sequence", new TimeSpan(0, 1, 0));
                    _topicServiceBusClient.GetWorkerServiceStatus();
                    //_sessionQueueServiceBusClient.Receive();

                }
                catch (Exception exception)
                {

                    throw;
                }


                //OnMessage();
                Thread.Sleep(2000);
            }
            while (true);
        }


        public void Start()
        {
            //client.OnMessage(OnMessage);
        

            workThread.Start();
        }

     

      
        public void Stop()
        {
        }
    }

}
