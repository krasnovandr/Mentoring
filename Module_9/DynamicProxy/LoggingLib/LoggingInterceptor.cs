using System;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using NLog;

namespace LoggingLib
{
    public class LoggingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                var logInfo = new LogInformation
                {
                    Method = invocation.MethodInvocationTarget,
                    Parameters = invocation.Arguments,
                    ExecutionTime = DateTime.Now
                };

                logger.Log(LogLevel.Info, ToJson(logInfo));

                invocation.Proceed();

                var returnValue = new LogInformationReturn
                {
                    MethodName = invocation.MethodInvocationTarget.Name,
                    ReturnValue = invocation.ReturnValue
                };

                logger.Log(LogLevel.Info, ToJson(returnValue));
            }

            catch (Exception)
            {
                logger.Error("Target threw an exception!");
                throw;
            }
        }

        public static string ToJson(object value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }
    }
}
