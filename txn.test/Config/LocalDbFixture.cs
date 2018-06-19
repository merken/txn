using System;

namespace txn.test
{
    public sealed class LocalDbFixture : IDisposable
    {
        public LocalDb LocalDb { get; private set; }
        public LocalDbFixture()
        {
            LocalDb = new LocalDb();
            LocalDb.Migrate();
        }

        public void Dispose()
        {
            LocalDb.Dispose();
        }
    }
}