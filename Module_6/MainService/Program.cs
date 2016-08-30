using Topshelf;

namespace MainService
{
    static class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(
            hostConf => hostConf.Service<MessageHandler>(
                s =>
                {
                    s.ConstructUsing(() => new MessageHandler());
                    s.WhenStarted(serv => serv.Start());
                    s.WhenStopped(serv => serv.Stop());
                }));
        }
    }
}
