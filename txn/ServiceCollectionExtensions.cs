using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace txn
{
    public static class ServiceCollectionExtensions
    {
        public static void UseAllOfType<T>(this IServiceCollection serviceCollection, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.IsClass && x.GetInterfaces().Contains(typeof(T))));
            foreach (var type in typesFromAssemblies)
                serviceCollection.Add(new ServiceDescriptor(type, type, lifetime));
        }

        public static void UseSqlServer(this IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddScoped<IDbConnection>((serviceProvider) =>
            {
                return new SqlConnection(connectionString);
            });
        }

        public static void UseOneTransactionPerHttpCall(this IServiceCollection serviceCollection, IsolationLevel level = IsolationLevel.ReadUncommitted)
        {
            serviceCollection.AddScoped<IDbTransaction>((serviceProvider) =>
            {
                var connection = serviceProvider
                    .GetService<IDbConnection>();
                connection.Open();

                return connection.BeginTransaction(level);
            });

            serviceCollection.AddScoped(typeof(UnitOfWorkFilter), typeof(UnitOfWorkFilter));

            serviceCollection
                .AddMvc(setup =>
                {
                    setup.Filters.AddService<UnitOfWorkFilter>(1);
                });
        }
    }
}