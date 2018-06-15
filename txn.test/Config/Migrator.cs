using System.Data;
using System.IO;
using System.Linq;
using Dapper;

namespace txn.test {
    public class Migrator {
        private readonly IDbConnection connection;
        public Migrator (IDbConnection connection) {
            this.connection = connection;
        }

        public void Migrate () {
            using (connection) {
                connection.Open ();
                using (var transaction = connection.BeginTransaction ()) {
                    var scripts = GetMigrationScripts ();
                    foreach (var script in scripts) {
                        var content = ReadEmbeddedContent (script);
                        connection.Execute (content, null, transaction);
                    }

                    transaction.Commit ();
                }
            }
        }

        private string ReadEmbeddedContent (string resource) {
            using (var stream = typeof (Startup)
                .Assembly
                .GetManifestResourceStream (resource)) {
                using (var reader = new StreamReader (stream)) {
                    return reader.ReadToEnd ();
                }
            }
        }

        private string[] GetMigrationScripts () {
            return typeof (Startup)
                .Assembly
                .GetManifestResourceNames ()
                .Where (r => r.StartsWith ("txn.Scripts"))
                .OrderBy (s => s)
                .ToArray ();
        }
    }
}