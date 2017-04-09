using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FileWatcher;
using Xunit;

namespace FileWatcherTests
{
    public class SourceFolderTests : FileWatcherTest
    {


        public SourceFolderTests()
        {

        }

        [Fact]
        public void SourceFolderList()
        {
            var sourceList = container.Resolve<ISourceFolderManager>();
            Assert.True(sourceList.Folders.Distinct().Count() == 2, "Количество папок источников с ошибкой");
        }
    }

    public class FileWatcherTest : IDisposable
    {
        protected IContainer container;

        public FileWatcherTest()
        {
            var _builder = new ContainerBuilder();

            _builder.RegisterModule(new FileWatcherModule());

            // Add the configuration to the ConfigurationBuilder.
            //var configurationBuilder = new ConfigurationBuilder();
            //configurationBuilder.AddXmlFile("FileWatcher.xml");

            // Register the ConfigurationModule with Autofac.
            // var module = new ConfigurationModule(configurationBuilder.Build());
            //_builder.RegisterModule(module);

            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);

            codeBasePath = $"{codeBasePath}.Test";

            if (!Directory.Exists(codeBasePath))
                Directory.CreateDirectory(codeBasePath);

            // Создаем тестовые папки
            var src1 = Path.Combine(codeBasePath, "src");
            var src2 = Path.Combine(codeBasePath, "src2");
            var dst1 = Path.Combine(codeBasePath, "dst");
            var dst2 = Path.Combine(codeBasePath, "dst2");

            CreateFolder(src1);
            CreateFolder(src2);
            CreateFolder(dst1);
            CreateFolder(dst2);


            var testConfig = new Config();
            testConfig.SourceFolder = $"{src1};{src2}";
            testConfig.DestFolder = $"{dst1};{dst2}";

            _builder.Register(r => testConfig).As<IConfig>().SingleInstance();


            container = _builder.Build();

            var cnf = container.Resolve<IConfig>();

            var king = container.Resolve<IFileWather>();


            // ... initialize data in the test database ...
        }

        private static void CreateFolder(string src1)
        {
            if (!Directory.Exists(src1))
                Directory.CreateDirectory(src1);
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }




    }
}
