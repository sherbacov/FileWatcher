using System;
using System.ComponentModel;
using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using NLog;
using Topshelf;
using IContainer = Autofac.IContainer;

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
            private readonly Logger _logger = LogManager.GetCurrentClassLogger();
            ContainerBuilder _builder;

            IContainer container;

            public void Start()
            {
                _builder = new ContainerBuilder();


                //_builder.RegisterType<Config>().As<IConfig>();
                _builder.RegisterType<FileWatcher>().As<IFileWather>();
                _builder.RegisterType<InternalFileSystemWather>().As<IFileSystemWather>();
                _builder.RegisterType<SourceFolder>().As<ISourceFolder>().PropertiesAutowired(); ;
                _builder.RegisterType<SourceFolderManager>().As<ISourceFolderManager>();


                // Add the configuration to the ConfigurationBuilder.
                var configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddXmlFile("FileWatcher.xml");

                // Register the ConfigurationModule with Autofac.
                var module = new ConfigurationModule(configurationBuilder.Build());
                _builder.RegisterModule(module);
               container = _builder.Build();

                var king = container.Resolve<IFileWather>();

                king.StartProcessing();
                _logger.Info("Запуск успешно завершен.");
            }

            public void Stop()
            {
                _logger.Info("Завершение работы.");

                // clean up, application exits
                container.Dispose();
            }
        }

    }
}
