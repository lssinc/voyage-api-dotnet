﻿using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Voyage.Data;
using Voyage.Models.Map;
using Voyage.Services;

namespace Voyage.Api
{
    /// <summary>
    /// This is vanilla autofac configuration based on:
    /// http://docs.autofac.org/en/latest/integration/webapi.html#quick-start
    /// </summary>
    public class ContainerConfig
    {
        public static IContainer Container { get; private set; }

        public static HubConfiguration HubConfiguration { get; private set; }

        public static void Register(HttpConfiguration httpConfig)
        {
            var builder = new ContainerBuilder();

            // Register the types in the container
            var connectionString = ConfigurationManager.ConnectionStrings["VoyageDataContext"].ConnectionString;
            builder.RegisterModule(new DataModule(connectionString));
            builder.RegisterModule<AutoMapperModule>();
            builder.RegisterModule<ServicesModule>();
            builder.RegisterModule(new WebModule(connectionString, httpConfig));

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();

            // Register the Web API controllers in external assemblies (extensions)
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();
            builder.RegisterApiControllers(assemblies).InstancePerRequest();

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(httpConfig);

            // Set the dependency resolver to be Autofac.
            Container = builder.Build();

            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(Container);

            // Configure Autofac for SignalR.
            var resolver = new AutofacDependencyResolver(Container);
            HubConfiguration = new HubConfiguration
            {
                Resolver = resolver
            };

            AddSignalRInjection(Container, resolver);
        }

        private static void AddSignalRInjection(IContainer container, IDependencyResolver resolver)
        {
            var updater = new ContainerBuilder();

            updater.RegisterInstance(resolver.Resolve<IConnectionManager>());
            updater.Update(container);
        }
    }
}
