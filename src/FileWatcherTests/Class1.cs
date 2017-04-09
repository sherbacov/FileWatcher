using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FileWatcher;
using Xunit;
using Xunit.Abstractions;
using Xunit.Ioc.Autofac;
using Xunit.Sdk;


namespace FileWatcherTests
{
    public class ConfigureTestFramework : AutofacTestFramework
    {
        private const string TestSuffixConvention = "Tests";

        public ConfigureTestFramework(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith(TestSuffixConvention));
            //builder.RegisterType<TestOutputHelper>().As<ITestOutputHelper>().InstancePerLifetimeScope();

            builder.Register(context => new Xunit.Sdk.TestOutputHelper())
                .As<ITestOutputHelper>()
                .InstancePerLifetimeScope();

            // configure your container
            // e.g. builder.RegisterModule<TestOverrideModule>();

            Container = builder.Build();
        }
    }

    [UseAutofacTestFramework]
    public class SourceFolderTestsOld
    {
        private ISourceFolderManager _sourceFolderManager;

        public SourceFolderTestsOld()
        {
            
        }

        public SourceFolderTestsOld(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
           // _sourceFolderManager = sourceFolderManager;

        }

        [Fact]
        public void AssertThatWeDoStuff()
        {
            _outputHelper.WriteLine("Hello");
        }

        public void TestSourceFolderManager()
        {
            
        }

        private readonly ITestOutputHelper _outputHelper;
    }

}
