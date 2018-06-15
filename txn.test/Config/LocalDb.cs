    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Text;
    using System.Threading.Tasks;
    using System;
    using Dapper;

    namespace txn.test {

        public class LocalDb : IDisposable {
            private bool disposed;

            public const string DatabaseDirectory = "Data";

            public string ConnectionString { get; private set; }
            public string DatabaseName { get; private set; }
            public string OutputFolder { get; private set; }
            public string DatabaseMdfPath { get; private set; }
            public string DatabaseLogPath { get; private set; }

            public LocalDb () {

                DatabaseName = Guid.NewGuid ().ToString ("N");
                DatabaseName = $"LOCALDB_AUDIT_{DatabaseName}";
                // if numbers at start of name, trim those out
                DatabaseName = Regex.Replace (DatabaseName, @"^\d+", string.Empty);
                CreateDatabase ();
            }

            public IDbConnection Connection () {
                return new SqlConnection (ConnectionString);
            }

            private void CreateDatabase () {
                OutputFolder = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location),
                    DatabaseDirectory);
                var mdfFilename = $"{DatabaseName}.mdf";
                DatabaseMdfPath = Path.Combine (OutputFolder, mdfFilename);
                DatabaseLogPath = Path.Combine (OutputFolder, $"{DatabaseName}_log.ldf");

                // Create Data Directory If It Doesn't Already Exist.
                if (!Directory.Exists (OutputFolder)) {
                    Directory.CreateDirectory (OutputFolder);
                }

                // If the database does not already exist, create it.
                const string connectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
                using (var connection = new SqlConnection (connectionString)) {
                    connection.Open ();
                    var cmd = connection.CreateCommand ();
                    DetachDatabase ();
                    cmd.CommandText =
                        $"CREATE DATABASE {DatabaseName} ON (NAME = N'{DatabaseName}', FILENAME = '{DatabaseMdfPath}')";
                    cmd.ExecuteNonQuery ();
                }

                // Open newly created, or old database.
                ConnectionString =
                    $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDBFileName={DatabaseMdfPath};Initial Catalog={DatabaseName};Integrated Security=True;MultipleActiveResultSets=True;";
            }

            private void DetachDatabase () {
                try {
                    var connectionString =
                        @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
                    using (var connection = new SqlConnection (connectionString)) {
                        connection.Open ();
                        var cmd = connection.CreateCommand ();
                        cmd.CommandText =
                            string.Format (
                                "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{0}'",
                                DatabaseName);
                        cmd.ExecuteNonQuery ();
                    }
                } catch (SqlException) { } // SQLException if database does not exist
                finally {
                    if (File.Exists (DatabaseMdfPath)) File.Delete (DatabaseMdfPath);
                    if (File.Exists (DatabaseLogPath)) File.Delete (DatabaseLogPath);
                }
            }

            public void Migrate () {
                var migrator = new Migrator (Connection ());
                migrator.Migrate ();
            }

            public void Dispose () {
                    Dispose (true);
                    GC.SuppressFinalize (this);
                }

                ~LocalDb () {
                    Dispose (false);
                }

            protected virtual void Dispose (bool disposing) {
                if (disposed)
                    return;

                if (disposing) {
                    // free other managed objects that implement
                    // IDisposable only
                }

                // release any unmanaged objects
                // set the object references to null
                DetachDatabase ();

                disposed = true;
            }
        }
    }