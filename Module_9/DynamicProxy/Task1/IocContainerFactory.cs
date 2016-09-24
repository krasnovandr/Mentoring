using Castle.Windsor;

namespace Task1
{
    public static class IocContainerFactory
    {
        public static IWindsorContainer Container { get; private set; }

        public static void Create()
        {
            Container = new WindsorContainer();
            Container.Install(new Installer());
        }
    }
}
