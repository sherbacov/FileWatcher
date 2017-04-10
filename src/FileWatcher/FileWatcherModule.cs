using Autofac;
using FileWatcher.Configuration;
using FileWatcher.Sources;

namespace FileWatcher
{
    public class FileWatcherModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Config>().As<IConfig>();
            builder.RegisterType<FileWatcher>().As<IFileWather>();
            builder.RegisterType<InternalFileSystemWather>().As<IFileSystemWather>();
            builder.RegisterType<SourceFolderWatñher>().As<ISourceFolder>().PropertiesAutowired(); ;
            builder.RegisterType<SourceFolderManager>().As<ISourceFolderManager>().SingleInstance();
        }
    }
}