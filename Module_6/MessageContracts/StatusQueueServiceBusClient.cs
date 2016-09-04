using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusClient
{
    public class StatusQueueServiceBusClient
    {
        private readonly QueueClient _statusQueueClient;
        private const string StatusQueueName = "statusqueue";
        public event Action<WorkerServiceStatus> SettingsRecievedEvent;

        public StatusQueueServiceBusClient()
        {
            _statusQueueClient = QueueClient.Create(StatusQueueName);
        }

        public void CreateQueue()
        {
            var namespaceManager = NamespaceManager.Create();
            if (namespaceManager.QueueExists(StatusQueueName) == false)
            {
                namespaceManager.CreateQueue(StatusQueueName);
            }

            _statusQueueClient.OnMessage(Recieve);

        }

        private void Recieve(BrokeredMessage obj)
        {
            if (SettingsRecievedEvent != null)
            {
                SettingsRecievedEvent(obj.GetBody<WorkerServiceStatus>());
            }
        }

        public void Send(WorkerServiceStatus status)
        {
            var message = new BrokeredMessage(status);
            _statusQueueClient.Send(message);
        }
    }
}