using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingLib
{
    class LogInformationReturn
    {
        public object ReturnValue { get; set; }
        public string MethodName { get; set; }
        public string Status { get; set; }
        public DateTime ExecutionTime { get; set; }
    }
}
