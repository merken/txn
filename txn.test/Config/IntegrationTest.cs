using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using txn;
using txn.Business;
using txn.Models;
using Xunit;

namespace txn.test
{
    public class TestContext
    {
        public MyItemsRepository ItemsRepository { get; set; }
        public BusinessLogic BusinessLogic { get; set; }
    }

    public abstract class IntegrationTest : IClassFixture<LocalDbFixture>
    {
        private readonly LocalDb localDb;
        private readonly MyItemsRepository repository;
        private readonly BusinessLogic businessLogic;

        protected IntegrationTest(LocalDbFixture localDb)
        {
            this.localDb = localDb.LocalDb;
        }

        protected async Task WithinTestContext(Func<TestContext, Task> codeUnderTest)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.UseAllOfType<IService>(new[] { typeof(Startup).Assembly }, ServiceLifetime.Scoped);
            serviceCollection.UseSqlServer(this.localDb.ConnectionString);
            serviceCollection.UseOneTransactionPerHttpCall();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var transaction = serviceProvider.GetService<IDbTransaction>();
            var context = new TestContext
            {
                ItemsRepository = serviceProvider.GetService<MyItemsRepository>(),
                BusinessLogic = serviceProvider.GetService<BusinessLogic>()
            };

            await codeUnderTest(context);

            transaction.Rollback();
        }
    }
}