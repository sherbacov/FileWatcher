using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using NLog;
using IContainer = Autofac.IContainer;

namespace FileWatcher
{
    partial class Program
    {
        public class FileWatcherManager
        {
            private readonly Logger _logger = LogManager.GetCurrentClassLogger();
            ContainerBuilder _builder;

            IContainer container;

            public void Start()
            {
                _builder = new ContainerBuilder();

                _builder.RegisterModule(new FileWatcherModule());

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
