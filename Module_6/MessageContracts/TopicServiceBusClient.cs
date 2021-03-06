﻿using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBusClient
{
    public class TopicServiceBusClient
    {
        private readonly TopicClient _settingsTopicClient;
        private readonly TopicClient _statusTopicClient;
        private SubscriptionClient _settingsSubscriptionClient;
        private SubscriptionClient _statusSubscriptionClient;
        const string SettingsTopicName = "SettingsTopic";
        const string StatusTopicName = "StatusTopic";


        public event Action<WorkerServiceSettings> SettingsRecievedEvent;
        public event Action<string> StatusRecievedEvent;
        public TopicServiceBusClient()
        {
            _statusTopicClient = TopicClient.Create(StatusTopicName);
            _settingsTopicClient = TopicClient.Create(SettingsTopicName);
        }

        public void CreateTopics()
        {
            var namespaceManager = NamespaceManager.Create();
            if (namespaceManager.TopicExists(StatusTopicName) == false)
            {
                namespaceManager.CreateTopic(StatusTopicName);
            }

            if (namespaceManager.TopicExists(SettingsTopicName) == false)
            {
                namespaceManager.CreateTopic(SettingsTopicName);
            }

        }

        public void CreateSubscription(string subscriptionName)
        {
            var namespaceManager = NamespaceManager.Create();

            if (!namespaceManager.SubscriptionExists(StatusTopicName, subscriptionName))
                namespaceManager.CreateSubscription(StatusTopicName, subscriptionName);

            if (!namespaceManager.SubscriptionExists(SettingsTopicName, subscriptionName))
                namespaceManager.CreateSubscription(SettingsTopicName, subscriptionName);

            _statusSubscriptionClient = SubscriptionClient.Create(StatusTopicName, subscriptionName, ReceiveMode.ReceiveAndDelete);
            _statusSubscriptionClient.OnMessage(UpdateStatus);

            _settingsSubscriptionClient = SubscriptionClient.Create(SettingsTopicName, subscriptionName, ReceiveMode.ReceiveAndDelete);
            _settingsSubscriptionClient.OnMessage(NewSettings);
        }

        private void NewSettings(BrokeredMessage obj)
        {
            if (SettingsRecievedEvent != null)
            {
                SettingsRecievedEvent(obj.GetBody<WorkerServiceSettings>());
            }
        }

        private void UpdateStatus(BrokeredMessage obj)
        {
            if (StatusRecievedEvent != null)
            {
                StatusRecievedEvent(obj.GetBody<string>());
            }
        }

        public void SendSettings(WorkerServiceSettings settings)
        {
            var message = new BrokeredMessage(settings)
            {
                TimeToLive = new TimeSpan(0, 1, 0)
            };

            _settingsTopicClient.Send(message);
        }

        public void GetWorkerServiceStatus()
        {
            var message = new BrokeredMessage("Get Status")
            {
                TimeToLive = new TimeSpan(0, 1, 0)
            };

            _statusTopicClient.Send(message);
        }
    }
}
