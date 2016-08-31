using System;
using System.Drawing;

namespace MessageContracts
{
    public class WorkerServiceSettings
    {
        public Bitmap BarcodeImage { get; set; }
        public string ImageName { get; set; }
        public TimeSpan ProcessingTimeout { get; set; }
    }
}