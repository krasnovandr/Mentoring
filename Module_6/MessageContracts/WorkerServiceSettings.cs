using System;

namespace ServiceBusClient
{
    public class WorkerServiceSettings
    {
        public string BarcodeStopSequence { get; set; }
        public TimeSpan ProcessingTimeout { get; set; }
    }
}