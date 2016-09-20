using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Effort;
using VotieeBackend.Models;

namespace VotieeBackend.Utils
{
    public class IocInstaller
    {
        private static ContainerBuilder Registration(ContainerBuilder builder)
        {
            if (!Singleton.IsTesting)
            {
                builder.RegisterType<VotieeDbContext>()
                    .As<VotieeDbContext>()
                    .WithParameter("connectionString", "DefaultConnection")
                    .InstancePerLifetimeScope();
            }
            else
            {
                //Injecting inmemory DB when testing
                builder.RegisterType<VotieeDbContext>()
                    .As<VotieeDbContext>()
                    .WithParameter("connection", DbConnectionFactory.CreatePersistent("1"))
                    .SingleInstance()
                    .OnActivated(e => TestData.TestData.SetupTestData(e.Instance))
                    .OnRelease(e => e.Dispose());
            }


            return builder;
        }

        public static void Setup()
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // Register custom injections
            builder = Registration(builder);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}