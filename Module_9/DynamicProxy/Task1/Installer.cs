using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LoggingLib;

namespace Task1
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<LoggingInterceptor>());

            container.Register(
                Component.For<IPdfDocumentManager>()
                    .ImplementedBy<PdfDocumentManager>()
            .Interceptors(new InterceptorReference(typeof(LoggingInterceptor))).Anywhere
            .LifestyleSingleton());
        }
    }
}
