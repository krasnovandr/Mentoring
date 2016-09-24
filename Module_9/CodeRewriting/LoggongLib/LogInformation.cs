using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace LoggingLib
{
    class LogInformation
    {
        public MethodBase Method { get; set; }
        public Arguments Parameters { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string Status { get; set; }
    }
}
