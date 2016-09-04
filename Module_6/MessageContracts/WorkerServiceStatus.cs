using System;
using System.Xml.Serialization;

namespace ServiceBusClient
{
    public enum WorkerServiceStates
    {
        Iddle,
        WaitNewFile,
    }

    public class WorkerServiceSettings
    {
        public string BarcodeStopSequence { get; set; }
        [XmlIgnore]
        public TimeSpan ProcessingTimeout { get; set; }

        public string MyTimeout
        {
            get { return ProcessingTimeout.ToString(); }
            set { ProcessingTimeout = TimeSpan.Parse(value); }
        }
    }

    public class WorkerServiceStatus
    {
        public WorkerServiceStates CurrentState { get; set; }
        public WorkerServiceSettings CurrentSettings { get; set; }
        public string ClientName { get; set; }
    }
}