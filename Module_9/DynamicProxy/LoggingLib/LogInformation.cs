using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoggingLib
{
    class LogInformation
    {
        public MethodBase Method { get; set; }
        public object[] Parameters { get; set; }
        public DateTime ExecutionTime { get; set; }
    }
}
