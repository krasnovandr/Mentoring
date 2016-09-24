using System;
using Newtonsoft.Json;
using NLog;
using PostSharp.Aspects;

namespace LoggingLib
{
    [Serializable]
    public class LoggerAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            var logInfo = new LogInformation
            {
                Status = "OnEntry",
                Method = args.Method,
                Parameters = args.Arguments,
                ExecutionTime = DateTime.Now
            };

            logger.Log(LogLevel.Info, ToJson(logInfo));


        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            var logInfo = new LogInformationReturn
            {
                Status = "OnExit",
                MethodName = args.Method.Name,
                ReturnValue = args.ReturnValue,
                ExecutionTime = DateTime.Now
            };

            logger.Log(LogLevel.Info, ToJson(logInfo));
        }


        //public override void OnSuccess(MethodExecutionArgs args)
        //{
        //}


        public string ToJson(object value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }
    }
}
