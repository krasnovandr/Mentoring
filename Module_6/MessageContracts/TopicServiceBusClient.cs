using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusClient
{
    public class TopicServiceBusClient
    {
        private readonly TopicClient _topicClient;
        private SubscriptionClient _subscriptionClient;
        const string TopicName = "SettingsTopic";

        public TopicServiceBusClient()
        {
            _topicClient = TopicClient.Create(TopicName);
        }
        public void CreateQueue()
        {
            var namespaceManager = NamespaceManager.Create();
            if (namespaceManager.TopicExists(TopicName) == false)
            {
                namespaceManager.CreateTopic(TopicName);
            }

        }

        public void CreateSubscription(string subscriptionName)
        {
            var namespaceManager = NamespaceManager.Create();

            if (!namespaceManager.SubscriptionExists(TopicName, subscriptionName))
                namespaceManager.CreateSubscription(TopicName, subscriptionName);

            _subscriptionClient = SubscriptionClient.Create(TopicName, subscriptionName, ReceiveMode.ReceiveAndDelete);
            _subscriptionClient.OnMessage(NewMessage);
        }


        private void NewMessage(BrokeredMessage obj)
        {
            var a = obj.GetBody<WorkerServiceSettings>();
        }

        public void SendSettings(string newSequence, TimeSpan timeSpan)
        {
            var settings = new WorkerServiceSettings
            {
                BarcodeStopSequence = newSequence,
                ProcessingTimeout = timeSpan
            };

            var message = new BrokeredMessage(settings)
            {
                TimeToLive = new TimeSpan(0, 1, 0)
            };

            _topicClient.Send(message);
        }
    }
}
