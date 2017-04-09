using System;
using System.ComponentModel;
using System.Reflection;
using Topshelf;

namespace FileWatcher
{
    partial class Program
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

    }
}
