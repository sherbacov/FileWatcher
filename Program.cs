using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Installer;
using NLog;
using Topshelf;

namespace FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            //new FileWatherService.FileWatcher().MainFolder = Settings.Default.MainFolder;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            // application starts...

            HostFactory.Run(x =>                                
            {
                x.Service<FileWatcherManager>(s =>                       
                {
                    s.ConstructUsing(name => new FileWatcherManager());
                    s.WhenStarted(tc => tc.Start());              
                    s.WhenStopped(tc => tc.Stop());               
                });
                x.RunAsLocalSystem();                          

                x.SetDescription("Служба копирования файлов");    
                x.SetDisplayName("FileWatcher");                   
                x.SetServiceName("FileWatcher");                   
            });
        }

        public class FileWatcherManager
        {
            private readonly Logger logger = LogManager.GetCurrentClassLogger();
            WindsorContainer container;

            public void Start()
            {
                container = new WindsorContainer(new XmlInterpreter());

                // adds and configures all components using WindsorInstallers from executing assembly
                container.Install(FromAssembly.This());

                container.Register(Component.For<IConfig>().ImplementedBy<Config>().Named("Config"));
                container.Register(Component.For<IFileWather>().ImplementedBy<FileWatcher>());
                container.Register(Component.For<IFileSystemWather>().ImplementedBy<InternalFileSystemWather>());
                container.Register(Component.For<ISourceFolder>().ImplementedBy<SourceFolder>());


                var king = container.Resolve<IFileWather>();
                king.StartProcessing();
                logger.Info("Запуск успешно завершен.");
            }

            public void Stop()
            {
                logger.Info("Завершение работы.");

                // clean up, application exits
                container.Dispose();
            }
        }

    }
}
